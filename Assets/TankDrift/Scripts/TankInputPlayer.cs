using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankInputPlayer : MonoBehaviour {

    public TankControl tc;
    public Transform aimRef;
    public Texture2D aimMark;

    Vector3 prediction;
    TankControl predictionHit;
    bool hasPrediction;

    private void Awake()
    {
        //Singleton<TankInputPlayer>.Register(this);
    }

    void Start () {
        if (enabled)
        {
            Singleton<TankInputPlayer>.Register(this);
        }
        if (!tc)
        {
            tc = GetComponent<TankControl>();
        }
	}

    private void OnEnable()
    {
        Singleton<TankInputPlayer>.Register(this);
    }

    void Update () {

        tc.leftMove = Input.GetAxis("Forward1");
        tc.rightMove = Input.GetAxis("Forward2");
        tc.leftBrake = Input.GetKey(KeyCode.Space) && Input.GetAxisRaw("Forward1") == 0;
        tc.rightBrake = Input.GetKey(KeyCode.Space) && Input.GetAxisRaw("Forward2") == 0;

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.A))
        {
            tc.TryFire();
        }

        Ray ray = new Ray(aimRef.position, aimRef.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, 1 << 10))
        {
            tc.turrentTarget = hit.point;
        }
        else
        {
            tc.turrentTarget = ray.origin + 1000 * ray.direction;
        }
        
        if(tc.Predict(0.4f, 6, out predictionHit, out prediction))
        {
            hasPrediction = true;
        }
    }


    private void OnGUI()
    {
        if (!tc.isDestroyed)
        {
            Vector2 hsize = new Vector2(15, 15);
            Vector3 w2s = Camera.main.WorldToScreenPoint(prediction);
            Vector2 center = new Vector2(w2s.x, Screen.height - w2s.y);
            //GUI.Label(new Rect(new Vector2(w2s.x, Screen.height - w2s.y), new Vector2(50, 50)), "↖" + predictionHit);
            GUI.DrawTexture(new Rect(center - hsize, 2 * hsize), aimMark);

            //Vector3 w2s = Camera.main.WorldToScreenPoint(cannon.transform.position + cannon.transform.forward * 1000);
            GUI.Label(new Rect(0, 0, 200, 30), (tc.GetComponent<Rigidbody>().velocity * 3.6f).magnitude.ToString("0.00") + " km/h");
            //GUI.Label(new Rect(new Vector2(w2s.x, Screen.height - w2s.y), new Vector2(50, 50)), "here");
            //Debug.Log(w2s);
        }
    }
}
