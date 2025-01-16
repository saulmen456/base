using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RayTest : MonoBehaviour
{
    private Camera _camera = null;
    [SerializeField] private Material DeadMaterial = null;

    private void Awake() => _camera = Camera.main;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject == gameObject)
                {
                    GetComponent<Renderer>().material = DeadMaterial;
                }
            }
        }
    }
}