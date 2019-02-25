using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    public CameraSwitch cswitch;
    public Follow follow;

    TankManager tm;

    void Start()
    {
        tm = Singleton<TankManager>.instance;
    }

    void Update()
    {
        var player = Singleton<TankInputPlayer>.instance;
        if (player)
        {
            Focus(player.GetComponent<TankControl>());
        }
    }

    void Focus(TankControl tank)
    {
        if (tank)
        {
            var camRef = tank.transform.Find("CameraReference");
            var cannonMark = tank.cannonMark;
            follow.target = camRef;
            cswitch.parentAim = cannonMark;
        }
    }
}
