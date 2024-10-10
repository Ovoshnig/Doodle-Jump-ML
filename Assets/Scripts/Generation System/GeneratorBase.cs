using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class GeneratorBase : MonoBehaviour
{
    protected GenerationSettings Settings { get; private set; }
    protected Camera Camera { get; private set; }
    protected Dictionary<GameObject, IObjectPool<GameObject>> ActiveObjects { get; private set; } = new();

    protected virtual void Awake() => Camera = Camera.main;

    public void SetSettings(GenerationSettings settings) => Settings = settings;

    public abstract void Generate(float height);

    public void RemoveOffScreenElements()
    {
        List<GameObject> objectsToRemove = new();

        foreach (var activeObject in ActiveObjects.Keys)
        {
            Vector2 position = activeObject.transform.position;
            Vector2 viewportPosition = Camera.WorldToViewportPoint(position);

            if (viewportPosition.y < 0f)
                objectsToRemove.Add(activeObject);
        }

        foreach (var activeObject in objectsToRemove)
        {
            if (ActiveObjects.TryGetValue(activeObject, out IObjectPool<GameObject> pool))
                pool.Release(activeObject);
            else
                Debug.LogError($"Unable to find object {activeObject} in active objects");

            ActiveObjects.Remove(activeObject);
        }
    }

    protected abstract Vector2 GetRandomPosition(float height);
}
