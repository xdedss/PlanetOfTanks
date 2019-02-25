using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CenterAlign : Editor
{
    static Vector3 targetPosition = new Vector3(0, 0, 0);


    [MenuItem(@"Selction/AlignPoint")]
    static void AlignPoint()
    {
        AlignY();
        SnapToGround0();
    }
    [MenuItem(@"Selction/AlignDrop")]
    static void AlignDrop()
    {
        AlignY();
        SnapToGround20();
    }

    [MenuItem(@"Selction/CenterAlign")]
    static void AlignDefault()
    {
        AlignSelection(Vector3.forward, Vector3.up);
    }
    [MenuItem(@"Selction/CenterAlignY")]
    static void AlignY()
    {
        AlignSelection(-Vector3.up, Vector3.forward);
    }

    [MenuItem(@"Selction/SnapToGround/SnapToGround0m")]
    static void SnapToGround0()
    {
        SnapToGround(0);
    }
    [MenuItem(@"Selction/SnapToGround/SnapToGround1m")]
    static void SnapToGround1()
    {
        SnapToGround(1);
    }
    [MenuItem(@"Selction/SnapToGround/SnapToGround2m")]
    static void SnapToGround2()
    {
        SnapToGround(2);
    }
    [MenuItem(@"Selction/SnapToGround/SnapToGround5m")]
    static void SnapToGround5()
    {
        SnapToGround(5);
    }
    [MenuItem(@"Selction/SnapToGround/SnapToGround8m")]
    static void SnapToGround8()
    {
        SnapToGround(8);
    }
    [MenuItem(@"Selction/SnapToGround/SnapToGround10m")]
    static void SnapToGround10()
    {
        SnapToGround(10);
    }
    [MenuItem(@"Selction/SnapToGround/SnapToGround20m")]
    static void SnapToGround20()
    {
        SnapToGround(20);
    }

    static void SnapToGround(float height)
    {
        Transform[] transforms = Selection.transforms;
        foreach (var t in transforms)
        {
            Ray ray = new Ray(t.position, RelativeSystemControl.DownAt(t.position));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1 << 10))
            {
                t.position = hit.point - RelativeSystemControl.DownAt(t.position) * height;
            }
        }
    }

    static void AlignSelection(Vector3 localFront, Vector3 localUp)
    {
        Transform[] transforms = Selection.transforms;
        foreach(var t in transforms)
        {
            var lookLocal = Quaternion.Inverse(Quaternion.LookRotation(localFront, localUp));
            var look = Quaternion.LookRotation(targetPosition - t.position, (t.rotation * localUp));
            t.rotation = look * lookLocal;
        }
    }
}
