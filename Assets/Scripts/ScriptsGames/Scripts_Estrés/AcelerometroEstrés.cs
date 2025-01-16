using UnityEngine;

public class AcelerometroEstrés : MonoBehaviour
{
    public float speed = 10f;
    private Camera mainCamera;
    private bool facingRight = true;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        float moveInput = Input.acceleration.x;
        Vector3 movement = new Vector3(moveInput, 0, 0) * speed * Time.deltaTime;
        transform.Translate(movement);

        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        Vector3 pos = transform.position;
        float halfWidth = GetObjectHalfWidth();
        float screenLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).x + halfWidth;
        float screenRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x - halfWidth;

        pos.x = Mathf.Clamp(pos.x, screenLeft, screenRight);
        transform.position = pos;
    }

    private void Flip()
    {
        facingRight = !facingRight; 
        Vector3 scale = transform.localScale;
        scale.x *= -1; 
        transform.localScale = scale;
    }

    private float GetObjectHalfWidth()
    {
        return GetComponent<Renderer>().bounds.extents.x;
    }
}
