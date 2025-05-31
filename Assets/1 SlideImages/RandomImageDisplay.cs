using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomImageDisplay : MonoBehaviour
{
    public string resourcesFolder = "ImagesLandscape"; // Relative to Assets/Resources
    public Renderer planeRenderer;
    private List<Texture2D> textures = new List<Texture2D>();
    private float changeInterval = 0.3f;

    void Start()
    {
        // Load all images from Resources/8SlideImages
        Object[] loadedTextures = Resources.LoadAll(resourcesFolder, typeof(Texture2D));
        foreach (Object obj in loadedTextures)
        {
            textures.Add(obj as Texture2D);
        }

        // Start the coroutine to change images
        if (textures.Count > 0)
        {
            StartCoroutine(ChangeImage());
        }
        else
        {
            Debug.LogError("No textures found in Resources/" + resourcesFolder);
        }
    }

    IEnumerator ChangeImage()
    {
        while (true)
        {
            // Select a random texture from the list
            Texture2D randomTexture = textures[Random.Range(0, textures.Count)];
            planeRenderer.material.mainTexture = randomTexture;

            // Wait for the specified interval before changing again
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
