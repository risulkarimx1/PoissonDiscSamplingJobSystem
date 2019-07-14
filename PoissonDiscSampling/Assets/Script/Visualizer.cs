using UnityEngine;
using Random = Unity.Mathematics.Random;

public class Visualizer : MonoBehaviour
{
    public float Width;
    public float Height;
    public float Radius;

    [Range(0,1)]
    public float sphereSize;
    // Start is called before the first frame update
    void Start()
    {
        var rand = new Random(1);
        var poissonDiscSampler = new PoissonDiscSampler(Width,Height,Radius,rand);
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
