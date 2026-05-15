using UnityEngine;
using UnityEngine.Rendering;

public class BlochSphereObject : MonoBehaviour
{
    enum ShaderType
    {
        URP,
        BuiltIn,
    }

    [SerializeField]
    QubitObject qubitObject;

    [SerializeField]
    Renderer sphereRenderer;

    [SerializeField]
    Transform arrow;

    [SerializeField]
    ShaderType shaderType = ShaderType.URP;

    void Start()
    {
        if (sphereRenderer == null)
            return;
        var mat = sphereRenderer.material;

        if (shaderType == ShaderType.URP)
        {
            mat.SetFloat("_Surface", 1);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
        else
        {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        mat.renderQueue = (int)RenderQueue.Transparent;
    }

    void Update()
    {
        UpdateArrowDirection();
        UpdateSphereColor();
    }

    void UpdateArrowDirection()
    {
        if (qubitObject?.State == null || arrow == null)
            return;

        var v = qubitObject.State.GetStateVector();
        float p0 = (float)qubitObject.State.GetZeroProbability(0);

        float theta = 2f * Mathf.Acos(Mathf.Sqrt(Mathf.Clamp01(p0)));
        float phi = Mathf.Atan2((float)v[1].Imaginary, (float)v[1].Real);

        float sinTheta = Mathf.Sin(theta);
        float blochX = sinTheta * Mathf.Cos(phi);
        float blochY = sinTheta * Mathf.Sin(phi);
        float blochZ = Mathf.Cos(theta);

        // Bloch Z → Unity Y (up), Bloch X → Unity X, Bloch Y → Unity Z
        arrow.up = new Vector3(blochX, blochZ, blochY);
    }

    void UpdateSphereColor()
    {
        if (qubitObject?.State == null || sphereRenderer == null)
            return;

        var v = qubitObject.State.GetStateVector();
        var beta = v[1];
        float oneProbability = (float)(1.0 - qubitObject.State.GetZeroProbability(0));
        float phase = Mathf.Atan2((float)beta.Imaginary, (float)beta.Real);
        float hue = (phase / (2f * Mathf.PI) + 1f) % 1f;

        var color = Color.HSVToRGB(hue, oneProbability, 1f);
        color.a = 0.3f;
        sphereRenderer.material.color = color;
    }
}
