using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AngleCalculation : MonoBehaviour
{
    public Transform midPoint;

    public Transform intersection;

    public Transform V0, V1, V2, V3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        var v0_2D = new Vector2(V0.position.x, V0.position.z);
        var v1_2D = new Vector2(V1.position.x, V1.position.z);
        var v2_2D = new Vector2(V2.position.x, V2.position.z);
        var v3_2D = new Vector2(V3.position.x, V3.position.z);

        var midX = (v1_2D - v0_2D);
        var magX = midX.magnitude;
        var midPointX = midX * (magX / 2);



        //midPoint.transform.position = new Vector3(v);
    }

    // Update is called once per frame
    void Updatex()
    {
        var v0_2D = new Vector2(V0.position.x, V0.position.z);
        var v3_2D = new Vector2(V3.position.x, V3.position.z);
        var interSectionPos = new Vector2(v3_2D.x, v0_2D.y);
        intersection.position = new Vector3(interSectionPos.x, 0, interSectionPos.y);

        var a = v0_2D - interSectionPos;
        var b = v0_2D - v3_2D;
        var angle = Vector2.Angle(a, b);
        Debug.Log($"Angle Is {angle}");

        var x = midPoint.transform.position.x;
        var y = midPoint.transform.position.z;

        var X = x * Mathf.Cos(angle) - y * Mathf.Sin(angle);
        var Y = x * Mathf.Sin(angle) + y * Mathf.Cos(angle);

        midPoint.transform.position = new Vector3(X,0,Y);

    }
}
