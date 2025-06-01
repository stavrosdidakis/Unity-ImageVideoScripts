using UnityEngine;

public class TunnelActivator : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject targetObject;
    public Renderer targetRenderer; // The Renderer that has the tunnel material

    [Header("Tunnel Size Settings")]
    public float tunnelSizeMin = 0.2f;
    public float tunnelSizeMax = 0.5f;

    [Header("Tunnel Speed Settings")]
    public float tunnelSpeedMin = -1.0f;
    public float tunnelSpeedMax = 1.0f;

    [Header("Twist Frequency Settings")]
    public float twistFrequencyMin = 1.0f;
    public float twistFrequencyMax = 5.0f;

    [Header("Twist Speed Settings")]
    public float twistSpeedMin = 0.1f;
    public float twistSpeedMax = 2.0f;

    // Shader property IDs for efficiency
    private int tunnelSizeID;
    private int tunnelSpeedID;
    private int twistFrequencyID;
    private int twistSpeedID;

    private void Start()
    {
        // Cache the shader property IDs (must match exactly your shader property names)
        tunnelSizeID = Shader.PropertyToID("_TunnelSize");
        tunnelSpeedID = Shader.PropertyToID("_TunnelSpeed");
        twistFrequencyID = Shader.PropertyToID("_TwistFrequency");
        twistSpeedID = Shader.PropertyToID("_TwistSpeed");

        if (targetRenderer == null)
        {
            Debug.LogError("TunnelActivator: Target Renderer is not assigned!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (targetObject == null) return;

            bool isActive = targetObject.activeSelf;
            targetObject.SetActive(!isActive);

            if (!isActive)
            {
                RandomizeTunnelShader();
            }
        }
    }

    private void RandomizeTunnelShader()
    {
        if (targetRenderer == null || targetRenderer.material == null) return;

        float randomTunnelSize = Random.Range(tunnelSizeMin, tunnelSizeMax);
        float randomTunnelSpeed = Random.Range(tunnelSpeedMin, tunnelSpeedMax);
        float randomTwistFrequency = Random.Range(twistFrequencyMin, twistFrequencyMax);
        float randomTwistSpeed = Random.Range(twistSpeedMin, twistSpeedMax);

        targetRenderer.material.SetFloat(tunnelSizeID, randomTunnelSize);
        targetRenderer.material.SetFloat(tunnelSpeedID, randomTunnelSpeed);
        targetRenderer.material.SetFloat(twistFrequencyID, randomTwistFrequency);
        targetRenderer.material.SetFloat(twistSpeedID, randomTwistSpeed);
    }
}
