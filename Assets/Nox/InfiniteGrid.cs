using UnityEngine;

public class InfiniteGrid : MonoBehaviour
{
    public GameObject gridPrefab;
    public Transform player;
    public float gridSize = 100f;
    public float edgeThreshold = 30f;

    private Transform[] grids = new Transform[4];

    void Start()
    {
        if (!gridPrefab || !player)
        {
            Debug.LogError("Missing references in InfiniteGrid script.");
            return;
        }

        // Initialize the grid pieces around the player
        for (int i = 0; i < 4; i++)
        {
            grids[i] = Instantiate(gridPrefab).transform;
        }

        RearrangeGrids();
    }

    void Update()
    {
        // Calculate the boundaries of the current central grid
        Vector3 gridCenter = (grids[0].position + grids[3].position) * 0.5f;

        if (Mathf.Abs(player.position.x - gridCenter.x) > (gridSize - edgeThreshold))
        {
            RearrangeGrids();
        }

        if (Mathf.Abs(player.position.y - gridCenter.y) > (gridSize - edgeThreshold))
        {
            RearrangeGrids();
        }
    }

    void RearrangeGrids()
    {
        // Sort grids based on distance to player, so grids[0] and grids[3] are the closest
        System.Array.Sort(grids, (a, b) => Vector3.Distance(player.position, a.position).CompareTo(Vector3.Distance(player.position, b.position)));

        // Place the grids around the player
        Vector3 playerGridPos = new Vector3(Mathf.Floor(player.position.x / gridSize) * gridSize, Mathf.Floor(player.position.y / gridSize) * gridSize, 0);

        grids[0].position = playerGridPos;
        grids[1].position = playerGridPos + new Vector3(gridSize, 0, 0);
        grids[2].position = playerGridPos + new Vector3(0, gridSize, 0);
        grids[3].position = playerGridPos + new Vector3(gridSize, gridSize, 0);
    }
}
