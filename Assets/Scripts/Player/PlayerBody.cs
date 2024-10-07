using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerBody : MonoBehaviour
{
    private Collider2D _collider;

    public event Action WithMonsterCollided;

    private void Awake() => _collider = GetComponent<Collider2D>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Monster>(out _))
        {
            _collider.enabled = false;
            WithMonsterCollided?.Invoke();
        }
    }
}
