using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerBody : MonoBehaviour
{
    public event Action<Collision2D> Collided;
    public event Action<Collider2D> Triggered;

    private void OnCollisionEnter2D(Collision2D collision) => Collided?.Invoke(collision);

    private void OnTriggerEnter2D(Collider2D collision) => Triggered?.Invoke(collision);
}
