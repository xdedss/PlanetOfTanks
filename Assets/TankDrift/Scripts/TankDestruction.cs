using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankControl))]
[RequireComponent(typeof(DamnageControl))]
public class TankDestruction : MonoBehaviour {

    private DamnageControl dc;
    private TankControl tc;
    private ParticleManager pm;
    private TimeManager tm;
    
    public MatReplacement[] matReplacements;

	void Start () {
        dc = GetComponent<DamnageControl>();
        tc = GetComponent<TankControl>();
        pm = Singleton<ParticleManager>.instance;
        tm = Singleton<TimeManager>.instance;
	}
	
	void Update () {
        if (dc.isDestroyed && tc.controllable)
        {
            SetDestroyed();
        }
	}

    void SetDestroyed()
    {
        foreach(var matReplacement in matReplacements)
        {
            matReplacement.mr.sharedMaterial = matReplacement.newMat;
        }
        var upRotation = Quaternion.LookRotation(-RelativeSystemControl.DownAt(transform.position));
        //pm.Emit("FragDestroyed", transform.position, upRotation);
        pm.Emit("DustDestroyed", transform.position, upRotation);
        pm.Emit("SparkDestroyed", transform.position, upRotation);
        pm.Emit("LightDestroyed", transform.position, upRotation);
        tm.DelayFunc(() => pm.Emit("SparkDestroyed", transform.position, upRotation), 0.4f);
        tm.DelayFunc(() => pm.Emit("DustDestroyed", transform.position, upRotation), 0.4f);
        tm.DelayFunc(() => pm.Emit("LightDestroyed", transform.position, upRotation), 0.4f);
        tc.controllable = false;
        tc.isDestroyed = true;
        tc.Explode();
    }
    
    [Serializable]
    public class MatReplacement
    {
        public MeshRenderer mr;
        public Material newMat;
    }
}
