using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PoissonDiscSampler
{
    private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.

    private readonly Rect rect;
    private readonly float radius;  // radius squared
    private readonly float cellSize;
    private Grid grid;
    private List<Vector2> activeSamples = new List<Vector2>();

    /// Create a sampler with the following parameters:
    ///
    /// width:  each sample's x coordinate will be between [0, width]
    /// height: each sample's y coordinate will be between [0, height]
    /// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
    public PoissonDiscSampler(float width, float height, float radius)
    {
        rect = new Rect(0, 0, width, height);
        this.radius = radius;
        cellSize = radius / Mathf.Sqrt(2);
        grid = new Grid(Mathf.CeilToInt(width / cellSize),
            Mathf.CeilToInt(height / cellSize));
    }

    /// Return a lazy sequence of samples. You typically want to call this in a foreach loop, like so:
    ///   foreach (Vector2 sample in sampler.Samples()) { ... }
    public IEnumerable<Vector2> Samples()
    {
        // First sample is chosen randomly
        var firstSample = new Vector2(Random.value * rect.width, Random.value * rect.height);
        yield return AddSample(firstSample);

        while (activeSamples.Count > 0)
        {

            // Pick a random active sample
            int i = (int)Random.value * activeSamples.Count;
            Vector2 sample = activeSamples[i];

            // Try `k` random candidates between [radius, 2 * radius] from that sample.
            bool found = false;
            for (int j = 0; j < k; ++j)
            {
                var randomDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                
                var randomMagnitude = Random.Range(radius*1.0f, radius * 2.0f);
                var offset = randomDirection * randomMagnitude;
                var candidate = sample + offset;


                // Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
                if (rect.Contains(candidate) && IsFarEnough(candidate))
                {
                    found = true;
                    yield return AddSample(candidate);
                    break;
                }
            }

            // If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
            if (!found)
            {
                activeSamples[i] = activeSamples[activeSamples.Count - 1];
                activeSamples.RemoveAt(activeSamples.Count - 1);
            }
        }
    }

    private bool IsFarEnough(Vector2 sample)
    {
        var pos = getGridPosition(sample);

        int xmin = Mathf.Max(pos.x - 2, 0);
        int ymin = Mathf.Max(pos.y - 2, 0);

        int xmax = Mathf.Min(pos.x + 2, grid.Width - 1);
        int ymax = Mathf.Min(pos.y + 2, grid.Height- 1);

        for (int y = ymin; y <= ymax; y++)
        {
            for (int x = xmin; x <= xmax; x++)
            {
                Vector2 s = grid.GetValue(x, y);
                if (s != Vector2.zero)
                {
                    var d = (s - sample).magnitude;
                    if (d< (radius))
                        return false;
                }
            }
        }

        return true;

        // Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
        // to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
        // and we might end up with another sample too close from (0, 0). This is a very minor issue.
    }

    /// Adds the sample to the active samples queue and the grid before returning it
    private Vector2 AddSample(Vector2 sample)
    {
        activeSamples.Add(sample);
        var pos = getGridPosition(sample);
        //grid[pos.x, pos.y] = sample;
        grid.AddValue(pos.x, pos.y,sample);
        return sample;
    }

    private Vector2Int getGridPosition(Vector2 sample)
    {
        var x = (int)(sample.x / cellSize);
        var y = (int)(sample.y / cellSize);
        return new Vector2Int(x,y);
    }
}