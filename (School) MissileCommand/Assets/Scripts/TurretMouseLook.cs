using UnityEngine;

public class TurretMouseLook : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotateSpeed = 120f;
    public float minYaw = -90f;
    public float maxYaw = 90f;

    [Header("Vertical Movement Settings")]
    public float moveSpeed = 5f;
    public float minHeight = 0f;
    public float maxHeight = 5f;

    [HideInInspector] public bool isActive = false;

    private float yaw;
    private float startYaw;

    private float startHeight;

    void Start()
    {
        startYaw = transform.eulerAngles.y;
        yaw = startYaw;

        startHeight = transform.position.y;
    }

    void Update()
    {
        if (isActive)
        {
            HandleInput();
        }
        else
        {
            ReturnToStart();
        }
    }

    void HandleInput()
    {
        
        float horizontal = Input.GetAxis("Horizontal");
        yaw += horizontal * rotateSpeed * Time.deltaTime;
        yaw = Mathf.Clamp(yaw, startYaw + minYaw, startYaw + maxYaw);

        
        float vertical = 0f;
        if (Input.GetKey(KeyCode.W))
            vertical = 1f;
        if (Input.GetKey(KeyCode.S))
            vertical = -1f;

        Vector3 pos = transform.position;
        pos.y += vertical * moveSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, startHeight + minHeight, startHeight + maxHeight);

        
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void ReturnToStart()
    {
        
        yaw = Mathf.Lerp(yaw, startYaw, 5f * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, startHeight, 5f * Time.deltaTime);

        transform.position = pos;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}