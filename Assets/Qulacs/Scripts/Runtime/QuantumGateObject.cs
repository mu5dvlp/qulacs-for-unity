using UnityEngine;

public enum GateType { X, Y, Z, H, CNOT }

public class QuantumGateObject : MonoBehaviour
{
    [SerializeField] private GateType m_gateType;
    [SerializeField] private GameObject wire;

    public GateType GateType
    {
        get => m_gateType;
        set { m_gateType = value; ApplyColor(); }
    }

    private Renderer _renderer;

    void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start() => ApplyColor();

    void ApplyColor()
    {
        if (_renderer == null) return;
        _renderer.material.color = m_gateType switch
        {
            GateType.X => Color.red,
            GateType.Y => Color.green,
            GateType.Z => Color.blue,
            GateType.H => Color.yellow,
            GateType.CNOT => Color.red,
            _ => Color.white,
        };
    }

    // signedHeight > 0: extends toward +y (control above target), < 0: toward -y, 0: hidden
    public void SetwireHeight(float signedHeight)
    {
        if (wire == null) return;
        var t = wire.transform;
        t.localScale = new Vector3(t.localScale.x, Mathf.Abs(signedHeight), t.localScale.z);
        t.localPosition = new Vector3(0, signedHeight * 0.5f, 0);
    }
}
