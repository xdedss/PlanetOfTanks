using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyOf : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        var t = Instantiate(target);
        t.transform.position = transform.position;
        t.transform.rotation = transform.rotation;
        t.name = gameObject.name;
        Destroy(gameObject);
    }

    void Update()
    {

    }
}
