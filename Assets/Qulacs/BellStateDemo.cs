using System;
using System.Collections.Generic;
using System.Text;
using Mu5dvlp.Qulacs;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Demonstrates creating a Bell state (|00> + |11>) / √2 and displaying measurement results.
///
/// Usage:
///   1. Attach this script to an empty GameObject.
///   2. (Optional) Assign a UI Text to ResultText to display results on screen.
///   3. Press Play — results will appear in the Console.
/// </summary>
public class BellStateDemo : MonoBehaviour
{
    [Header("Number of measurement shots")]
    public int samplingCount = 1000;

    [Header("(Optional) UI Text for displaying results")]
    public Text resultText;

    void Start()
    {
        RunBellStateDemo();
    }

    void RunBellStateDemo()
    {
        // Create a 2-qubit quantum state
        using var state = new QuantumState(2);

        // Build circuit: H(0) -> CNOT(0, 1) produces Bell state (|00> + |11>) / √2
        using var circuit = new QuantumCircuit(2);
        circuit.H(0).CNOT(0, 1);

        // Apply circuit to state
        circuit.UpdateQuantumState(state);

        // Retrieve state vector
        var vector = state.GetStateVector();
        var sb = new StringBuilder();
        sb.AppendLine("=== Bell State ===");
        sb.AppendLine($"Qubit count : {state.QubitCount}");
        sb.AppendLine($"Squared norm: {state.GetSquaredNorm():F6}");
        sb.AppendLine();
        sb.AppendLine("State vector:");
        for (int i = 0; i < vector.Length; i++)
        {
            string basis = Convert.ToString(i, 2).PadLeft(state.QubitCount, '0');
            sb.AppendLine($"  |{basis}> : ({vector[i].Real:+0.000;-0.000}, {vector[i].Imaginary:+0.000;-0.000}i)");
        }

        // Sample the state
        var samples = state.Sampling(samplingCount);
        var counts = new Dictionary<ulong, int>();
        foreach (var s in samples)
        {
            counts.TryGetValue(s, out int c);
            counts[s] = c + 1;
        }

        sb.AppendLine();
        sb.AppendLine($"Sampling ({samplingCount} shots):");
        foreach (var kv in counts)
        {
            string basis = Convert.ToString((long)kv.Key, 2).PadLeft(state.QubitCount, '0');
            float ratio = (float)kv.Value / samplingCount * 100f;
            sb.AppendLine($"  |{basis}> : {kv.Value,5} ({ratio:F1}%)");
        }

        // Zero probability per qubit
        sb.AppendLine();
        sb.AppendLine("Zero probability:");
        for (int q = 0; q < state.QubitCount; q++)
        {
            sb.AppendLine($"  qubit[{q}] = {state.GetZeroProbability(q):F4}");
        }

        string result = sb.ToString();
        Debug.Log(result);

        if (resultText != null)
            resultText.text = result;
    }
}
