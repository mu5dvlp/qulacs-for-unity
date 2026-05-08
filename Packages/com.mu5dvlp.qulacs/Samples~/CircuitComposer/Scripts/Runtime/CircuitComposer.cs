using System;
using Mu5dvlp.Qulacs;
using UnityEngine;

public enum ComposerGate
{
    None, H, X, Y, Z, S, T, CNOT, SWAP, Measure
}

[Serializable]
public struct ComposerCell
{
    public ComposerGate gate;
    public int partnerQubit;
    public bool isControl;

    public bool IsEmpty => gate == ComposerGate.None;
    public bool IsTwoQubit => gate is ComposerGate.CNOT or ComposerGate.SWAP;

    public static ComposerCell Empty => new() { gate = ComposerGate.None, partnerQubit = -1 };
}

public class CircuitComposer : MonoBehaviour
{
    [SerializeField, Range(1, 8)] int qubitCount = 3;
    [SerializeField, Range(1, 16)] int stepCount = 8;

    ComposerCell[,] grid;
    ComposerGate selectedTool = ComposerGate.H;
    int pendingQubit = -1, pendingStep = -1;
    CircuitComposerUI view;

    public int QubitCount => qubitCount;
    public int StepCount => stepCount;
    public ComposerGate SelectedTool => selectedTool;
    public ComposerCell GetCell(int q, int s) => grid[q, s];

    void Start()
    {
        InitGrid();
        view = gameObject.AddComponent<CircuitComposerUI>();
        view.Init(this);
        RunCircuit();
    }

    void InitGrid()
    {
        grid = new ComposerCell[qubitCount, stepCount];
        for (int q = 0; q < qubitCount; q++)
            for (int s = 0; s < stepCount; s++)
                grid[q, s] = ComposerCell.Empty;
    }

    public void SelectTool(ComposerGate tool)
    {
        selectedTool = tool;
        CancelPending();
        view.OnToolChanged();
    }

    public void OnCellClicked(int qubit, int step)
    {
        if (selectedTool == ComposerGate.None)
        {
            EraseCellPair(qubit, step);
            RunCircuit();
            return;
        }
        if (selectedTool is ComposerGate.CNOT or ComposerGate.SWAP)
            HandleTwoQubitPlacement(qubit, step);
        else
            PlaceSingle(qubit, step);
    }

    void PlaceSingle(int qubit, int step)
    {
        EraseCellPair(qubit, step);
        grid[qubit, step] = new ComposerCell { gate = selectedTool, partnerQubit = -1 };
        view.RefreshCell(qubit, step);
        RunCircuit();
    }

    void HandleTwoQubitPlacement(int qubit, int step)
    {
        if (pendingQubit < 0)
        {
            EraseCellPair(qubit, step);
            pendingQubit = qubit;
            pendingStep = step;
            grid[qubit, step] = new ComposerCell
                { gate = selectedTool, partnerQubit = -1, isControl = true };
            view.RefreshCell(qubit, step);
            view.ShowStatus($"Select target qubit (step {step})");
            return;
        }
        if (step != pendingStep || qubit == pendingQubit)
        {
            CancelPending();
            HandleTwoQubitPlacement(qubit, step);
            return;
        }
        EraseCellPair(qubit, step);

        var ctrl = grid[pendingQubit, pendingStep];
        ctrl.partnerQubit = qubit;
        grid[pendingQubit, pendingStep] = ctrl;

        grid[qubit, step] = new ComposerCell
            { gate = selectedTool, partnerQubit = pendingQubit, isControl = false };

        view.RefreshCell(pendingQubit, pendingStep);
        view.RefreshCell(qubit, step);
        view.RefreshConnections();
        pendingQubit = pendingStep = -1;
        view.ShowStatus("");
        RunCircuit();
    }

    void CancelPending()
    {
        if (pendingQubit >= 0 && grid[pendingQubit, pendingStep].partnerQubit < 0)
        {
            grid[pendingQubit, pendingStep] = ComposerCell.Empty;
            view?.RefreshCell(pendingQubit, pendingStep);
        }
        pendingQubit = pendingStep = -1;
        view?.ShowStatus("");
    }

