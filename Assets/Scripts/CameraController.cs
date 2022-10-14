using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public const int NUMBER_OF_CAMERAS = 3;
	// A reference to the player object.
	public GameObject Player;
		
	// The distance between the camera and the player object.
	private Vector3 mainOffset;
	private Vector3 povOffset;

	private Camera[] sceneCameras = new Camera[NUMBER_OF_CAMERAS];
	private int currCamera = 0;


	// Start is called before the first frame update.
	void Start()
	{
		Camera.GetAllCameras(sceneCameras);
		Player = GameObject.Find("Player");
		calculateOffsets();
		
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.C)) {
			currCamera = (currCamera + 1) % 3;
     }
	 for (int i = 0 ; i < NUMBER_OF_CAMERAS; i++) {
		if (i == currCamera){
			sceneCameras[i].enabled = true;
		} else{
			sceneCameras[i].enabled = false;
		}
	 }
	}
	// LateUpdate is called after Update but before rendering the frame.
	void LateUpdate()
	{
		if (sceneCameras[currCamera].name == "Main Camera"){
			sceneCameras[currCamera].transform.position = Player.transform.position + mainOffset;
		} else if (sceneCameras[currCamera].name == "povCamera") {
			sceneCameras[currCamera].transform.position = Player.transform.position + povOffset;
		}
	}

	private void calculateOffsets()
	{
		GameObject mainCam = GameObject.Find("Main Camera");
		GameObject povCam = GameObject.Find("povCamera");
		mainOffset = mainCam.transform.position - Player.transform.position;
		povOffset = povCam.transform.position - Player.transform.position;

	}
	
}