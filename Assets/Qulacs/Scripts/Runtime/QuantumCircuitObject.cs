using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GateEntry
{
    public QuantumGateObject Gate;
    public int QubitIndex;
}

public class QuantumCircuitObject : MonoBehaviour
{
    public List<QbitObject> Qubits = new();
    public List<GateEntry> Gates = new();
}
