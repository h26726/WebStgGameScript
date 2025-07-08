using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting) //遊戲正在退出
                return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError("Singleton Repeat");
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void OnDestroy()
    {
        _applicationIsQuitting = true;
    }
}