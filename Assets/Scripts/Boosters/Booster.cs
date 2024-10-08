using UnityEngine;

public abstract class Booster : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerBody _)
            || collision.TryGetComponent(out PlayerLegs _))
            gameObject.SetActive(false);
    }
}
