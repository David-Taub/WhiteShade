using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

public class MapController : MonoBehaviour {
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    private List<GameObject>[][] map;
    public int Columns = 50;
    public int Rows = 50;
    private const int NumOfPlayers = 30;
    public GridGraph Graph;
    private const float WallRatio = 0.3f;
    private const float NodeDistance = 1.0f;
    private readonly List<Vector3> _freeLocations = new List<Vector3>();


    public void BoardSetup()
    {
        var boardHolder = new GameObject("Board").transform;
        Graph = AstarPath.active.data.AddGraph(typeof(GridGraph)) as GridGraph;
        Debug.Assert(Graph != null, "Graph != null");
        Graph.SetDimensions(Columns, Rows, NodeDistance);
        Graph.center = new Vector3(Columns / 2.0f - 0.5f, Rows / 2.0f - 0.5f, 0);
        Graph.rotation = new Vector3(-90, -90, 90);
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {

            for (var x = 0; x < Columns; x++)
            {
                for (var y = 0; y < Rows; y++)
                {
                    GameObject toInstantiate = FloorPrefab;

                    //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                    GridNode node = (GridNode)Graph.GetNode(x, y);
                    node.position = new Int3(x, y, 0);
                    if (IsForbiddenPosition(x, y) || (Random.value < WallRatio))
                    {
                        toInstantiate = WallPrefab;
                        node.Walkable = false;
                    }
                    else
                    {
                        _freeLocations.Add(new Vector3(x, y, 0));
                        node.Walkable = true;
                    }
                    var instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                    AddObjectPos(instance);
                    instance.transform.SetParent(boardHolder);
                }
            }
            Graph.GetNodes(node => Graph.CalculateConnections((GridNodeBase)node));

        }));
        AstarPath.active.Scan();
    }

    private bool IsForbiddenPosition(int x, int y)
    {
        return (x == 0 || x == Columns - 1 || y == 0 || y == Rows - 1);
    }

    void Start() 
    {
        BoardSetup();
        PlacePlayers();
        PlaceEnemies();
    }

    public void AddObjectPos(GameObject obj)
    {
        map[Mathf.RoundToInt(obj.transform.position.x)][Mathf.RoundToInt(obj.transform.position.x)].Add(obj);
    }

    public void UpdateObjectPos(GameObject obj, Vector3 oldPosition)
    {
        map[Mathf.RoundToInt(obj.transform.position.x)][Mathf.RoundToInt(obj.transform.position.x)].Remove(obj);
        AddObjectPos(obj);
    }

    private void PlacePlayers()
    {
        Transform playerHolder = new GameObject("Players").transform;
        for (int i = 0; i < NumOfPlayers; i++)
        {
            Vector3 pos = PopRandomPosition();
            GameObject player = Instantiate(PlayerPrefab, pos, Quaternion.identity);
            AddObjectPos(player);
            player.transform.SetParent(playerHolder);
        }
    }

    private void PlaceEnemies()
    {
        Transform enemiesHolder = new GameObject("Enemies").transform;
        for (int i = 0; i < NumOfPlayers; i++)
        {
            Vector3 pos = PopRandomPosition();
            GameObject enemy = Instantiate(EnemyPrefab, pos, Quaternion.identity);
            AddObjectPos(enemy);
            enemy.transform.SetParent(enemiesHolder);
        }
    }


    Vector3 PopRandomPosition()
    {
        int randomIndex = Random.Range(0, _freeLocations.Count);
        Vector3 randomPosition = _freeLocations[randomIndex];
        _freeLocations.RemoveAt(randomIndex);
        return randomPosition;
    }
}
