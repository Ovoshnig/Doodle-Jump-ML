using UnityEngine;

public abstract class Booster : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMover _))
            gameObject.SetActive(false);
    }
}
