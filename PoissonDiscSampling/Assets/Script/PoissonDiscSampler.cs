using System.Collections.Generic;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
public struct PoissonDiscSampler
{
    private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.

    public float Radius; 
    public float Width;
    public float Height;
    public float CellSize;
    
    public List<float2> ActiveSamples;
    public List<float2> Output;

    private LinearGrid _linearGrid;

    public void CreateSamples()
    {
        var random= new Random(1);

        _linearGrid = new LinearGrid()
        {
            Width = (int)math.ceil(Width / CellSize),
            Height = (int)math.ceil(Height / CellSize)
        };

        _linearGrid.CreateGrid();

        // First sample is chosen randomly
        var firstSample = new float2(random.NextFloat()* Width, random.NextFloat() * Height);
        AddSample(firstSample);

        while (ActiveSamples.Count > 0)
        {

            // Pick a Random active sample
            var index = (int)random.NextFloat() * ActiveSamples.Count;
            var sample = ActiveSamples[index];

            // Try `k` Random candidates between [radius, 2 * radius] from that sample.
            bool found = false;
            for (int j = 0; j < k; ++j)
            {
                var randomDirection = new float2(random.NextFloat(-1.0f, 1.0f), random.NextFloat(-1.0f, 1.0f));
                
                var randomMagnitude = random.NextFloat(Radius, Radius * 2);
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
                ActiveSamples[index] = ActiveSamples[ActiveSamples.Count - 1];
                ActiveSamples.RemoveAt(ActiveSamples.Count - 1);
            }
        }
    }

    private bool IsFarEnough(float2 sample)
    {
        var pos = GetGridPosition(sample);

        int xmin = math.max(pos.x - 2, 0);
        int ymin = math.max(pos.y - 2, 0);

        int xmax = math.min(pos.x + 2, _linearGrid.Width - 1);
        int ymax = math.min(pos.y + 2, _linearGrid.Height- 1);

        for (int y = ymin; y <= ymax; y++)
        {
            for (int x = xmin; x <= xmax; x++)
            {
                var s = _linearGrid.GetValue(x, y);
                if (s.x != float.MinValue)
                {
                    
                    if (Distance(s, sample) < (Radius))
                        return false;
                }
            }
        }

        return true;
    }

    /// Adds the sample to the active samples queue and the _linearGrid 
    private void AddSample(float2 sample)
    {
        ActiveSamples.Add(sample);
        var pos = GetGridPosition(sample);
        _linearGrid.AddValue(pos.x, pos.y,sample);
        Output.Add(sample);
    }

    private int2 GetGridPosition(float2 sample)
    {
        var x = (int)(sample.x / CellSize);
        var y = (int)(sample.y / CellSize);
        return new int2(x,y);
    }

    private bool Contains(float2 candidate)
    {
        return (candidate.x >= 0 && candidate.x < Width
              &&
              candidate.y >= 0 && candidate.y < Height);
    }

    private float Distance(float2 a, float2 b)
    {
        return math.sqrt(math.pow((a.x - b.x), 2) + math.pow((a.y - b.y), 2));
    }
}