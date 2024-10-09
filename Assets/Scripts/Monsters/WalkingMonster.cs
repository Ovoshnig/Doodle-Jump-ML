using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent (typeof(CapsuleCollider2D))]
public class WalkingMonster : Monster
{
    [SerializeField, Min(0f)] private float _speed = 1f;

    private SpriteRenderer _spriteRenderer;
    private CapsuleCollider2D _collider;
    private Camera _camera;
    private float _colliderWidth;
    private int _direction;

    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CapsuleCollider2D>();
        _camera = Camera.main;
    }

    protected override void Start()
    {
        base.Start();
        _colliderWidth = _collider.bounds.extents.x;
        _direction = Random.Range(0, 2) == 0 ? -1 : 1;
        _spriteRenderer.flipX = _direction == -1;
    }

    protected override void Update()
    {
        Vector2 position = transform.position;
        position.x += _direction * _colliderWidth;
        Vector2 viewportPosition = _camera.WorldToViewportPoint(position);

        if (viewportPosition.x < 0f || viewportPosition.x > 1f)
        {
            _direction = -_direction;
            _spriteRenderer.flipX = _direction == -1;
        }

        transform.Translate(new Vector2(_speed * _direction * Time.deltaTime, 0f));
    }
}
