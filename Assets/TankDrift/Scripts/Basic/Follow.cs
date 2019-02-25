using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Follow : MonoBehaviour {

    [SerializeField]
    public Transform target;
    [SerializeField]
    Vector3 bias;
    [SerializeField]
    public bool translation;
    [SerializeField]
    bool translationLocal;
    [SerializeField]
    Vector3 biasEuler;
    [SerializeField]
    public bool rotation;
    [SerializeField]
    bool rotationLocal;

    [Space]
    [SerializeField]
    public bool update;
    [SerializeField]
    public bool fixedUpdate;
    [SerializeField]
    public bool lateUpdate;
    
	void Start ()
    {
        Refresh();
    }
	
	void Update () {
        if (update)
            Refresh();
	}

    private void LateUpdate()
    {
        if (lateUpdate)
            Refresh();
    }

    private void FixedUpdate()
    {
        if (fixedUpdate)
            Refresh();
    }

    void Refresh()
    {
        if (target)
        {
            if (translation)
            {
                transform.position = target.position + bias;
            }
            else if (translationLocal)
            {
                transform.localPosition = target.localPosition + bias;
            }
            if (rotation)
            {
                transform.rotation = target.rotation;
            }
            else if (rotationLocal)
            {
                transform.localEulerAngles = target.localEulerAngles + biasEuler;
            }
        }
    }
}
