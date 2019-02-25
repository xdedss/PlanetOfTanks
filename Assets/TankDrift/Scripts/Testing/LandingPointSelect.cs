using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPointSelect : MonoBehaviour
{
    public Texture2D cross;
    public GameObject dropMe;
    bool showCross;

    void Start()
    {

    }

    void Update()
    {
        var tankplayer = Singleton<TankInputPlayer>.instance;

        if (Input.GetKeyDown(KeyCode.M))
        {
            Singleton<CameraSwitch>.instance.mapMode ^= true;
            if (!tankplayer)
            {
                Singleton<CameraSwitch>.instance.mapMode = true;
            }
        }

        showCross = Singleton<CameraSwitch>.instance.mapMode && (!tankplayer || tankplayer.tc.isDestroyed);
        if (showCross)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 2000, 1 << 10))
                {
                    if (tankplayer)
                    {
                        tankplayer.enabled = false;
                        Singleton<TankInputPlayer>.Clear();
                    }
                    var t = Instantiate(dropMe).transform;
                    t.position = hit.point - 20 * RelativeSystemControl.DownAt(hit.point);
                    //Singleton<TankInputPlayer>.Register(t.GetComponent<TankInputPlayer>());

                    Singleton<CameraSwitch>.instance.mapMode = false;
                }
            }
        }
    }

    private void OnGUI()
    {
        if (showCross)
        {
            var hsize = new Vector2(15, 15);
            var hscr = new Vector2(Screen.width / 2, Screen.height / 2);
            GUI.DrawTexture(new Rect(hscr - hsize, 2 * hsize), cross);
        }
    }
}
