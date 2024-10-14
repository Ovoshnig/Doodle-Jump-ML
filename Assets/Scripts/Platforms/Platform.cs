using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Platform : MonoBehaviour
{
    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
    }
}
