using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    private const string HorizontalAxisName = "Horizontal";

    [SerializeField, Min(0f)] private float _jumpForce = 1f;
    [SerializeField, Min(0f)] private float _horizontalSpeed = 1f;
    [SerializeField, Min(0f)] private float _maxVelocityMagnitude = 1f;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private float _horizontalInput = 0f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis(HorizontalAxisName);

        if (_horizontalInput > 0f && !_spriteRenderer.flipX)
            _spriteRenderer.flipX = true;
        else if (_horizontalInput < 0f && _spriteRenderer.flipX)
            _spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
        if (_horizontalInput == 0)
            _rigidbody.linearVelocityX /= 1.2f;
        else
            _rigidbody.AddForceX(_horizontalSpeed * _horizontalInput, ForceMode2D.Impulse);

        _rigidbody.linearVelocityX = Vector2.ClampMagnitude(_rigidbody.linearVelocity, _maxVelocityMagnitude).x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Platform>(out _) 
            && _rigidbody.linearVelocityY <= 0f)
            _rigidbody.AddForceY(_jumpForce, ForceMode2D.Impulse);
    }
}
