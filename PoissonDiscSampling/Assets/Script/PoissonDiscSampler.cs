using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PoissonDiscSampler
{
    private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.
    private readonly float _radius; 
    private readonly float _width;
    private readonly float _height;
    private readonly float _cellSize;
    private LinearGrid _linearGrid;
    private List<Vector2> _activeSamples;

    /// Create a sampler with the following parameters:
    ///
    /// width:  each sample's x coordinate will be between [0, width]
    /// height: each sample's y coordinate will be between [0, height]
    /// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
    public PoissonDiscSampler(float width, float height, float radius)
    {
        //_rect = new Rect(0, 0, width, height);
        _radius = radius;
        _cellSize = radius / Mathf.Sqrt(2);
        _height = height;
        _width = width;

        _linearGrid = new LinearGrid()
        {
            Width = Mathf.CeilToInt(width / _cellSize),
            Height = Mathf.CeilToInt(height / _cellSize)
        };

        _linearGrid.CreateGrid();
        _activeSamples = new List<Vector2>();
    }

    /// Return a lazy sequence of samples. You typically want to call this in a foreach loop, like so:
    ///   foreach (Vector2 sample in sampler.Samples()) { ... }
    public IEnumerable<Vector2> Samples()
    {
        // First sample is chosen randomly
        var firstSample = new Vector2(Random.value * _width, Random.value * _height);
        yield return AddSample(firstSample);

        while (_activeSamples.Count > 0)
        {

            // Pick a random active sample
            var index = (int)Random.value * _activeSamples.Count;
            Vector2 sample = _activeSamples[index];

            // Try `k` random candidates between [radius, 2 * radius] from that sample.
            bool found = false;
            for (int j = 0; j < k; ++j)
            {
                var randomDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                
                var randomMagnitude = Random.Range(_radius, _radius * 2);
                var offset = randomDirection * randomMagnitude;
                var candidate = sample + offset;


                // Accept candidates if it's inside the _rect and farther than 2 * radius to any existing sample.

                if (Contains(candidate) && IsFarEnough(candidate))
                {
                    found = true;
                    yield return AddSample(candidate);
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

    private bool IsFarEnough(Vector2 sample)
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
                Vector2 s = _linearGrid.GetValue(x, y);
                if (s != Vector2.zero)
                {
                    var d = (s - sample).magnitude;
                    if (d< (_radius))
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
    private Vector2 AddSample(Vector2 sample)
    {
        _activeSamples.Add(sample);
        var pos = GetGridPosition(sample);
        _linearGrid.AddValue(pos.x, pos.y,sample);
        return sample;
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
}