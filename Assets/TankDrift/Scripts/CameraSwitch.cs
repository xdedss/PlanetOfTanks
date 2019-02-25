using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{

    public Transform parentNormal;
    public Transform parentAim;
    public Transform parentMap;
    public Transform fog;

    [Space]
    [SerializeField]
    public bool aimMode;
    [SerializeField]
    public bool mapMode;

    private void Awake()
    {
        Singleton<CameraSwitch>.Register(this);
    }

    void Start()
    {

    }

    void Update()
    {
        if (mapMode)
        {
            Camera.main.cullingMask &= ~(1 << 15);
            Camera.main.cullingMask |= 1 << 14;
            Camera.main.cullingMask &= ~(1 << 13);
            transform.parent = parentMap;
            GetComponent<Camera>().fieldOfView = 60;
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.zero;
            fog.gameObject.SetActive(false);
        }
        else
        {
            Camera.main.cullingMask |= 1 << 15;
            Camera.main.cullingMask &= ~(1 << 14);
            Camera.main.cullingMask |= 1 << 13;
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                aimMode ^= true;
            }
            fog.gameObject.SetActive(true);
            if (aimMode)
            {
                transform.parent = parentAim;
                GetComponent<Camera>().fieldOfView = 5;
                transform.localEulerAngles = Vector3.zero;
                transform.localPosition = Vector3.zero;
            }
            else
            {
                transform.parent = parentNormal;
                GetComponent<Camera>().fieldOfView = 60;
                transform.localEulerAngles = Vector3.zero;
                transform.localPosition = Vector3.zero;
            }
        }
    }
}
