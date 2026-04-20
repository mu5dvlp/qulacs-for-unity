using UnityEngine;
using Complex = System.Numerics.Complex;

public enum StateSetMode { Bloch, Components }

public class QuantumStateSetter : MonoBehaviour
{
    [SerializeField] QubitObject qubitObject;
    [SerializeField] StateSetMode mode;

    // Bloch sphere
    [SerializeField, Range(0f, 180f)] float thetaDeg = 0f;
    [SerializeField, Range(0f, 360f)] float phiDeg = 0f;

    // Components: |ψ⟩ = alpha|0⟩ + (betaReal + i*betaImag)|1⟩
    [SerializeField] float alpha = 1f;
    [SerializeField] float betaReal = 0f;
    [SerializeField] float betaImag = 0f;

    public void Apply()
    {
        if (qubitObject?.State == null) return;

        Complex a, b;

        if (mode == StateSetMode.Bloch)
        {
            float theta = thetaDeg * Mathf.Deg2Rad;
            float phi   = phiDeg   * Mathf.Deg2Rad;
            float halfTheta = theta * 0.5f;
            a = new Complex(Mathf.Cos(halfTheta), 0);
            b = new Complex(Mathf.Sin(halfTheta) * Mathf.Cos(phi),
                            Mathf.Sin(halfTheta) * Mathf.Sin(phi));
        }
        else
        {
            a = new Complex(alpha, 0);
            b = new Complex(betaReal, betaImag);
        }

        qubitObject.State.SetStateVector(new Complex[] { a, b });
    }
}
