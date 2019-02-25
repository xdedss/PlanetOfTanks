using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Singleton<T> where T : MonoBehaviour
{

    public static T instance
    {
        get
        {
            if (_instance)
            {
                return _instance;
            }
            else
            {
                Debug.Log("no instance found" + typeof(T));
                return null;
                //return new GameObject(typeof(T).ToString()).AddComponent<T>();
            }
        }
    }

    private static T _instance;

    public static void Clear()
    {
        _instance = null;
    }

    public static void Register(T t)
    {
        if (_instance && t != _instance)
        {
            Debug.Log("instance already exists" + typeof(T));
            UnityEngine.Object.Destroy(t);
        }
        else
        {
            _instance = t;
        }
    }

}
