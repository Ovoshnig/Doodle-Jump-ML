using UnityEngine;

public class FlyingMonster : Monster
{
    [SerializeField, Min(0f)] private float _rotationSpeed;
    [SerializeField, Min(0f)] private float _maxTilt;

    private int _direction;

    protected override void Start()
    {
        base.Start();
        _direction = Random.Range(0, 2) == 0 ? -1 : 1;
    }

    protected override void Update()
    {
        float rotationZ = transform.eulerAngles.z;

        if (rotationZ > 180f)
            rotationZ -= 360f;

        if (Mathf.Abs(rotationZ) > _maxTilt)
            _direction = -_direction;

        transform.Rotate(0f, 0f, _direction * _rotationSpeed * Time.deltaTime);
    }
}
