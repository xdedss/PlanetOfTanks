using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeSystemControl : MonoBehaviour {

    public static Transform system;
    public static Vector3 DownAt(Vector3 pos)
    {
        return ((system ? system.position : Vector3.zero) - pos).normalized;
    }
    public static float worldRadius = 1200;

    public Transform theCenterOfTheUniverse;
    public float rotateAngle;

    private void Awake()
    {
        CenterGravity.center = transform;
        system = transform;
    }

    void Start () {
        TranslateAllTo(new Vector3(0, 10, 0));
	}
	
	void Update () {
        if (Vector3.Angle(theCenterOfTheUniverse.position - transform.position, Vector3.up) > rotateAngle)
        {
            RotateAll();
            Debug.Log("Rotate");
        }
	}

    private void RotateAll()
    {
        Vector3 centerLocal = theCenterOfTheUniverse.position - transform.position;
        Quaternion delta = Quaternion.FromToRotation(centerLocal.normalized, Vector3.up);
        theCenterOfTheUniverse.position = delta * centerLocal + transform.position;
        transform.rotation = delta * transform.rotation;
        theCenterOfTheUniverse.rotation = delta * theCenterOfTheUniverse.rotation;
    }

    private void TranslateAll(Vector3 delta)
    {
        transform.position += delta;
        theCenterOfTheUniverse.position += delta;
    }

    private void TranslateAllTo(Vector3 to)
    {
        Vector3 delta = to - theCenterOfTheUniverse.position;
        TranslateAll(delta);
    }
}
