using UnityEngine;
using UnityEngine.InputSystem;

public class SideMoveAD : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    void Update()
    {
        float x = 0f;

        var kb = Keyboard.current;
        if (kb == null) return; 

        if (kb.aKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed) x += 1f;

        transform.position += new Vector3(x, 0f, 0f) * speed * Time.deltaTime;
    }
}