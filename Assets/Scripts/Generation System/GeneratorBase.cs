using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public abstract class GeneratorBase : MonoBehaviour
{
    protected GenerationSettings Settings { get; private set; }
    protected Camera Camera { get; private set; }
    protected Dictionary<string, float> ObjectBoundsX { get; } = new();
    protected Dictionary<string, float> ObjectHalfSizesY { get; } = new();
    protected Dictionary<GameObject, IObjectPool<GameObject>> ActiveObjects { get; } = new();

    protected virtual void Awake() => Camera = Camera.main;

    public void SetSettings(GenerationSettings settings) => Settings = settings;

    public abstract void Generate(float height);

    public void RemoveOffScreenElements()
    {
        GameObject[] keys = ActiveObjects.Keys.ToArray();

        for (int i = 0; i < keys.Length; i++)
        {
            GameObject activeObject = keys[i];
            Vector2 position = activeObject.transform.position;
            Vector2 topPosition = new(position.x, position.y + ObjectHalfSizesY[activeObject.name]);
            Vector2 topViewportPosition = Camera.WorldToViewportPoint(topPosition);

            if (topViewportPosition.y < 0f)
            {
                if (ActiveObjects.TryGetValue(activeObject, out IObjectPool<GameObject> pool))
                    pool.Release(activeObject);
                else
                    Debug.LogError($"Unable to find object {activeObject} in active objects");

                ActiveObjects.Remove(activeObject);
            }
        }
    }
}
