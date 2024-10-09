using UnityEngine;

public class DisappearingPlatform : Platform
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<PlayerLegs>(out _))
        {
            if (collision.gameObject.TryGetComponent(out Rigidbody2D rigidbody)
                && rigidbody.linearVelocityY <= 0f)
                    gameObject.SetActive(false);
        }
    }
}
