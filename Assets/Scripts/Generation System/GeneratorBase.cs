using UnityEngine;

public abstract class GeneratorBase : MonoBehaviour
{
    protected GenerationSettings Settings { get; private set; }

    public void SetSettings(GenerationSettings settings) => Settings = settings;

    public abstract void Generate(float height);

    public abstract void RemoveOffScreenElements(float cameraHeight);

    protected abstract Vector2 GetRandomPosition(float height);
}
