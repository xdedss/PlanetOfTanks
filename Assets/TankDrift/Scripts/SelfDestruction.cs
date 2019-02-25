using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruction : MonoBehaviour
{

    public float life;
    public bool fixedUpdate;

    void Start()
    {

    }

    void Update()
    {
        if (!fixedUpdate)
        {
            life -= Time.deltaTime;
            if (life < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (fixedUpdate)
        {
            life -= Time.fixedDeltaTime;
            if(life < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
