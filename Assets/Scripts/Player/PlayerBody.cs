using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerBody : MonoBehaviour
{
    public event Action CollidedWithMonster;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent<Monster>(out _))
            CollidedWithMonster?.Invoke();
    }
}
