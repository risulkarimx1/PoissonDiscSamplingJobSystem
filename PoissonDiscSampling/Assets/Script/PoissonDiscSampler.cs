using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
public class PoissonDiscSampler
{
    private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.
    private readonly float _radius; 
    private readonly float _width;
    private readonly float _height;
    private readonly float _cellSize;
    private LinearGrid _linearGrid;
    private List<float2> _activeSamples;

    public List<float2> Output;
    private Random _random;
    /// Create a sampler with the following parameters:
    ///
    /// width:  each sample's x coordinate will be between [0, width]
    /// height: each sample's y coordinate will be between [0, height]
    /// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
    public PoissonDiscSampler(float width, float height, float radius,Random random)
    {
        //_rect = new Rect(0, 0, width, height);
        _radius = radius;
        _cellSize = radius / math.sqrt(2);
        _height = height;
        _width = width;
        _random = random;
        _linearGrid = new LinearGrid()
        {
            Width = (int) math.ceil(width / _cellSize),
            Height = (int) math.ceil(height / _cellSize)
        };

        _linearGrid.CreateGrid();
        _activeSamples = new List<float2>();
        Output = new List<float2>();
        
    }

    /// Return a lazy sequence of samples. You typically want to call this in a foreach loop, like so:
    ///   foreach (Vector2 sample in sampler.CreateSamples()) { ... }
    public void CreateSamples()
    {
        // First sample is chosen randomly
        var firstSample = new float2(_random.NextFloat()* _width, _random.NextFloat() * _height);
        AddSample(firstSample);

        while (_activeSamples.Count > 0)
        {

            // Pick a _random active sample
            var index = (int)_random.NextFloat() * _activeSamples.Count;
            var sample = _activeSamples[index];

            // Try `k` _random candidates between [radius, 2 * radius] from that sample.
            bool found = false;
            for (int j = 0; j < k; ++j)
            {
                var randomDirection = new float2(_random.NextFloat(-1.0f, 1.0f), _random.NextFloat(-1.0f, 1.0f));
                
                var randomMagnitude = _random.NextFloat(_radius, _radius * 2);
                var offset = randomDirection * randomMagnitude;
                var candidate = sample + offset;


                // Accept candidates if it's inside the _rect and farther than 2 * radius to any existing sample.

                if (Contains(candidate) && IsFarEnough(candidate))
                {
                    found = true;
                    AddSample(candidate);
                    break;
                }
            }

            // If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
            if (!found)
            {
                _activeSamples[index] = _activeSamples[_activeSamples.Count - 1];
                _activeSamples.RemoveAt(_activeSamples.Count - 1);
            }
        }
    }

    private bool IsFarEnough(float2 sample)
    {
        var pos = GetGridPosition(sample);

        int xmin = Mathf.Max(pos.x - 2, 0);
        int ymin = Mathf.Max(pos.y - 2, 0);

        int xmax = Mathf.Min(pos.x + 2, _linearGrid.Width - 1);
        int ymax = Mathf.Min(pos.y + 2, _linearGrid.Height- 1);

        for (int y = ymin; y <= ymax; y++)
        {
            for (int x = xmin; x <= xmax; x++)
            {
                var s = _linearGrid.GetValue(x, y);
                if (s.x != float.MinValue)
                {
                    
                    if (Distance(s, sample) < (_radius))
                        return false;
                }
            }
        }

        return true;

        // Note: we use the zero vector to denote an unfilled cell in the _linearGrid. This means that if we were
        // to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
        // and we might end up with another sample too close from (0, 0). This is a very minor issue.
    }

    /// Adds the sample to the active samples queue and the _linearGrid before returning it
    private void AddSample(Vector2 sample)
    {
        _activeSamples.Add(sample);
        var pos = GetGridPosition(sample);
        _linearGrid.AddValue(pos.x, pos.y,sample);
        Output.Add(sample);
    }

    private Vector2Int GetGridPosition(Vector2 sample)
    {
        var x = (int)(sample.x / _cellSize);
        var y = (int)(sample.y / _cellSize);
        return new Vector2Int(x,y);
    }

    private bool Contains(Vector2 candidate)
    {
        return (candidate.x >= 0 && candidate.x < _width
              &&
              candidate.y >= 0 && candidate.y < _height);
    }

    private float Distance(float2 a, float2 b)
    {
        return math.sqrt(math.pow((a.x - b.x), 2) + math.pow((a.y - b.y), 2));
    }
}