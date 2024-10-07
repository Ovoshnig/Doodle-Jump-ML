using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovingPlatform : Platform
{
    [SerializeField, Min(0f)] private float _speed = 1f;

    private BoxCollider2D _collider;
    private float _colliderWidth; 
    private float screenBoundPositionX;
    private int _direction;

    private void Awake() => _collider = GetComponent<BoxCollider2D>();

    private void Start()
    {
        _colliderWidth = _collider.size.x / 2;
        _direction = Random.Range(0, 2) == 0 ? -1 : 1;
        screenBoundPositionX = Camera.main.orthographicSize * Screen.width / Screen.height;
    }

    private void Update()
    {
        transform.Translate(new Vector2(_speed * _direction * Time.deltaTime, 0f));
        float positionX = transform.position.x;

        if (Mathf.Abs(positionX) + _colliderWidth > screenBoundPositionX)
            _direction = -_direction;
    }
}
