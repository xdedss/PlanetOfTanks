using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleTankControlTest : MonoBehaviour {

    //public WheelForceControl leftWheels;
    //public WheelForceControl rightWheels;
    public TrackPhysics leftTrack;
    public TrackPhysics rightTrack;
    public TurrentMotor turrent;
    public TurrentMotor cannon;

    public Transform frontMark;
    Rigidbody rigid;

    Vector3 turrentTarget;
    float leftmove = 0;
    float rightmove = 0;
    [SerializeField]
    float maxPower = 1000;
    
	void Start () {
        rigid = GetComponent<Rigidbody>();
	}
	

	void Update () {
        leftmove = Input.GetAxis("Forward1");
        rightmove = Input.GetAxis("Forward2");
        //Camera.main.transform.LookAt(transform.position);
        
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000, 1 << 10))
        {
            turrentTarget = hit.point;
        }
        else
        {
            turrentTarget = ray.origin + 1000 * ray.direction;
        }
        turrent.worldTarget = turrentTarget;
        cannon.worldTarget = turrentTarget;

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            EditorApplication.isPaused = true;
        }
#endif
    }

    private void FixedUpdate()
    {
        leftTrack.power = maxPower * leftmove;
        rightTrack.power = maxPower * rightmove;
        leftTrack.brake = Input.GetAxisRaw("Forward1") == 0 && Input.GetKey(KeyCode.Space) ? 400000 : 0;
        rightTrack.brake = Input.GetAxisRaw("Forward2") == 0 && Input.GetKey(KeyCode.Space) ? 400000 : 0;
    }

    private void OnGUI()
    {
        //Vector3 w2s = Camera.main.WorldToScreenPoint(cannon.transform.position + cannon.transform.forward * 1000);
        GUI.Label(new Rect(0, 0, 200, 30), (rigid.velocity * 3.6f).magnitude.ToString("0.00") + " km/h");
        //GUI.Label(new Rect(new Vector2(w2s.x, Screen.height - w2s.y), new Vector2(50, 50)), "here");
        //Debug.Log(w2s);
    }
}
