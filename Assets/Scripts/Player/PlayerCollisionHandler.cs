using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerCollisionHandler : MonoBehaviour
{
    public event Action<float> PlatformJumpedOff;
    public event Action<float> MonsterDowned;
    public event Action CrashedIntoMonster;
    public event Action FlewIntoHole;
    public event Action<Booster> BoosterUsed;
    public event Action<StaticBooster> StaticBoosterUsed;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0];
        Vector2 collisionNormal = contact.normal;

        if (collision.collider.TryGetComponent(out Platform platform))
        {
            if (collisionNormal.y > 0.5f)
                PlatformJumpedOff?.Invoke(collision.transform.position.y);
        }
        else if (collision.collider.TryGetComponent(out StaticBooster staticBooster))
        {
            if (collisionNormal.y > 0.5f)
                StaticBoosterUsed?.Invoke(staticBooster);
        }

        if (collision.collider.TryGetComponent(out Monster monster))
        {
            if (collisionNormal.y > 0.5f)
                MonsterDowned?.Invoke(collision.transform.position.y);
            else
                CrashedIntoMonster?.Invoke();
        }

        if (collision.collider.TryGetComponent<Hole>(out _))
            FlewIntoHole?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Booster booster))
            BoosterUsed?.Invoke(booster);
    }
}
