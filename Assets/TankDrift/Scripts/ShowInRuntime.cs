using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInRuntime : MonoBehaviour {

    public GameObject[] objects;
    public bool show = true;

	void Start () {
        foreach(GameObject gameObj in objects)
        {
            gameObj.SetActive(show);
        }
	}
	
	void Update () {
		
	}
}