    void EraseCellPair(int q, int s)
    {
        var cell = grid[q, s];
        if (cell.IsEmpty) return;
        if (cell.IsTwoQubit && cell.partnerQubit >= 0)
        {
            grid[cell.partnerQubit, s] = ComposerCell.Empty;
            view.RefreshCell(cell.partnerQubit, s);
        }
        grid[q, s] = ComposerCell.Empty;
        view.RefreshCell(q, s);
        view.RefreshConnections();
    }

    public void AddQubit()
    {
        if (qubitCount >= 8) return;
        Resize(qubitCount + 1, stepCount);
    }

    public void RemoveQubit()
    {
        if (qubitCount <= 1) return;
        ClearRefsToQubit(qubitCount - 1);
        Resize(qubitCount - 1, stepCount);
    }

    public void AddStep()
    {
        if (stepCount >= 16) return;
        Resize(qubitCount, stepCount + 1);
    }

    public void RemoveStep()
    {
        if (stepCount <= 1) return;
        ClearRefsInStep(stepCount - 1);
        Resize(qubitCount, stepCount - 1);
    }

    void ClearRefsToQubit(int q)
    {
        for (int s = 0; s < stepCount; s++)
        {
            var c = grid[q, s];
            if (c.IsTwoQubit && c.partnerQubit >= 0)
                grid[c.partnerQubit, s] = ComposerCell.Empty;
        }
    }

    void ClearRefsInStep(int s)
    {
        for (int q = 0; q < qubitCount; q++)
        {
            var c = grid[q, s];
            if (c.IsTwoQubit && c.partnerQubit >= 0)
                grid[c.partnerQubit, s] = ComposerCell.Empty;
        }
    }

    void Resize(int nq, int ns)
    {
        var old = grid;
        int oq = qubitCount, os = stepCount;
        qubitCount = nq;
        stepCount = ns;
        grid = new ComposerCell[nq, ns];
        for (int q = 0; q < nq; q++)
            for (int s = 0; s < ns; s++)
                grid[q, s] = (q < oq && s < os) ? old[q, s] : ComposerCell.Empty;
        view.RebuildAll();
        RunCircuit();
    }

    public void ClearAll()
    {
        CancelPending();
        InitGrid();
        view.RebuildAll();
        RunCircuit();
    }

    public void RunCircuit()
    {
        if (view == null) return;

        using var state = new QuantumState(qubitCount);
        using var circuit = new QuantumCircuit(qubitCount);

        for (int s = 0; s < stepCount; s++)
            for (int q = 0; q < qubitCount; q++)
            {
                var c = grid[q, s];
                switch (c.gate)
                {
                    case ComposerGate.H: circuit.H(q); break;
                    case ComposerGate.X: circuit.X(q); break;
                    case ComposerGate.Y: circuit.Y(q); break;
                    case ComposerGate.Z: circuit.Z(q); break;
                    case ComposerGate.S: circuit.S(q); break;
                    case ComposerGate.T: circuit.T(q); break;
                    case ComposerGate.Measure: circuit.Measure(q, q); break;
                    case ComposerGate.CNOT when c.isControl && c.partnerQubit >= 0:
                        circuit.CNOT(q, c.partnerQubit);
                        break;
                    case ComposerGate.SWAP when c.isControl && c.partnerQubit >= 0:
                        circuit.SWAP(q, c.partnerQubit);
                        break;
                }
            }

        circuit.UpdateQuantumState(state);

        var sv = state.GetStateVector();
        var probs = new double[state.Dimension];
        for (int i = 0; i < probs.Length; i++)
        {
            var a = sv[i];
            probs[i] = a.Real * a.Real + a.Imaginary * a.Imaginary;
        }

        var qp = new double[qubitCount];
        for (int q = 0; q < qubitCount; q++)
            qp[q] = 1.0 - state.GetZeroProbability(q);

        view.ShowResults(probs, qp, circuit.CalculateDepth(), circuit.GateCount);
    }
}
