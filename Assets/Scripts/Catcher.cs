using UnityEngine;

public class Catcher : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 10f;
    public float limit = 6f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsGameOver) return;

        float input = Input.GetAxisRaw("Horizontal");
        transform.Translate(speed*input*Vector3.right*Time.deltaTime);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -limit,limit);
        transform.position = pos;
    }
}
