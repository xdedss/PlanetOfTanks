using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    private void Awake()
    {
        Singleton<TimeManager>.Register(this);
    }

    void Start () {
		
	}
	
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            UnityEditor.EditorApplication.isPaused = true;
        }
#endif
    }
    
    public delegate void DelayedFunc();
    public void DelayFunc(DelayedFunc func, float time)
    {
        StartCoroutine(DelayFunc_(func, time));
    }
    IEnumerator DelayFunc_(DelayedFunc func, float time)
    {
        yield return new WaitForSeconds(time);
        func.Invoke();
    }
}
