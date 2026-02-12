using UnityEngine;
using System.Collections;

public class TurretCameraSwitcher : MonoBehaviour
{
    public TurretShooter[] turretShooters;
    public Camera mainCamera;
    public Transform[] cameraTargets;
    public TurretMouseLook[] turrets;

    public float transitionSpeed = 5f;
    public float followSpeed = 8f;

    private int currentIndex = 0;
    private Coroutine transitionRoutine;
    private Transform currentFollowTarget;

    [SerializeField] private TurretSelectorUI turretSelectorUI;

    void Start()
    {
        ActivateTurret(0);

        mainCamera.transform.position = cameraTargets[0].position;
        mainCamera.transform.rotation = cameraTargets[0].rotation;

        currentFollowTarget = cameraTargets[0];
            if (turretSelectorUI != null)
        turretSelectorUI.UpdateSelection(currentIndex, cameraTargets.Length);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SwitchCamera(-1);

        if (Input.GetKeyDown(KeyCode.E))
            SwitchCamera(1);

    
            for (int i = 0; i < cameraTargets.Length; i++)
            {
            if (Input.GetKeyDown((i + 1).ToString()))
                {
                    SwitchToIndex(i);
                }
            }

    FollowTarget();
}

    void SwitchCamera(int direction)
    {
        currentIndex += direction;

        if (currentIndex < 0) currentIndex = cameraTargets.Length - 1;
        if (currentIndex >= cameraTargets.Length) currentIndex = 0;

        ActivateTurret(currentIndex);
        if (turretSelectorUI != null)
        turretSelectorUI.UpdateSelection(currentIndex, cameraTargets.Length);

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(SmoothTransition(cameraTargets[currentIndex]));
    }

    void SwitchToIndex(int index)
    {
        if (index < 0 || index >= cameraTargets.Length)
            return;

        currentIndex = index;

        ActivateTurret(currentIndex);
        if (turretSelectorUI != null)
        turretSelectorUI.UpdateSelection(currentIndex, cameraTargets.Length);

        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(SmoothTransition(cameraTargets[currentIndex]));
    }








    void ActivateTurret(int index)
    {
        for (int i = 0; i < turrets.Length; i++)
            turrets[i].isActive = (i == index);

        for (int i = 0; i < turretShooters.Length; i++)
            turretShooters[i].isActive = (i == index);
    }

    IEnumerator SmoothTransition(Transform target)
    {
        float t = 0f;
        Transform cam = mainCamera.transform;

        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            cam.position = Vector3.Lerp(startPos, target.position, t);
            cam.rotation = Quaternion.Slerp(startRot, target.rotation, t);
            yield return null;
        }

        currentFollowTarget = target;
    }

    void FollowTarget()
    {
        if (currentFollowTarget == null) return;

        Transform cam = mainCamera.transform;

        cam.position = Vector3.Lerp(
            cam.position,
            currentFollowTarget.position,
            followSpeed * Time.deltaTime
        );

        cam.rotation = Quaternion.Slerp(
            cam.rotation,
            currentFollowTarget.rotation,
            followSpeed * Time.deltaTime
        );
    }
}