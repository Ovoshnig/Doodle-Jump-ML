using UnityEngine;

public abstract class Platform : MonoBehaviour
{
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
