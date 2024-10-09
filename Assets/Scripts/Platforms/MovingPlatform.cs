using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovingPlatform : Platform
{
    [SerializeField, Min(0f)] private float _speed = 1f;

    private BoxCollider2D _collider;
    private float _colliderWidth; 
    private float _screenBoundPositionX;
    private int _direction;

    protected override void Awake()
    {
        base.Awake();
        _collider = GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();
        _colliderWidth = (_collider.size * transform.lossyScale).x / 2;
        _direction = Random.Range(0, 2) == 0 ? -1 : 1;
        _screenBoundPositionX = Camera.main.orthographicSize * Screen.width / Screen.height;
    }

    private void Update()
    {
        transform.Translate(new Vector2(_speed * _direction * Time.deltaTime, 0f));
        float positionX = transform.position.x;

        if (Mathf.Abs(positionX) + _colliderWidth > _screenBoundPositionX)
        {
            _direction = -_direction;
            transform.Translate(new Vector2(_speed * _direction * Time.deltaTime, 0f));
        }
    }
}
