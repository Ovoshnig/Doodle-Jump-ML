using UnityEngine;

public abstract class Platform : MonoBehaviour
{
    protected PlatformGenerator PlatformGenerator { get; private set; }

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
    }

    public void SetPlatformGenerator(PlatformGenerator platformGenerator) => PlatformGenerator = platformGenerator;
}
