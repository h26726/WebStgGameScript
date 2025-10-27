using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    private static readonly object _lock = new object();
    public static T Instance;

    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (Instance == null)
            {
                Instance = (T)this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"Duplicate Singleton detected: {typeof(T).Name} on {gameObject.name}. Destroying this duplicate.");
                Destroy(gameObject);
            }
        }
    }
}