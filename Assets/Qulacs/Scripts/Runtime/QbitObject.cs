using System.Numerics;
using Mu5dvlp.Qulacs;
using UnityEngine;

/// <summary>
/// Inspector-configurable representation of a single qubit state.
///
///   |ψ⟩ = α|0⟩ + β|1⟩
///
/// Magnitudes are coupled via a single angle θ so that |α|²+|β|²=1 always holds:
///   |α| = cos(θ),  |β| = sin(θ)
///
/// Sign and phase (×i) are set independently for each amplitude.
///
/// θ = 0°  →  |0⟩
/// θ = 90° →  |1⟩
/// θ = 45° →  equal superposition
/// </summary>
public class QbitObject : MonoBehaviour
{
    [Header("|ψ> = α|0> + β|1>")]
    [Tooltip("θ (degrees): |α| = cos θ, |β| = sin θ")]
    [Range(0f, 90f)] public float thetaDeg = 0f;

    [Header("α — amplitude of |0>")]
    [Tooltip("Negate: multiply by −1")] public bool alphaNegative;
    [Tooltip("Imaginary: multiply by i")] public bool alphaImaginary;

    [Header("β — amplitude of |1>")]
    [Tooltip("Negate: multiply by −1")] public bool betaNegative;
    [Tooltip("Imaginary: multiply by i")] public bool betaImaginary;

    // ----- read-only properties (updated by Refresh) -----

    /// <summary>Current amplitude of |0> as read from the native state.</summary>
    public Complex Alpha { get; private set; }

    /// <summary>Current amplitude of |1> as read from the native state.</summary>
    public Complex Beta { get; private set; }

    /// <summary>Probability of measuring |0>.</summary>
    public double ZeroProbability { get; private set; }

    /// <summary>Probability of measuring |1>.</summary>
    public double OneProbability => 1.0 - ZeroProbability;

    /// <summary>Squared norm (≈ 1 for a normalised state).</summary>
    public double SquaredNorm { get; private set; }

    // ----- native state -----

    private QuantumState _state;

    // ------------------------------------------------------------------

    void OnEnable()
    {
        _state = new QuantumState(1);
        Apply();
    }

    void OnDisable()
    {
        _state?.Dispose();
        _state = null;
    }

    // ------------------------------------------------------------------

    /// <summary>
    /// Pushes the configured amplitudes to the native <see cref="QuantumState"/>,
    /// then refreshes the read-only properties.
    /// </summary>
    public void Apply()
    {
        if (_state == null) return;

        double theta = thetaDeg * Mathf.Deg2Rad;
        double aMag = System.Math.Cos(theta);
        double bMag = System.Math.Sin(theta);

        if (alphaNegative) aMag = -aMag;
        if (betaNegative) bMag = -bMag;

        Complex a = alphaImaginary ? new Complex(0.0, aMag) : new Complex(aMag, 0.0);
        Complex b = betaImaginary ? new Complex(0.0, bMag) : new Complex(bMag, 0.0);

        _state.SetStateVector(new[] { a, b });
        Refresh();
    }

    /// <summary>
    /// Reads the current native state and updates the read-only properties.
    /// Call this after applying gates externally via <see cref="State"/>.
    /// </summary>
    public void Refresh()
    {
        if (_state == null) return;
        var v = _state.GetStateVector();
        Alpha = v[0];
        Beta = v[1];
        ZeroProbability = _state.GetZeroProbability(0);
        SquaredNorm = _state.GetSquaredNorm();
    }

    /// <summary>
    /// Direct access to the underlying <see cref="QuantumState"/> for gate application.
    /// Call <see cref="Refresh"/> afterwards to update the read-only properties.
    /// </summary>
    public QuantumState State => _state;
}
