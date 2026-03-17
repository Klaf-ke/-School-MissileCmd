using UnityEngine;

public class UFOHover : MonoBehaviour
{
    [Header("Hover Movement")]
    [SerializeField] private float distance = 4f;   
    [SerializeField] private float speed = 2f;      

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * distance;

        transform.position = startPosition + transform.forward * offset;
    }
}