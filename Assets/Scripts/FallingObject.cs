using UnityEngine;

// Shared behaviour for anything that falls from the top (coins, bombs):
// moves straight down, reacts when it touches the catcher, and is removed below the screen.
public abstract class FallingObject : MonoBehaviour
{
    public float fallSpeed = 4f;
    public float destroyY = -7f;

    SpriteRenderer selfRenderer;
    SpriteRenderer catcherRenderer;

    void Start()
    {
        selfRenderer = GetComponent<SpriteRenderer>();

        Catcher catcher = FindAnyObjectByType<Catcher>();
        if (catcher != null)
        {
            catcherRenderer = catcher.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (GameManager.IsGameOver) return;

        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        if (catcherRenderer != null && selfRenderer.bounds.Intersects(catcherRenderer.bounds))
        {
            OnCaught();
            Destroy(gameObject);
            return;
        }

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    // Called once when this object touches the catcher.
    protected abstract void OnCaught();
}
