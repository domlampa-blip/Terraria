using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [Header("Settings")]
    public int width = 100;
    public int height = 50;
    public int groundHeight = 20;
    public float smoothness = 10f;
    public int seat; // Seed for randomness

    [Header("References")]
    public Tilemap groundTilemap;
    public TileBase dirtTile;
    public TileBase grassTile;
    public TileBase stoneTile;

    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        groundTilemap.ClearAllTiles();
        seat = Random.Range(0, 10000);

        for (int x = 0; x < width; x++)
        {
            // Calculate height using Perlin Noise
            int h = groundHeight + Mathf.RoundToInt(Mathf.PerlinNoise((x + seat) / smoothness, seat) * 10);

            for (int y = 0; y < height; y++)
            {
                if (y < h)
                {
                    TileBase selectedTile;

                    // Top layer is grass
                    if (y == h - 1)
                    {
                        selectedTile = grassTile;
                    }
                    // Layer below grass is dirt
                    else if (y > h - 5)
                    {
                        selectedTile = dirtTile;
                    }
                    // Deep layer is stone
                    else
                    {
                        selectedTile = stoneTile;
                    }

                    groundTilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
                }
            }
        }
        
        // Add borders
        AddBorders();
    }

    void AddBorders()
    {
        // Simple invisible borders or stone walls at left/right/bottom
        for (int y = 0; y < height * 1.5; y++)
        {
            groundTilemap.SetTile(new Vector3Int(-1, y, 0), stoneTile);
            groundTilemap.SetTile(new Vector3Int(width, y, 0), stoneTile);
        }
    }
}
