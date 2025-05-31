using UnityEngine;

public class ImageSplitter : MonoBehaviour
{
    [Header("Material and Image Folder")]
    public Material regionMaterial;     // Material with shader
    public string resourceFolder = "ImagesLandscape"; // Set this in the Inspector

    private Texture2D[] images;         // Loaded images
    private float noiseOffset;

    void Start()
    {
        noiseOffset = Random.value * 100f;

        // Load all images from the specified Resources folder
        images = Resources.LoadAll<Texture2D>(resourceFolder);

        if (images.Length == 0)
        {
            Debug.LogError($"No images found in Resources/{resourceFolder}/");
            return;
        }

        // Randomize images initially
        LoadRandomImages();
    }

    void Update()
    {
        // Animate rotation with Perlin noise
        float noise = Mathf.PerlinNoise(Time.time * 0.2f, noiseOffset);
        float angle = Mathf.Lerp(-60f, 60f, noise);
        regionMaterial.SetFloat("_Angle", angle);

        // Space bar to randomize images
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadRandomImages();
        }
    }

    void LoadRandomImages()
    {
        if (images.Length < 3)
        {
            Debug.LogWarning("Not enough images to load 3 random textures.");
            return;
        }

        // Choose 3 random images (could be duplicates if there are less than 3 images)
        Texture2D texA = images[Random.Range(0, images.Length)];
        Texture2D texB = images[Random.Range(0, images.Length)];
        Texture2D texC = images[Random.Range(0, images.Length)];

        // Assign them to the shader
        regionMaterial.SetTexture("_ImageA", texA);
        regionMaterial.SetTexture("_ImageB", texB);
        regionMaterial.SetTexture("_ImageC", texC);
    }
}
