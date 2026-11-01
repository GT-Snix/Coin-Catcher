// A bomb: touching it costs one heart.
public class Bomb : FallingObject
{
    protected override void OnCaught()
    {
        GameManager.LoseLife();
    }
}
