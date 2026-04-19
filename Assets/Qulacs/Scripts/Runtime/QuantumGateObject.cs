using UnityEngine;

public enum GateType { X, Y, Z, H }

public class QuantumGateObject : MonoBehaviour
{
    [SerializeField] private GateType m_gateType;
    public GateType GateType => m_gateType;

    private Renderer _renderer;

    void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        if (_renderer == null) return;
        switch (m_gateType)
        {
            case GateType.X:
                _renderer.material.color = Color.red;
                break;
            case GateType.Y:
                _renderer.material.color = Color.green;
                break;
            case GateType.Z:
                _renderer.material.color = Color.blue;
                break;
            case GateType.H:
                _renderer.material.color = Color.yellow;
                break;
        }
    }
}
