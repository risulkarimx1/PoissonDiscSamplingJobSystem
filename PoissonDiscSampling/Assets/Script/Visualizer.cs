using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class Visualizer : MonoBehaviour
{
    public float _width;
    public float _height;
    public float _radius;

    [Range(0,1)]
    public float sphereSize;
    // Start is called before the first frame update
    void Start()
    {
        //var poissonDiscSampler = new PoissonDiscSampler(_width,_height,_radius,rand);
        var poissonDiscSampler = new PoissonDiscSampler()
        {
            Width =  _width,
            Height = _height,
            Radius =  _radius,
            CellSize =  _radius / math.sqrt(2),
            Output = new List<float2>(),
            ActiveSamples = new List<float2>()
        };
        poissonDiscSampler.CreateSamples();
        var output = poissonDiscSampler.Output;
        foreach (var sample in output)
        {
            var sampleSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sampleSphere.transform.position = new Vector3(sample.x,0,sample.y);
            sampleSphere.transform.localScale = Vector3.one*sphereSize;
        }
    }
}
