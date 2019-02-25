using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAnimation : MonoBehaviour {

    Animator animator;

    public Transform forwardRef;
    public Transform target;

    Vector3 lastPos = Vector3.zero;
    Vector3 deltaPos = Vector3.zero;

	void Start ()
    {
        animator = GetComponent<Animator>();
        if (!target)
        {
            target = transform;
        }
	}
	
	void Update () {
		if(lastPos == Vector3.zero)
        {
            deltaPos = Vector3.zero;
        }
        else
        {
            deltaPos = target.position - lastPos;
        }

        animator.SetFloat("Speed", Vector3.Dot(forwardRef.forward, deltaPos) / Time.fixedDeltaTime);

        lastPos = target.position;
	}
}
