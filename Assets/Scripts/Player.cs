using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using BW = BoundaryWall;

public enum Status
{
    BeginGame,
    Playing,
    ResetLevel,
    NextLevelLoad,
    LevelComplete,
    GameOver,
    GameWon,
}

public enum Level
{
    level1,
    level2,
}

public class Player : MonoBehaviour
{
    // The speed of the ball.
    public float Speed = 12;
    public float Acceleration = 2;
    public float offsetTime = 5f; // time for regular cubes to respawn
    
    public bool facingFront = true;

    public static int NUM_CUBES = Cube.TOTAL_CUBES;

    public static int NUM_LEVELS = 2;

    private GameObject[] cubeArr = new GameObject[NUM_CUBES]; 
    private float[] cubeTimers = new float[NUM_CUBES]; // keeping track of how long each cube has been disabled

    // A reference to the ball's RigitBody component.
    private Rigidbody body;

    private float timer;

    public GameObject barrier;
    public GameObject area2;
    public GameObject area1;
    public GameObject area0;

    public Level currentLevel = Level.level1;

    public Status playerStatus;

    public Camera mainCamera;
    public Camera povCamera;
    public Camera birdEyeCamera;

    public Light spotLight1;
    public Light spotLight2;
    public Light mainLight;

    public UIController UIController;

    // Start is called before the first frame update.
    void Start()
    {
        playerStatus = Status.BeginGame;
        body = GetComponent<Rigidbody>();
        // area2.SetActive(false);
        spotLight1.enabled = true;
        spotLight2.enabled = false;
        area0.SetActive(true);
        area1.SetActive(false);
        area2.SetActive(false);
        UIController.handleEvent(playerStatus);
    
        timer = 0f;
    }

    // FixedUpdate is called before each step in the physics engine.
    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (playerStatus == Status.Playing){
            Vector3 currVel = body.velocity;
            float horizInput = facingFront ? Input.GetAxis("Horizontal") : -Input.GetAxis("Horizontal");
            float verticalInput = facingFront ? Input.GetAxis("Vertical") : -Input.GetAxis("Vertical")  ;

            if (birdEyeCamera.enabled){ // if birdeye perspective, alter controls to keep intuitive
                if (facingFront){
                    float temp = horizInput;
                    horizInput = -verticalInput;
                    verticalInput = temp;
                } else {
                    float temp = -horizInput;
                    horizInput = verticalInput;
                    verticalInput = temp;
                }
            }
            // check if player is adding force in the direction they're currently travelling in
            bool sameHorizDirection = currVel.x * horizInput > 0;
            bool sameVerticalDirection = currVel.z * verticalInput > 0;
            
            // add more force when user is switching directions (to stop, turn faster) 
            float horizontalForce =  sameHorizDirection ? horizInput * Speed : horizInput * 4 * Speed;
            float verticalForce = sameVerticalDirection ? verticalInput * Speed : verticalInput * 4 * Speed;

            // prioritize user's current input with respect to opposite directions.
            // if user trying to go left while currently going forward/backward, slow down fwd/bwd motion, increase left force.
            if (Math.Abs(currVel.z) > Math.Abs(currVel.x) && verticalInput == 0 && horizInput != 0){
                verticalForce *= .75f;
                horizontalForce *= 2f;
            } else if ( Math.Abs(currVel.x) > Math.Abs(currVel.z) && horizInput == 0 && verticalInput != 0){
                verticalForce *= 2f;
                horizontalForce *= .75f;
            }
            body.AddForce(new Vector3(horizontalForce, 0, verticalForce));
        }

        for (int i = 0; i < NUM_CUBES  ; i++){
            if (cubeTimers[i] >= offsetTime){
                cubeArr[i].SetActive(true);
                cubeTimers[i] = 0f;
            }
            if (cubeTimers[i] > 0f){
                cubeTimers[i] += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("FallThruPlane"))
        {
            if (playerStatus == Status.Playing)
            {
                gameOver();
            }
        } else if (other.gameObject.CompareTag("PickUp") || other.gameObject.CompareTag("ArrowPickUp") ) // if collision with cube
        {
            int cubeId = other.gameObject.GetComponent<Cube>().cubeID;
            bool isSpecialCube = other.gameObject.GetComponent<Cube>().isSpecialCube;

            // deactive cube
            other.gameObject.SetActive(false);
            cubeArr[cubeId] = other.gameObject;
            
            if (!isSpecialCube){ // start counting for respawn of non-special cubes
                cubeTimers[cubeId] = .00001f;
            } else {
                BoundaryWall.removeKinematics();
                barrier.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("FinishLevel1"))
        {
            startLevel2();
        }
        else if (other.gameObject.CompareTag("Stage1CompleteTrigger"))
        {
            beginStage2Transition();
        }
        else if (other.gameObject.CompareTag("OnStage2Trigger"))
        {
            beginStage2();
        }
        else if (other.gameObject.CompareTag("LevelCompleteTrigger"))
        {
            levelComplete();
        } 
    }

    public void beginLevel()
    {
        playerStatus = Status.Playing;
        UIController.handleEvent(playerStatus);
    }

    public void startLevel2(){
        playerStatus = Status.LevelComplete;
        UIController.handleEvent(playerStatus);
        this.gameObject.transform.position = new Vector3(0, 2, -7);
        area1.SetActive(true);
        area0.SetActive(false);
    }

    private void beginStage2Transition()
    { // turn off all lights, let player fall to second area
        spotLight1.enabled = false;
        spotLight2.enabled = false;
        mainLight.enabled = false;
        handleCameraChanges();
    }

    private void beginStage2()
    {
        GameObject.Find("Area1").SetActive(false);
        area2.SetActive(true);
        spotLight2.enabled = true;
        mainLight.enabled = true;
    }

    private void handleCameraChanges()
    {
        mainCamera.GetComponent<CameraController>().flipped = true;
        facingFront = false;
        mainCamera.transform.position += new Vector3(0, 0, 10); 
        mainCamera.transform.Rotate(60, 180, 0);
        povCamera.transform.Rotate(0, 180, 0);
    }

    private void gameOver()
    {
        playerStatus = Status.GameOver;
        UIController.handleEvent(playerStatus);
    }

    public void resetLevel()
    {
        BoundaryWall.reset();
        this.gameObject.transform.position = new Vector3(0, 25f, 0);
        barrier.SetActive(true);
        playerStatus = Status.Playing;
        UIController.handleEvent(playerStatus);
        timer = 0f;
        
    }

    private void gameWon()
    {
        playerStatus = Status.GameWon;
    }

    private void levelComplete()
    {
        area1.SetActive(true);
        area0.SetActive(false);
        playerStatus = Status.LevelComplete;
        UIController.handleEvent(playerStatus);
        
    }

    public void nextLevel()
    {
        if (currentLevel == Level.level2){
            playerStatus = Status.GameWon;
        } else if (currentLevel == Level.level1){
            playerStatus = Status.NextLevelLoad;
        }
        currentLevel = Level.level2;
        UIController.handleEvent(playerStatus);
    }
}