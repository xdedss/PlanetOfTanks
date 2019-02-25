using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelAnimation : MonoBehaviour {

    //public WheelCollider wheelRef;
    public Transform forwardRef;

    public Vector2 limit;
    //float original;
    float r = 0.35f;


    void Start () {
        //original = transform.localPosition.y;
	}
	
	void Update () {
        Ray ray = new Ray(transform.position, -forwardRef.up);
        RaycastHit hit;
        float d = 1;
        if (Physics.Raycast(ray, out hit, 5, 1 << 10))
        {
            d = hit.distance - r;
        }
        //Debug.Log(d);
        transform.localPosition = transform.localPosition.SetY(Mathf.Clamp(transform.localPosition.y - d, limit.x, limit.y));


    }
}
