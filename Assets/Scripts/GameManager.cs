using UnityEngine;

// Holds score and lives, draws the HUD (coins + hearts in the top-left),
// shows the Game Over screen and restarts the game when Space is pressed.
// Created automatically when Play starts, so nothing needs to be added to the scene.
public class GameManager : MonoBehaviour
{
    public const int MaxLives = 3;

    public static int Score { get; private set; }
    public static int Lives { get; private set; }
    public static bool IsGameOver { get; private set; }

    static GameManager instance;

    float hitFlashUntil;
    GUIStyle hudStyle, titleStyle, textStyle;
    Texture2D heartTexture;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (FindAnyObjectByType<GameManager>() == null)
        {
            new GameObject("Game Manager").AddComponent<GameManager>();
        }
    }

    void Awake()
    {
        instance = this;
        ResetState();
        heartTexture = CreateHeartTexture(64);
    }

    void Update()
    {
        if (IsGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
    }

    public static void AddScore(int amount)
    {
        if (IsGameOver) return;
        Score += amount;
    }

    public static void LoseLife()
    {
        if (IsGameOver) return;

        Lives--;
        if (instance != null) instance.hitFlashUntil = Time.time + 0.2f;

        if (Lives <= 0)
        {
            Lives = 0;
            IsGameOver = true;
        }
    }

    static void ResetState()
    {
        Score = 0;
        Lives = MaxLives;
        IsGameOver = false;
    }

    void Restart()
    {
        foreach (FallingObject obj in FindObjectsByType<FallingObject>(FindObjectsSortMode.None))
        {
            Destroy(obj.gameObject);
        }
        ResetState();
    }

    // ---------- HUD ----------

    void OnGUI()
    {
        if (hudStyle == null) BuildStyles();

        int fontSize = hudStyle.fontSize;

        // Coins counter
        GUI.Label(new Rect(20, 15, 400, fontSize * 1.5f), "Coins: " + Score, hudStyle);

        // Hearts (lost hearts are drawn faded)
        float heartSize = fontSize * 1.1f;
        float heartsY = 15 + fontSize * 1.5f;
        for (int i = 0; i < MaxLives; i++)
        {
            GUI.color = i < Lives ? Color.white : new Color(1f, 1f, 1f, 0.2f);
            GUI.DrawTexture(new Rect(20 + i * (heartSize + 8), heartsY, heartSize, heartSize), heartTexture);
        }
        GUI.color = Color.white;

        // Short red flash when a bomb hits
        if (Time.time < hitFlashUntil)
        {
            DrawOverlay(new Color(1f, 0f, 0f, 0.3f));
        }

        // Game Over screen
        if (IsGameOver)
        {
            DrawOverlay(new Color(0f, 0f, 0f, 0.75f));

            float w = Screen.width, h = Screen.height;
            GUI.Label(new Rect(0, h * 0.30f, w, h * 0.15f), "GAME OVER", titleStyle);
            GUI.Label(new Rect(0, h * 0.47f, w, h * 0.08f), "Score: " + Score, textStyle);
            GUI.Label(new Rect(0, h * 0.58f, w, h * 0.08f), "Press SPACE to restart", textStyle);
        }
    }

    void BuildStyles()
    {
        hudStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(Screen.height * 0.05f),
            fontStyle = FontStyle.Bold
        };
        hudStyle.normal.textColor = Color.white;

        titleStyle = new GUIStyle(hudStyle)
        {
            fontSize = Mathf.RoundToInt(Screen.height * 0.12f),
            alignment = TextAnchor.MiddleCenter
        };
        titleStyle.normal.textColor = new Color(1f, 0.3f, 0.25f);

        textStyle = new GUIStyle(hudStyle) { alignment = TextAnchor.MiddleCenter };
    }

    static void DrawOverlay(Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    // Draws a red heart using the classic heart curve, with 4x4 supersampling for smooth edges.
    static Texture2D CreateHeartTexture(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color red = new Color(0.9f, 0.1f, 0.2f);
        const int samples = 4;
        Color[] pixels = new Color[size * size];

        for (int py = 0; py < size; py++)
        {
            for (int px = 0; px < size; px++)
            {
                int inside = 0;
                for (int sy = 0; sy < samples; sy++)
                {
                    for (int sx = 0; sx < samples; sx++)
                    {
                        float x = ((px + (sx + 0.5f) / samples) / size) * 2.6f - 1.3f;
                        float y = ((py + (sy + 0.5f) / samples) / size) * 2.5f - 1.2f;
                        float a = x * x + y * y - 1f;
                        if (a * a * a - x * x * y * y * y <= 0f) inside++;
                    }
                }
                red.a = inside / (float)(samples * samples);
                pixels[py * size + px] = red;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
