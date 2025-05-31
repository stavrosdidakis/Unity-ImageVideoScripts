using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialImageDisplay : MonoBehaviour
{
    public string resourcesFolder = "ImagesSquare";  // Relative to Assets/Resources
    public Renderer planeRenderer;
    private List<Texture2D> textures = new List<Texture2D>();
    private float changeInterval = 0.3f;
    private int currentIndex = 0;

    void Start()
    {
        // Load all images from Resources/ImagesSquare
        Object[] loadedTextures = Resources.LoadAll(resourcesFolder, typeof(Texture2D));
        foreach (Object obj in loadedTextures)
        {
            textures.Add(obj as Texture2D);
        }

        // Start the coroutine to change images
        if (textures.Count > 0)
        {
            StartCoroutine(ChangeImageSequentially());
        }
        else
        {
            Debug.LogError("No textures found in Resources/" + resourcesFolder);
        }
    }

    IEnumerator ChangeImageSequentially()
    {
        while (true)
        {
            // Set the texture to the current index
            planeRenderer.material.mainTexture = textures[currentIndex];

            // Move to the next index and loop back to the start if necessary
            currentIndex = (currentIndex + 1) % textures.Count;

            // Wait for the specified interval before changing again
            yield return new WaitForSeconds(changeInterval);
        }
    }
}
