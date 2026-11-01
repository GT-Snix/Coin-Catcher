// A coin: catching it adds 1 to the score.
public class Coin : FallingObject
{
    protected override void OnCaught()
    {
        GameManager.AddScore(1);
    }
}
