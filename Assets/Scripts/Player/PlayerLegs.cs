using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerLegs : MonoBehaviour
{
    public event Action<Transform> CollidedWithPlatform;
    public event Action<Transform> CollidedWithMonster;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Platform>(out _))
            CollidedWithPlatform?.Invoke(collision.transform);
        else if (collision.collider.TryGetComponent<Monster>(out _))
            CollidedWithMonster?.Invoke(collision.transform);
    }
}
