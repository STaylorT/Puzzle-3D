using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundZ : MonoBehaviour
{
	// The speed at which the coin rotates.
	public float Speed = 10;

    // A reference to the cube's RigitBody component.
    private Rigidbody body;

	private bool isSpecialCube = false;

	public int cubeNum = -1;

    public static int specialCube = -1;

	public static int numCubesStarted = 0;

    // Start is called before the first frame update.
    void Start()
    {
		body = GetComponent<Rigidbody>();

		if (numCubesStarted == 0){
			int randNum = Random.Range(0,11);
			specialCube = randNum;
		}
        body = GetComponent<Rigidbody>();
		
		if (cubeNum == -1){
			cubeNum = numCubesStarted;
		}
		if (specialCube == cubeNum){
			isSpecialCube = true;
		}
		numCubesStarted++;

    }

	// FixedUpdate is called before each step in the physics engine.
	void FixedUpdate()
	{	// Rotate the object by 15 (x axis), 30 (y axis) and 45 (z axis) times deltaTime
		// to make the rotation per second rather than per frame.
		
		if (!isSpecialCube){
			transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
		} else {
			transform.Rotate(new Vector3(-30, -40, -60) * Time.deltaTime*3);
		}
	}
	
}