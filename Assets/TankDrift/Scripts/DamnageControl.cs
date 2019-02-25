using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamnageControl : MonoBehaviour {

    public DamnageControl contributeTo;
    public float contributeMultiplier;
    public bool directContribute;
    public DamnageArea[] damnageAreas;

    public float hitPoint;
    public bool isDestroyed { get { return hitPoint <= 0; } }
    
	void Start () {
		
	}
	
	void Update () {
		
	}

    public void TakeDamnage(float amount)
    {
        if (contributeTo)
        {
            contributeTo.TakeDamnage(amount * contributeMultiplier);
        }
        else
        {
            hitPoint -= amount;
        }
    }

    public void TakeDamnage(float amount, Vector3 normal, Vector3 worldPoint)
    {
        if (directContribute)
        {
            contributeTo.TakeDamnage(amount, normal, worldPoint);
        }
        else
        {
            foreach (var d in damnageAreas)
            {
                if (d.area.bounds.Contains(worldPoint))
                {
                    Debug.Log(amount + "*" + d.multiplier);
                    amount *= d.multiplier;
                    break;
                }
            }
            TakeDamnage(amount);
        }
    }

    [Serializable]
    public class DamnageArea
    {
        public BoxCollider area;
        public float multiplier;
    }
}
