using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour {

    public float maxPitch;
    public float maxYaw;

    float currentY;
    float currentX;
    public float scale;

	void Start () {
        currentY = transform.localEulerAngles.y;
        currentX = transform.localEulerAngles.x;
	}
	
	void Update () {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = -Input.GetAxis("Mouse Y");
        float slowfac = Singleton<CameraSwitch>.instance.aimMode ? 0.2f : 1f;
        currentY = currentY + mouseX * scale * slowfac * Time.deltaTime;
        currentX = Mathf.Clamp(currentX + mouseY * scale * slowfac * Time.deltaTime, -85, 85);

        transform.localEulerAngles = new Vector3(currentX, currentY, 0);
	}
}
