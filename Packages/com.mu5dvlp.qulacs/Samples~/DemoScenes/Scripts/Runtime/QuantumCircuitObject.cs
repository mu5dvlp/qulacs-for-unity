using System;
using System.Collections.Generic;
using Mu5dvlp.Qulacs;
using UnityEngine;

[Serializable]
public struct GateEntry
{
    public GateType gateType;
    public int controlIndex;
    public int targetIndex;
}

public class QuantumCircuitObject : MonoBehaviour
{
    [SerializeField] QubitObject qubitObjectPrefab;
    [SerializeField] QuantumGateObject gateObjectPrefab;
    [SerializeField] GameObject wire;

    [Space(20)]
    [SerializeField, Range(1, 64)] int qubitCount = 2;
    [SerializeField] List<GateEntry> gates = new();

    const float Spacing = 1.5f;

    List<QubitObject> qubitObjects = new();
    List<QuantumGateObject> gateObjects = new();

    void Start()
    {
        float wireLength = (gates.Count + 1) * Spacing;

        for (int i = 0; i < qubitCount; i++)
        {
            var qubitObj = Instantiate(qubitObjectPrefab, transform);
            qubitObj.gameObject.SetActive(true);
            qubitObj.transform.localPosition = new Vector3(0, -i * Spacing, 0);
            qubitObjects.Add(qubitObj);

            if (wire != null)
            {
                var wireObj = Instantiate(wire, transform);
                wireObj.gameObject.SetActive(true);
                wireObj.transform.localPosition = new Vector3(0, -i * Spacing, 0);
                wireObj.transform.localScale = new Vector3(wireObj.transform.localScale.x, wireLength, wireObj.transform.localScale.z);
            }
        }

        for (int gi = 0; gi < gates.Count; gi++)
        {
            var gate = gates[gi];
            var gateObj = Instantiate(gateObjectPrefab, transform);
            gateObj.gameObject.SetActive(true);
            float x = (gi + 1) * Spacing;
            float y = -gate.targetIndex * Spacing;
            gateObj.transform.localPosition = new Vector3(x, y, 0);
            gateObj.GateType = gate.gateType;
            gateObjects.Add(gateObj);

            float wireHeight = gate.gateType is GateType.CNOT
                ? (gate.targetIndex - gate.controlIndex)
                : 0f;
            gateObj.SetwireHeight(wireHeight);
        }

        RunCircuit();
    }

    public void RunCircuit()
    {
        if (qubitObjects.Count == 0) return;

        using var state = new QuantumState(qubitCount);
        using var circuit = new QuantumCircuit(qubitCount);

        foreach (var gate in gates)
        {
            switch (gate.gateType)
            {
                case GateType.X:
                    circuit.X(gate.targetIndex);
                    break;
                case GateType.Y:
                    circuit.Y(gate.targetIndex);
                    break;
                case GateType.Z:
                    circuit.Z(gate.targetIndex);
                    break;
                case GateType.H:
                    circuit.H(gate.targetIndex);
                    break;
                case GateType.CNOT:
                    circuit.CNOT(gate.controlIndex, gate.targetIndex);
                    break;
            }
        }

        circuit.UpdateQuantumState(state);

        for (int i = 0; i < qubitCount && i < qubitObjects.Count; i++)
        {
            double p1 = 1.0 - state.GetZeroProbability(i);
            double p0 = 1.0 - p1;
            qubitObjects[i].State.SetStateVector(new System.Numerics.Complex[]
            {
                new System.Numerics.Complex(Math.Sqrt(p0), 0),
                new System.Numerics.Complex(Math.Sqrt(p1), 0),
            });
        }
    }
}
