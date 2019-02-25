using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class TrackParticleControl : MonoBehaviour {

    public float maxSize;
    public float minSize;
    public float maxSizeVel;
    public float minSizeVel;
    [Space]
    public float maxRate;
    public float minRate;
    public float maxRateVel;
    public float minRateVel;
    [Space]
    public bool twoSide;
    [Space]
    public ParticleSystem ps;
    Vector3 m_lastPos;

	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	void Update () {
        if(m_lastPos == Vector3.zero)
        {
            m_lastPos = transform.position;
        }

        Vector3 d = transform.position - m_lastPos;
        float vel = GetH(0.3f) > 0.3f ? Vector3.Dot(transform.forward, d) / Time.deltaTime : 0;
        //lastVel = Mathf.MoveTowards(lastVel, vel, Time.deltaTime * 8);
        float altVel = Mathf.Max(0, (twoSide ? Mathf.Abs(vel) : vel) - 1);

        var main = ps.main;
        main.startSize = Mathf.Lerp(minSize, maxSize, Mathf.Clamp01(Mathf.InverseLerp(minSizeVel, maxSizeVel, altVel)));

        var em = ps.emission;
        var rateOverTime = em.rateOverTime;
        rateOverTime.constant = Mathf.Lerp(minRate, maxRate, Mathf.Clamp01(Mathf.InverseLerp(minRateVel, maxRateVel, altVel)));
        em.rateOverTime = rateOverTime;

        

        m_lastPos = transform.position;
	}

    private float GetH(float max)
    {
        Ray ray = new Ray(transform.position + transform.up, -transform.up);
        RaycastHit hit;
        float distance = max;
        if(Physics.Raycast(ray, out hit, 1f + max, 1 << 10))
        {
            distance = hit.distance;
        }
        return distance;
    }
}
