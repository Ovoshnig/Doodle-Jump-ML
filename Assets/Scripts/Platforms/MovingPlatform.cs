using UnityEngine;

public class MovingPlatform : Platform
{
    [SerializeField, Min(0f)] private float _speed = 1f;
}
