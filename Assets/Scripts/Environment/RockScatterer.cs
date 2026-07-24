using UnityEngine;

public class RockScatterer : MonoBehaviour
{
    [Header("Rock Prefabs")]
    [SerializeField] private GameObject[] rockPrefabs;

    [Header("Scatter Settings")]
    [SerializeField] private int rockCount = 20;
    [SerializeField] private float scatterRadius = 80f;
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float minHeightOffset = -0.3f;
    [SerializeField] private float maxHeightOffset = 0.2f;

    [Header("Exclusion Zones")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private float exclusionRadius = 8f;
    [SerializeField] private Transform[] binPositions;
    [SerializeField] private float binExclusionRadius = 4f;

    [Header("Options")]
    [SerializeField] private bool scatterOnStart = true;

    private Terrain terrain;

    private void Start()
    {
        if (scatterOnStart)
            ScatterRocks();
    }

    public void ScatterRocks()
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0) return;

        terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        TerrainData td = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        float terrainWidth = td.size.x;
        float terrainLength = td.size.z;

        for (int i = 0; i < rockCount; i++)
        {
            Vector3 randomPos = GetRandomPosition(terrainPos, terrainWidth, terrainLength);

            if (IsExcluded(randomPos)) continue;

            float height = terrain.SampleHeight(randomPos) + terrainPos.y;
            randomPos.y = height + Random.Range(minHeightOffset, maxHeightOffset);

            GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];
            GameObject rock = Instantiate(prefab, randomPos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));

            float scale = Random.Range(minScale, maxScale);
            rock.transform.localScale = Vector3.one * scale;

            rock.transform.SetParent(transform);
            rock.layer = LayerMask.NameToLayer("Environment");
        }
    }

    private Vector3 GetRandomPosition(Vector3 terrainPos, float width, float length)
    {
        float x = terrainPos.x + Random.Range(0f, width);
        float z = terrainPos.z + Random.Range(0f, length);
        return new Vector3(x, 0f, z);
    }

    private bool IsExcluded(Vector3 pos)
    {
        if (playerSpawn != null && Vector3.Distance(pos, playerSpawn.position) < exclusionRadius)
            return true;

        if (binPositions != null)
        {
            foreach (var bin in binPositions)
            {
                if (bin != null && Vector3.Distance(pos, bin.position) < binExclusionRadius)
                    return true;
            }
        }

        return false;
    }
}
