using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanetsRendering : MonoBehaviour {

    public Camera mainCam;
    public Transform t;
    public Camera planetsCam;
    public RenderTexture tex;
    public Material addMat;

    void Start()
    {
        planetsCam = null;
    }
    
    void Update()
    {
        EnsureCameraStatus();
    }

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        planetsCam.Render();
        //Graphics.Blit(tex, source, addMat);
        //Graphics.Blit(source, destination);
        Graphics.Blit(source, destination, addMat);
    }

    void EnsureCameraStatus()
    {
        if (mainCam == null || planetsCam == null || t == null || tex == null || tex.width != Screen.width)
        {
            Debug.Log("refreshing planetscam");
            if (t)
            {
                DestroyImmediate(t.gameObject);
            }
            mainCam = Camera.main;
            t = new GameObject("PlanetsCam").transform;
            t.parent = mainCam.transform;
            t.localEulerAngles = Vector3.zero;
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.zero;
            planetsCam = t.gameObject.AddComponent<Camera>();
            planetsCam.CopyFrom(mainCam);
            planetsCam.farClipPlane = 100;
            planetsCam.cullingMask = 1 << 11;
            planetsCam.renderingPath = RenderingPath.Forward;
            planetsCam.clearFlags = CameraClearFlags.SolidColor;
            planetsCam.backgroundColor = Color.black;
            planetsCam.enabled = false;

            if (tex)
            {
                DestroyImmediate(tex);
            }
            tex = new RenderTexture(Screen.width, Screen.height, 0);
            tex.filterMode = FilterMode.Point;
            planetsCam.targetTexture = tex;
            addMat.SetTexture("_BackTex", tex);
        }
        if(planetsCam.fieldOfView != mainCam.fieldOfView)
        {
            planetsCam.fieldOfView = mainCam.fieldOfView;
        }
    }
}
