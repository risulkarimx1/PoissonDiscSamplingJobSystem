using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private float _width;
    [SerializeField] private float _height;
    [SerializeField] private float _radius;

    [Range(0,1)]
    [SerializeField] private float _sphereSize;

    [SerializeField] private uint _randomSeed;
    // Start is called before the first frame update
    void Start()
    {
        var activeSamples = new NativeList<float2>(Allocator.TempJob);
        var results = new NativeList<float2>(Allocator.TempJob);

        var cellSize = _radius / math.sqrt(2);

        var gridWidth = (int) math.ceil(_width / cellSize);
        var gridHeight = (int)math.ceil(_height/ cellSize);

        var gridArray = new NativeArray<float2>(gridWidth * gridHeight, Allocator.TempJob);
        for (var i = 0; i < gridArray.Length; i++)
        {
            gridArray[i]= new float2(float.MinValue, float.MinValue);
        }

        var poissonDiscSampler = new PoissonDiscSampler()
        {
            // Area Parameters
            Width =  _width,
            Height = _height,
            Radius =  _radius,
            CellSize =  cellSize,
            RandomSeed = _randomSeed,
            // Grid Parameters
            GridWidth = gridWidth,
            GridHeight = gridHeight,
            GridArray = gridArray,

            // Sample and Results
            ActiveSamples = activeSamples,
            Result = results
        };

        var handle = poissonDiscSampler.Schedule();
        handle.Complete();
        Debug.Log($"Length of result is: {results.Length}");
        for (int i =0;i<results.Length;i++)
        {
            var sampleSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sampleSphere.transform.position = new Vector3(results[i].x, 0, results[i].y);
            sampleSphere.transform.localScale = Vector3.one * _sphereSize;
        }
        activeSamples.Dispose();
        results.Dispose();
        gridArray.Dispose();
    }
}
