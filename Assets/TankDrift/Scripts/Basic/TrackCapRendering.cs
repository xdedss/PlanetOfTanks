using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TrackCapRendering : MonoBehaviour {

    public Camera mainCam;
    public Transform t;
    public Camera capCam;
    public RenderTexture tex;
    public Shader replacementShader;
    [Space]
    public Material carveMat;

	void Start () {
        capCam = null;
	}

    void Update()
    {
        EnsureCameraStatus();
    }

    void EnsureCameraStatus()
    {
        if (mainCam == null || capCam == null || t == null || tex == null || tex.width != Screen.width || carveMat.GetTexture("_CutoffMap") == null)
        {
            Debug.Log("refreshing capcam");
            if (t)
            {
                DestroyImmediate(t.gameObject);
            }
            mainCam = Camera.main;
            mainCam.depthTextureMode = DepthTextureMode.DepthNormals;
            t = new GameObject("CapCam").transform;
            t.parent = mainCam.transform;
            t.localEulerAngles = Vector3.zero;
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.zero;
            capCam = t.gameObject.AddComponent<Camera>();
            capCam.CopyFrom(mainCam);
            capCam.depth = -2;
            capCam.cullingMask = 1 << 9;
            capCam.renderingPath = RenderingPath.Forward;
            capCam.clearFlags = CameraClearFlags.SolidColor;
            capCam.backgroundColor = Color.black;

            if (tex)
            {
                DestroyImmediate(tex);
            }
            tex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.R8);
            tex.filterMode = FilterMode.Point;
            capCam.targetTexture = tex;
            capCam.SetReplacementShader(replacementShader, "RenderType");

            //capCam.enabled = false;


            carveMat.SetTexture("_CutoffMap", tex);
            //TrackCapMark[] allTrackCap = FindObjectsOfType<TrackCapMark>();
            //foreach(TrackCapMark mark in allTrackCap)
            //{
            //    mark.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_CutoffMap", tex);
            //}
        }

        if (capCam.fieldOfView != mainCam.fieldOfView)
        {
            capCam.fieldOfView = mainCam.fieldOfView;
        }
    }
}
