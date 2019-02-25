using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour {

    public bool relativeToPlanet = true;
    public bool hasAxis = true;
    
    [SerializeField]
    Vector3 localFront = new Vector3(0, 1, 0);
    [SerializeField]
    Vector3 localUp = new Vector3(0, 0, -1);
    Vector3 axis;

    void Start ()
    {

    }
	
	void Update ()
    {
        if (hasAxis)
        {
            if (relativeToPlanet)
            {
                axis = -RelativeSystemControl.DownAt(transform.position);
            }
            else
            {
                axis = transform.up;
            }


            var look = Quaternion.LookRotation(axis, Camera.main.transform.position - transform.position);
            var startlocal = Quaternion.Inverse(Quaternion.LookRotation(localFront, localUp));
            transform.rotation = look * startlocal;
        }
        else
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
