﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    public float Width;

    public float Height;

    public float Radius;
    // Start is called before the first frame update
    void Start()
    {
        var poissonDiscSampler = new PoissonDiscSampler(Width,Height,Radius);
        foreach (var sample in poissonDiscSampler.Samples())
        {
            var sampleSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sampleSphere.transform.position = new Vector3(sample.x,0,sample.y);
            sampleSphere.transform.localScale = new Vector3(.25f,.25f,.25f);
        }
    }
}