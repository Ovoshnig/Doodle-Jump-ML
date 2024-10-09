using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovingPlatform : Platform
{
    [SerializeField, Min(0f)] private float _speed = 1f;

    private BoxCollider2D _collider;
    private Camera _camera;
    private float _colliderWidth; 
    private int _direction;

    protected override void Awake()
    {
        base.Awake();
        _collider = GetComponent<BoxCollider2D>();
        _camera = Camera.main;
    }

    protected override void Start()
    {
        base.Start();
        _colliderWidth = _collider.bounds.extents.x;
        _direction = Random.Range(0, 2) == 0 ? -1 : 1;
    }

    private void Update()
    {
        Vector2 position = transform.position;
        position.x += _direction * _colliderWidth;
        Vector2 viewportPosition = _camera.WorldToViewportPoint(position);

        if (viewportPosition.x < 0f || viewportPosition.x > 1f)
            _direction = -_direction;

        transform.Translate(new Vector2(_speed * _direction * Time.deltaTime, 0f));
    }
}
