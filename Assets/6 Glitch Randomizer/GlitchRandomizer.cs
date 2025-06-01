using UnityEngine;

public class GlitchRandomizer : MonoBehaviour
{
    [Header("Target Settings")]
    public Material glitchMaterial;

    [Header("Chromatic Aberration")]
    public Vector2 chromAberrXRange = new Vector2(0f, 0.02f);
    public Vector2 chromAberrYRange = new Vector2(0f, 0.02f);

    [Header("Right Stripes")]
    public Vector2 rightStripesAmountRange = new Vector2(1f, 10f);
    public Vector2 rightStripesFillRange = new Vector2(0f, 1f);

    [Header("Left Stripes")]
    public Vector2 leftStripesAmountRange = new Vector2(1f, 10f);
    public Vector2 leftStripesFillRange = new Vector2(0f, 1f);

    [Header("Displacement")]
    public Vector2 displacementXYRange = new Vector2(0f, 0.02f);
    public Vector2 wavyDisplZWRange = new Vector2(0f, 0.02f);
    public Vector2 wavyDisplFreqRange = new Vector2(5f, 30f);

    [Header("Smooth Transition")]
    public float lerpSpeed = 1.0f; // Higher = faster transitions
    public float changeInterval = 2.0f; // How often to pick new random values

    private float timer;

    // Current & target values for smooth interpolation
    private float chromX_Current, chromX_Target;
    private float chromY_Current, chromY_Target;
    private float rightStripesAmount_Current, rightStripesAmount_Target;
    private float rightStripesFill_Current, rightStripesFill_Target;
    private float leftStripesAmount_Current, leftStripesAmount_Target;
    private float leftStripesFill_Current, leftStripesFill_Target;
    private Vector4 displacement_Current, displacement_Target;
    private float wavyFreq_Current, wavyFreq_Target;

    private void Start()
    {
        if (glitchMaterial == null)
        {
            Debug.LogError("GlitchRandomizer: Please assign a Glitch Material!");
        }

        // Initialize current and target values
        PickNewTargets(true);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            PickNewTargets(false);
            timer = 0f;
        }

        // Smoothly interpolate
        chromX_Current = Mathf.Lerp(chromX_Current, chromX_Target, lerpSpeed * Time.deltaTime);
        chromY_Current = Mathf.Lerp(chromY_Current, chromY_Target, lerpSpeed * Time.deltaTime);
        rightStripesAmount_Current = Mathf.Lerp(rightStripesAmount_Current, rightStripesAmount_Target, lerpSpeed * Time.deltaTime);
        rightStripesFill_Current = Mathf.Lerp(rightStripesFill_Current, rightStripesFill_Target, lerpSpeed * Time.deltaTime);
        leftStripesAmount_Current = Mathf.Lerp(leftStripesAmount_Current, leftStripesAmount_Target, lerpSpeed * Time.deltaTime);
        leftStripesFill_Current = Mathf.Lerp(leftStripesFill_Current, leftStripesFill_Target, lerpSpeed * Time.deltaTime);
        displacement_Current = Vector4.Lerp(displacement_Current, displacement_Target, lerpSpeed * Time.deltaTime);
        wavyFreq_Current = Mathf.Lerp(wavyFreq_Current, wavyFreq_Target, lerpSpeed * Time.deltaTime);

        // Apply to material
        if (glitchMaterial)
        {
            glitchMaterial.SetFloat("_ChromAberrAmountX", chromX_Current);
            glitchMaterial.SetFloat("_ChromAberrAmountY", chromY_Current);
            glitchMaterial.SetFloat("_RightStripesAmount", rightStripesAmount_Current);
            glitchMaterial.SetFloat("_RightStripesFill", rightStripesFill_Current);
            glitchMaterial.SetFloat("_LeftStripesAmount", leftStripesAmount_Current);
            glitchMaterial.SetFloat("_LeftStripesFill", leftStripesFill_Current);
            glitchMaterial.SetVector("_DisplacementAmount", displacement_Current);
            glitchMaterial.SetFloat("_WavyDisplFreq", wavyFreq_Current);
        }
    }

    private void PickNewTargets(bool initial)
    {
        chromX_Target = Random.Range(chromAberrXRange.x, chromAberrXRange.y);
        chromY_Target = Random.Range(chromAberrYRange.x, chromAberrYRange.y);
        rightStripesAmount_Target = Random.Range(rightStripesAmountRange.x, rightStripesAmountRange.y);
        rightStripesFill_Target = Random.Range(rightStripesFillRange.x, rightStripesFillRange.y);
        leftStripesAmount_Target = Random.Range(leftStripesAmountRange.x, leftStripesAmountRange.y);
        leftStripesFill_Target = Random.Range(leftStripesFillRange.x, leftStripesFillRange.y);

        Vector4 newDisplacement = new Vector4(
            Random.Range(displacementXYRange.x, displacementXYRange.y),
            Random.Range(displacementXYRange.x, displacementXYRange.y),
            Random.Range(wavyDisplZWRange.x, wavyDisplZWRange.y),
            Random.Range(wavyDisplZWRange.x, wavyDisplZWRange.y)
        );

        displacement_Target = newDisplacement;
        wavyFreq_Target = Random.Range(wavyDisplFreqRange.x, wavyDisplFreqRange.y);

        // If first time, initialize current = target
        if (initial)
        {
            chromX_Current = chromX_Target;
            chromY_Current = chromY_Target;
            rightStripesAmount_Current = rightStripesAmount_Target;
            rightStripesFill_Current = rightStripesFill_Target;
            leftStripesAmount_Current = leftStripesAmount_Target;
            leftStripesFill_Current = leftStripesFill_Target;
            displacement_Current = displacement_Target;
            wavyFreq_Current = wavyFreq_Target;
        }
    }
}
