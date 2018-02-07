using Pathfinding;
using UnityEngine;
using System;

using System.Threading;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

public class MapCreator : MonoBehaviour {
    public GameObject floorTile;
    public GameObject wallTile;
    private Transform boardHolder;
    public int columns;
    public int rows;
    private AutoResetEvent autoEvent;
    public GridGraph graph;
    public float WALL_RATIO = 0.2f;
    public float nodeDistance = 1.0f;
    private List<Vector3> freeLocations = new List<Vector3>();
    public void BoardSetup()
    {
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    GameObject toInstantiate = floorTile;

                    //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                    GraphNode node = graph.GetNode(x, y);
                    node.position = new Int3(x, y, 0);
                    if (isForbiddenPosition(x, y) || (Random.value < WALL_RATIO))
                    {
                        toInstantiate = wallTile;
                        node.Walkable = false;
                    }
                    else
                    {
                        freeLocations.Add(new Vector3(x, y, 0));
                        node.Walkable = true;
                    }

                    //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                    GameObject instance =
                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                    //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                    instance.transform.SetParent(boardHolder);
                }
            }
            graph.GetNodes(node => graph.CalculateConnections((GridNodeBase)node));
            ctx.QueueFloodFill();
            AstarPath.active.Scan();
        }));
    }

    private Boolean isForbiddenPosition(int x, int y)
    {
        return (x == 0 || x == columns - 1 || y == 0 || y == rows - 1);
    }

    // Use this for initialization
    void Start() {
        boardHolder = new GameObject("Board").transform;
        GraphSetup();
        BoardSetup();
        Vector3 pos = PopRandomPosition();
        //GameObject Player = GameObject.FindGameObjectWithTag("Player");
        GameObject player = GameObject.Find("Player");
        player.transform.position = new Vector3(pos.x, pos.y, 0f);
    }

    Vector3 PopRandomPosition()
    {
        int randomIndex = Random.Range(0, freeLocations.Count);
        Vector3 randomPosition = freeLocations[randomIndex];
        freeLocations.RemoveAt(randomIndex);
        return randomPosition;
    }
    // Update is called once per frame
    void Update () {
		
	}
    void GraphSetup()
    {
        graph = AstarPath.active.data.AddGraph(typeof(GridGraph)) as GridGraph;
        graph.SetDimensions(columns, rows, nodeDistance);
        AstarPath.active.Scan();
       
    }
}
