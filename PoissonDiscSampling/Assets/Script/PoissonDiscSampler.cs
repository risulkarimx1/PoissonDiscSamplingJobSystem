using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public struct PoissonDiscSampler:IJob
{
    private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.

    public float Radius; 
    public float Width;
    public float Height;
    public float CellSize;
    public uint RandomSeed;

    public int GridWidth;
    public int GridHeight;
    public NativeArray<float2> GridArray;

    public NativeList<float2> ActiveSamples;
    public NativeList<float2> Result;

    public void Execute()
    {
        var random = new Random(RandomSeed);
        CreateGrid();

        // First sample is chosen randomly
        var firstSample = new float2(random.NextFloat() * Width, random.NextFloat() * Height);
        AddSample(firstSample);

        while (ActiveSamples.Length > 0)
        {

            // Pick a Random active sample
            var index = (int)random.NextFloat() * ActiveSamples.Length;
            var sample = ActiveSamples[index];

            // Try `k` Random candidates between [radius, 2 * radius] from that sample.
            var found = false;
            for (var j = 0; j < k; j++)
            {
                var randomDirection = new float2(random.NextFloat(-1.0f, 1.0f), random.NextFloat(-1.0f, 1.0f));
                var randomMagnitude = random.NextFloat(Radius, Radius * 2);
                var offset = randomDirection * randomMagnitude;
                var candidate = sample + offset;

                // Accept candidates if it's inside the Width and Height and farther than 2 * radius to any existing sample.
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
                ActiveSamples.RemoveAtSwapBack(index);
            }
        }
    }

    private bool IsFarEnough(float2 sample)
    {
        var pos = GetGridPosition(sample);

        var xmin = math.max(pos.x - 2, 0);
        var ymin = math.max(pos.y - 2, 0);

        var xmax = math.min(pos.x + 2, GridWidth - 1);
        var ymax = math.min(pos.y + 2, GridHeight- 1);

        for (int y = ymin; y <= ymax; y++)
        {
            for (int x = xmin; x <= xmax; x++)
            {
                var s = GetValueFromGrid(x, y);
                if (s.x != float.MinValue)
                {
                    
                    if (Distance(s, sample) < (Radius))
                        return false;
                }
            }
        }

        return true;
    }

    /// Adds the sample to the active samples queue and the gridArray 
    private void AddSample(float2 sample)
    {
        ActiveSamples.Add(sample);
        var pos = GetGridPosition(sample);
        AddValueToGrid(pos.x, pos.y,sample);
        Result.Add(sample);
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

    // Grid codes

    private void CreateGrid()
    {
        for (var i = 0; i < GridArray.Length; i++)
        {
            GridArray[i] = new float2(float.MinValue, float.MinValue);
        }
    }

    public void AddValueToGrid(int col, int row, float2 value)
    {
        var index = GetLinearIndex(col, row);
        GridArray[index] = value;
    }
    private int GetLinearIndex(int col, int row)
    {
        return col + row * GridWidth;
    }
    public float2 GetValueFromGrid(int col, int row)
    {
        return GridArray[GetLinearIndex(col, row)];
    }
}