using System;
using Mu5dvlp.Qulacs;
using UnityEngine;

public class QubitObject : MonoBehaviour
{
    [SerializeField] bool useColor = false;
    public QuantumState State { get; private set; }

    private Renderer _renderer;

    void OnEnable()
    {
        State = new QuantumState(1);
        _renderer = GetComponent<Renderer>();
    }

    void OnDisable()
    {
        State?.Dispose();
        State = null;
    }

    void Update()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        if (State == null || _renderer == null) return;
        if (!useColor)
        {
            _renderer.material.color = Color.white;
            return;
        }

        var v = State.GetStateVector();
        var beta = v[1];
        float oneProbability = (float)(1.0 - State.GetZeroProbability(0));
        float phase = (float)Math.Atan2(beta.Imaginary, beta.Real);
        float hue = (phase / (2f * Mathf.PI) + 1f) % 1f;

        _renderer.material.color = Color.HSVToRGB(hue, oneProbability, 1f);
    }
}
