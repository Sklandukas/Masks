using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class SideMoveArrows : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float x = 0f;

        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.leftArrowKey.isPressed)  x -= 1f;
            if (kb.rightArrowKey.isPressed) x += 1f;
        }

        Vector2 move = new Vector2(x * speed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(rb.position + move);
    }
}
