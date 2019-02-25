using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereControl : MonoBehaviour {

    public Material atmoMat;
    public Transform sun;
    public float nightFactor;
    public Gradient SkyColor;
    public Vector4 SkyGradientASL;
    public Gradient FogColor;
    public Vector4 FogDistanceASL;
    float earthRadius;

    void Start () {
        earthRadius = RelativeSystemControl.worldRadius;
	}
	
	void Update () {
        nightFactor = Vector3.Angle(sun.forward, RelativeSystemControl.DownAt(transform.position)) / 180f;
        float distance = Mathf.Max((transform.position - RelativeSystemControl.system.position).magnitude, earthRadius + 1);
        float biasAngle = Mathf.Acos(earthRadius / distance);
        float skyTopAlpha = Mathf.Lerp(0, 1, Mathf.Clamp01(Mathf.InverseLerp(100 + earthRadius, earthRadius, distance)));
        float thickness = earthRadius / distance;

        atmoMat.SetVector("_SkyDistance", new Vector4(SkyGradientASL.x * skyTopAlpha, SkyGradientASL.y, SkyGradientASL.z - biasAngle, SkyGradientASL.z - biasAngle + (SkyGradientASL.w - SkyGradientASL.z) * thickness));
        atmoMat.SetColor("_SkyColor", SkyColor.Evaluate(nightFactor));
        atmoMat.SetVector("_FogDistance", new Vector4(FogDistanceASL.x, FogDistanceASL.y, FogDistanceASL.z - biasAngle, FogDistanceASL.w - biasAngle));
        atmoMat.SetColor("_FogColor", FogColor.Evaluate(nightFactor));

    }
}
