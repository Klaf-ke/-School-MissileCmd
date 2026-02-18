using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Flying Movement")]
    [SerializeField] private float hoverCenterHeight = 4f;
    [SerializeField] private float hoverAmplitude = 1f;
    [SerializeField] private float hoverSpeed = 2f;

    private float hoverOffset;

    protected override void Awake()
    {
        base.Awake();
        hoverOffset = Random.Range(0f, Mathf.PI * 2f);
    }

   protected override void Move()
    {
        if (targetBunker == null) return;

    
        Vector3 targetXZ = new Vector3(
            targetBunker.transform.position.x,
            transform.position.y,
            targetBunker.transform.position.z
        );

        Vector3 horizontalDir = (targetXZ - transform.position).normalized;

        transform.position += horizontalDir * speed * Time.deltaTime;

   
        float hoverY = hoverCenterHeight +
                    Mathf.Sin(Time.time * hoverSpeed + hoverOffset) * hoverAmplitude;

        Vector3 pos = transform.position;
        pos.y = hoverY;
        transform.position = pos;
    }
}