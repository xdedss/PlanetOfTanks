using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterGravity : MonoBehaviour {

    public const float g = 9.81f;
    public static Transform center;

    public float multiplier = 1;

    Rigidbody rigid;

	void Start () {
        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = false;
	}

	void FixedUpdate () {
        rigid.AddForce(g * multiplier * (center.position - transform.position).normalized * rigid.mass);
	}
}
