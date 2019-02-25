using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour {

    public Transform target;
    public Transform up;
    public Vector3 localFront;
    public Vector3 localUp;
    [Space]
    public bool update;
    public bool fixedUpdate;
    public bool lateUpdate;

    private void Start()
    {
        if (localFront == Vector3.zero)
        {
            localFront = Vector3.forward;
        }
        if (localUp == Vector3.zero)
        {
            localUp = Vector3.up;
        }
    }

    private void Refresh()
    {
        if (target)
        {
            var look = Quaternion.LookRotation(target.position - transform.position, up ? (up.position - transform.position) : (transform.rotation * localUp));
            var lookLocal = Quaternion.Inverse(Quaternion.LookRotation(localFront, localUp));
            transform.rotation = look * lookLocal;
        }
    }

    private void Update()
    {
        if (update)
        {
            Refresh();
        }
    }

    private void FixedUpdate()
    {
        if (fixedUpdate)
        {
            Refresh();
        }
    }

    private void LateUpdate()
    {
        if (lateUpdate)
        {
            Refresh();
        }
    }
}
