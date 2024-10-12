using System;
using UnityEngine;

public class MonsterHead : MonoBehaviour
{
    public event Action<Collision2D> Collided;

    private void OnCollisionEnter2D(Collision2D collision) => Collided?.Invoke(collision);
}
