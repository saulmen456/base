using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    public float minSpeed = 1f;
    public float maxSpeed = 5f;
    public float minDirectionChangeInterval = 1f;
    public float maxDirectionChangeInterval = 3f;
    private float speed;
    private float directionChangeInterval;
    private float timer = 0f;
    private Vector2 direction = Vector2.right;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        SetRandomSpeedAndInterval();
        SetRandomDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= directionChangeInterval)
        {
            timer = 0f;
            SetRandomSpeedAndInterval();
            SetRandomDirection();
        }

        Vector3 movement = direction * speed * Time.deltaTime;
        transform.Translate(movement);

        Vector3 pos = transform.position;
        float halfWidth = GetObjectHalfWidth();
        float screenLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).x + halfWidth;
        float screenRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x - halfWidth;

        if (pos.x <= screenLeft || pos.x >= screenRight)
        {
            direction = -direction;
        }

        pos.x = Mathf.Clamp(pos.x, screenLeft, screenRight);
        transform.position = pos;
    }

    private void SetRandomSpeedAndInterval()
    {
        speed = Random.Range(minSpeed, maxSpeed); 
        directionChangeInterval = Random.Range(minDirectionChangeInterval, maxDirectionChangeInterval); 
    }

    private void SetRandomDirection()
    {
        direction = Random.value > 0.5f ? Vector2.right : Vector2.left;
    }

    private float GetObjectHalfWidth()
    {
        return GetComponent<Renderer>().bounds.extents.x;
    }
}
