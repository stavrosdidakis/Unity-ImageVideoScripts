using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class GridManager169 : MonoBehaviour
{

    [Header("Folder")]
    public string resourcesFolderName = "Videos";

    [Header("Canvas & Timing")]
    public RectTransform canvasRect;   // assign in inspector
    public float period    = 2f;       // how often to regenerate

    [Header("Grid Settings")]
    public int maxGridSize = 10;
    public int screenWidth = 3840;
    public int screenHeight= 2160;


    List<VideoClip>   videoClips    = new List<VideoClip>();
    System.Random     rnd           = new System.Random();
    GameObject        currentParent;
    List<GameObject>  currentCells  = new List<GameObject>();
    float             nextActionTime;

    void Start()
    {
        if (canvasRect == null)
        {
            Debug.LogError("Canvas RectTransform not set!");
            enabled = false;
            return;
        }

        videoClips.AddRange(Resources.LoadAll<VideoClip>(resourcesFolderName));
        if (videoClips.Count == 0)
        {
            Debug.LogError("No VideoClips found!");
            enabled = false;
            return;
        }

        // initial container & 1×1 grid
        currentParent = new GameObject("Grid_Current", typeof(CanvasGroup), typeof(RectTransform));
        currentParent.transform.SetParent(canvasRect, false);
        currentParent.GetComponent<CanvasGroup>().alpha = 1f;
        BuildGrid(1, 1, currentParent.transform, currentCells, null);

        nextActionTime = Time.time + period;
    }

    void Update()
    {
        if (Time.time < nextActionTime) return;
        nextActionTime = Time.time + period;

        int gx = rnd.Next(1, maxGridSize + 1);
        int gy = rnd.Next(1, maxGridSize + 1);
        StartCoroutine(SwitchToNewGrid(gx, gy));
    }

    IEnumerator SwitchToNewGrid(int gridX, int gridY)
    {
        // new hidden container
        var nextParent = new GameObject("Grid_Next", typeof(CanvasGroup), typeof(RectTransform));
        nextParent.transform.SetParent(canvasRect, false);
        var nextCg = nextParent.GetComponent<CanvasGroup>();
        nextCg.alpha = 0f;

        // build & prepare
        var nextCells = new List<GameObject>();
        int totalToPrepare = gridX * gridY;
        int preparedCount  = 0;
        BuildGrid(gridX, gridY, nextParent.transform, nextCells, () => preparedCount++);

        // wait until all videos are buffered & playing
        while (preparedCount < totalToPrepare)
            yield return null;

        // swap visibility
        currentParent.GetComponent<CanvasGroup>().alpha = 0f;
        nextCg.alpha = 1f;

        // destroy old
        foreach (var c in currentCells) Destroy(c);
        Destroy(currentParent);

        // promote
        currentParent = nextParent;
        currentCells = nextCells;
    }

    void BuildGrid(int gridX, int gridY, Transform parent, List<GameObject> list, System.Action onPlayerPrepared)
    {
        float cellW = (float)screenWidth  / gridX;
        float cellH = (float)screenHeight / gridY;

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                // -- cell mask --
                var cell = new GameObject($"Cell_{x}_{y}", typeof(RectTransform), typeof(RectMask2D));
                cell.transform.SetParent(parent, false);
                var cr = cell.GetComponent<RectTransform>();
                cr.anchorMin = cr.anchorMax = cr.pivot = Vector2.zero;
                cr.anchoredPosition = new Vector2(x * cellW, y * cellH);
                cr.sizeDelta = new Vector2(cellW, cellH);

                // pick a random clip & its aspect
                var clip       = videoClips[rnd.Next(videoClips.Count)];
                float videoRatio = (float)clip.width / clip.height;
                float cellRatio  = cellW / cellH;

                // compute aspect-fill dimensions
                float rw, rh;
                if (cellRatio >= videoRatio)
                {
                    rw = cellW;
                    rh = rw / videoRatio;
                }
                else
                {
                    rh = cellH;
                    rw = rh * videoRatio;
                }

                // -- RawImage setup --
                var imgGO = new GameObject("VideoImage", typeof(RectTransform), typeof(RawImage));
                imgGO.transform.SetParent(cell.transform, false);
                var imgRT = imgGO.GetComponent<RectTransform>();
                imgRT.anchorMin = imgRT.anchorMax = new Vector2(0.5f, 0.5f);
                imgRT.pivot   = new Vector2(0.5f, 0.5f);
                imgRT.sizeDelta = new Vector2(rw, rh);
                imgRT.anchoredPosition = Vector2.zero;

                var raw = imgGO.GetComponent<RawImage>();

                // match RenderTexture to that size
                int rtW = Mathf.CeilToInt(rw);
                int rtH = Mathf.CeilToInt(rh);
                var rt = new RenderTexture(rtW, rtH, 0);
                rt.Create();
                raw.texture = rt;

                // -- VideoPlayer setup --
                var vp = cell.AddComponent<VideoPlayer>();
                vp.playOnAwake   = false;
                vp.isLooping     = true;
                vp.renderMode    = VideoRenderMode.RenderTexture;
                vp.targetTexture = rt;
                vp.clip          = clip;

                vp.Prepare();
                vp.prepareCompleted += (VideoPlayer src) =>
                {
                    src.time = rnd.NextDouble() * src.clip.length;
                    src.Play();
                    onPlayerPrepared?.Invoke();
                };

                list.Add(cell);
            }
        }
    }
}
