using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCollision : MonoBehaviour {

    public float detectDistance = 10;
    public float safeDistance = 1;
    Vector3 localPos;
    Vector3 targetLocalPos;
    
	void Start () {
        localPos = transform.localPosition;
	}
	
	void Update () {
        Vector3 dir = (transform.up - RelativeSystemControl.DownAt(transform.position)).normalized;
        Vector3 originalWorld = (transform.parent.localToWorldMatrix * localPos.V4(1)).V3();
        Ray ray = new Ray(originalWorld + dir * (detectDistance), -dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, detectDistance + safeDistance, (1 << 10) + (1 << 4)))
        {
            targetLocalPos = (transform.parent.worldToLocalMatrix * (hit.point + dir * safeDistance).V4(1)).V3();
        }
        else
        {
            targetLocalPos = localPos;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Mathf.Clamp01(40f * Time.deltaTime));
	}
}
