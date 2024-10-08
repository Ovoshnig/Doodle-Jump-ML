using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerBody : MonoBehaviour
{
    public event Action<Collision2D> Collided;

    private void OnCollisionEnter2D(Collision2D collision) => Collided?.Invoke(collision);
}
