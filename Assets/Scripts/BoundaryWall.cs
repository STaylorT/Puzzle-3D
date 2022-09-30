using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryWall : MonoBehaviour
{

    private static int TOTAL_WALLS = 4;

    private static int numWalls = 0;

    private int wallID = -1;

    // A reference to the ball's RigitBody component.
    private Rigidbody body;

    private static Rigidbody[] walls = new Rigidbody[TOTAL_WALLS];

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        if (wallID == -1){
            wallID = numWalls;
        }
        walls[wallID] = body;
        if (numWalls < (TOTAL_WALLS - 1)){
            numWalls++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void reset()
    {
        numWalls = 0;
    }

    public static void removeKinematics()
    {
        foreach (Rigidbody wall in walls)
        {
            wall.isKinematic = false;
        }
    }
}
