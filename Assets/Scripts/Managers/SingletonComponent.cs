using UnityEngine;

public class SingletonComponent<T> : MonoBehaviour where T : Object
{
    private static T _instance;
    public static T Inst => _instance ? _instance : FindObjectOfType<T>();

    protected virtual void Awake()
    {
        if (_instance != null && _instance != GetComponent<T>()) return;
        _instance = GetComponent<T>();
    }
}