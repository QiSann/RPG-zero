using UnityEngine;

public class MageProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private LayerMask whatIsPlayer;

    private float direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(float _direction)
    {
        direction = _direction;
        rb.linearVelocity = new Vector2(speed * direction, 0);

        // 朝向飞行方向
        if (direction > 0)
            transform.eulerAngles = new Vector3(0, 0, 0);
        else
            transform.eulerAngles = new Vector3(0, 180, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & whatIsPlayer) != 0)
        {
            PlayerStats player = collision.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy") && !collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
