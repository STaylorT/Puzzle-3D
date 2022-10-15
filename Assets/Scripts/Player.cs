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

public class Player : MonoBehaviour
{
    // The speed of the ball.
    public float Speed = 12;
    public float Acceleration = 2;
    public float offsetTime = 5f; // time for regular cubes to respawn
    
    public bool facingFront = true;

    public static int NUM_CUBES = Cube.TOTAL_CUBES;

    private GameObject[] cubeArr = new GameObject[NUM_CUBES]; 
    private float[] cubeTimers = new float[NUM_CUBES]; // keeping track of how long each cube has been disabled

    // A reference to the ball's RigitBody component.
    private Rigidbody body;

    private float timer;

    public GameObject area2;

    public int currLevel = 0;

    public Status playerStatus;

    public Camera mainCamera;
    public Camera povCamera;
    public Camera birdEyeCamera;

    public Light spotLight1;
    public Light spotLight2;

    public UIController UIController;

    // Start is called before the first frame update.
    void Start()
    {
        playerStatus = Status.BeginGame;
        UIController.handleEvent(playerStatus);
        body = GetComponent<Rigidbody>();
        
        area2.SetActive(false);
        spotLight1.enabled = true;
        spotLight2.enabled = false;

        timer = 0f;
    }

    // FixedUpdate is called before each step in the physics engine.
    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= 5){
            playerStatus = Status.Playing;
            UIController.handleEvent(playerStatus);
            Vector3 currVel = body.velocity;
            float horizInput = facingFront ? Input.GetAxis("Horizontal") : -Input.GetAxis("Horizontal");
            float verticalInput = facingFront ? Input.GetAxis("Vertical") : -Input.GetAxis("Vertical")  ;
            

            if (birdEyeCamera.enabled){ // if birdeye perspective, alter controls to keep intuitive
                float temp =-horizInput;
                horizInput = verticalInput;
                verticalInput = temp;
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
        } 
        else if (other.gameObject.CompareTag("Stage1CompleteTrigger"))
        {
            beginStage2();
        }
        else if (other.gameObject.CompareTag("FinishPlane"))
        {
            levelComplete();
        }
    }

    private void beginStage2()
    {
        GameObject.Find("Area1").SetActive(false);
        spotLight1.enabled = false;
        spotLight2.enabled = true;
        facingFront = false;
        area2.SetActive(true);
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
        playerStatus = Status.Playing;
        UIController.handleEvent(playerStatus);
        timer = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void gameWon()
    {
        playerStatus = Status.GameWon;
    }

    private void levelComplete()
    {
        playerStatus = Status.LevelComplete;
        UIController.handleEvent(playerStatus);
    }

    public void nextLevel()
    {
        playerStatus = Status.Playing;
        UIController.handleEvent(playerStatus);
        currLevel++;
        SceneManager.LoadScene(currLevel);
    }
}