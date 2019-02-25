using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMarkControl : MonoBehaviour
{
    public Color mainColor;
    float maxDisplayAngle = 45;
    float minDisplayAngle = 5;
    float maxSize = 70;
    float minSize = 15;
    float maxA = 0.2f;
    float minA = 0f;
    float maxHeight = 300;
    float minHeight = 50;
    MeshRenderer mr;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mr.material = new Material(mr.material);
        transform.localPosition = new Vector3(0, 180, 0);
    }

    void Update()
    {
        Vector3 camDown = RelativeSystemControl.DownAt(Camera.main.transform.position);
        Vector3 thisDown = -transform.up;
        float angle = Vector3.Angle(camDown, thisDown);
        if(angle > maxDisplayAngle)
        {
            mr.enabled = false;
        }
        else
        {
            mr.enabled = true;
            var t = Mathf.InverseLerp(minDisplayAngle, maxDisplayAngle, angle);
            var size = Mathf.Lerp(minSize, maxSize, t);
            var a = Mathf.Lerp(minA, maxA, Mathf.Clamp01(t));
            var height = Mathf.Lerp(minHeight, maxHeight, t);
            transform.localPosition = new Vector3(0, height, 0);
            transform.localScale = Vector3.one * size;
            mr.material.SetColor("_TintColor", new Color(mainColor.r, mainColor.g, mainColor.b, a));
        }
    }
}
