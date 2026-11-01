using UnityEngine;

// Spawns yellow coins and dark bombs at random X positions above the top of the screen.
// Everything falls straight down toward the catcher.
// If no CoinSpawner exists in the scene, one is created automatically when Play starts.
public class CoinSpawner : MonoBehaviour
{
    [Header("Timing (seconds, picked randomly between min and max)")]
    public float minSpawnDelay = 0.6f;
    public float maxSpawnDelay = 1.8f;

    [Header("Fall speed (picked randomly between min and max)")]
    public float minFallSpeed = 3f;
    public float maxFallSpeed = 6f;

    [Header("Bombs")]
    [Range(0f, 1f)] public float bombChance = 0.25f;
    public float bombSize = 0.7f;

    [Header("Coin look")]
    public float coinSize = 0.6f;
    public Color coinColor = new Color(1f, 0.85f, 0.1f, 1f);

    [Header("Horizontal spawn range")]
    public float edgeMargin = 0.5f;

    Sprite coinSprite;
    Sprite bombSprite;
    Camera cam;
    float nextSpawnTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (FindAnyObjectByType<CoinSpawner>() == null)
        {
            new GameObject("Coin Spawner").AddComponent<CoinSpawner>();
        }
    }

    void Start()
    {
        cam = Camera.main;
        coinSprite = CreateCircleSprite(64, Color.white, Color.white);
        bombSprite = CreateCircleSprite(64, new Color(0.12f, 0.12f, 0.14f), new Color(0.9f, 0.15f, 0.1f));
        ScheduleNext();
    }

    void Update()
    {
        if (GameManager.IsGameOver) return;

        if (Time.time >= nextSpawnTime)
        {
            if (Random.value < bombChance) Spawn<Bomb>("Bomb", bombSprite, Color.white, bombSize);
            else Spawn<Coin>("Coin", coinSprite, coinColor, coinSize);

            ScheduleNext();
        }
    }

    void ScheduleNext()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    void Spawn<T>(string name, Sprite sprite, Color tint, float size) where T : FallingObject
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 camPos = cam.transform.position;

        float x = Random.Range(camPos.x - halfWidth + edgeMargin, camPos.x + halfWidth - edgeMargin);
        float y = camPos.y + halfHeight + size;

        GameObject obj = new GameObject(name);
        obj.transform.position = new Vector3(x, y, 0f);
        obj.transform.localScale = Vector3.one * size;

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = tint;
        sr.sortingOrder = 1; // draw in front of the platform

        T falling = obj.AddComponent<T>();
        falling.fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        falling.destroyY = camPos.y - halfHeight - size;
    }

    // Builds an anti-aliased circle sprite (1 world unit wide) with an outer ring,
    // so no image asset is needed. Coins use plain white (tinted yellow); bombs are dark with a red ring.
    static Sprite CreateCircleSprite(int size, Color fill, Color ring)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float r = size / 2f;
        float ringStart = r * 0.78f;
        Color[] pixels = new Color[size * size];
        for (int py = 0; py < size; py++)
        {
            for (int px = 0; px < size; px++)
            {
                float dx = px + 0.5f - r;
                float dy = py + 0.5f - r;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                Color c = Color.Lerp(fill, ring, Mathf.Clamp01(dist - ringStart));
                c.a = Mathf.Clamp01(r - dist);
                pixels[py * size + px] = c;
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
