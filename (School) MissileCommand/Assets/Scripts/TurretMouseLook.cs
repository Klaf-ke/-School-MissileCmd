using UnityEngine;

public class TurretMouseLook : MonoBehaviour
{
    public float mouseSensitivity = 120f;
    public float minYaw = -180f;
    public float maxYaw = 180f;

    [HideInInspector] public bool isActive = false;

    private float yaw;

    void Start()
    {
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        if (!isActive) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);

        // ONLY Y axis rotation (no tilt, no roll)
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}