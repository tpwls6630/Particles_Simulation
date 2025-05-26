using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name + " (Singleton)");
                    _instance = obj.AddComponent<T>();
                }
            }
            DontDestroyOnLoad(_instance.gameObject);
            return _instance;
        }
    }
}
