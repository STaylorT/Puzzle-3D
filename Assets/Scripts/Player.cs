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
    Playing,
    GameOver,
    GameWon,
}

public class Player : MonoBehaviour
{
    private PlayerStats playerStats = new PlayerStats();
    // The speed of the ball.
    public float Speed = 12;

    public bool facingFront = true;

    public float Acceleration = 2;

    public float offsetTime = 5f; // time for regular cubes to respawn
    
    public static int NUM_CUBES = Cube.TOTAL_CUBES;

    private GameObject[] cubeArr = new GameObject[NUM_CUBES]; 
    private float[] cubeTimers = new float[NUM_CUBES]; // keeping track of how long each cube has been disabled

    // A reference to the ball's RigitBody component.
    private Rigidbody body;

    private float timer;

    public GameObject gameOverText;

    public GameObject blackOutSquare;

    public GameObject wonText;

    public GameObject area2;

    public Button resetButton;

    public Button nextLevelButton;

    public int currLevel = 0;

    public Status playerStatus;

    public Camera mainCamera;

    public Camera povCamera;

    public Camera birdEyeCamera;

    public Light spotLight;

    // Start is called before the first frame update.
    void Start()
    {
        // RenderSettings.ambientLight = Color.black;
        body = GetComponent<Rigidbody>();
        gameOverText.SetActive(false);
        wonText.SetActive(false);
        resetButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        area2.SetActive(false);

        StartCoroutine(FadeBlackOutSquare(false));
        timer = 0f;
        Button btn = resetButton.GetComponent<Button>();
		btn.onClick.AddListener(resetLevel);
    }

    // FixedUpdate is called before each step in the physics engine.
    private void FixedUpdate()
    {
        // UI = gameObject.GetComponent(typeof(UIController)) as UIController;
        timer += Time.deltaTime;
        if (timer >= .1){
            Vector3 currVel = body.velocity;
            float horizInput = facingFront ? Input.GetAxis("Horizontal") : -Input.GetAxis("Horizontal");
            float verticalInput = facingFront ? Input.GetAxis("Vertical") : -Input.GetAxis("Vertical")  ;
            

            if (birdEyeCamera.enabled){ // intuitive controls on camera rotate
                float temp = horizInput;
                horizInput = -verticalInput;
                verticalInput = temp;
            } 
            bool sameHorizDirection = currVel.x * horizInput > 0;
            bool sameVerticalDirection = currVel.z * verticalInput > 0;
            
            float horizontalForce =  sameHorizDirection ? horizInput * Speed : horizInput * 4 * Speed;
            float verticalForce = sameVerticalDirection ? verticalInput * Speed : verticalInput * 4 * Speed;

            if (Math.Abs(currVel.z) > Math.Abs(currVel.x) && verticalInput == 0 && horizInput != 0){
                horizontalForce *= 2;
            } else if ( Math.Abs(currVel.x) > Math.Abs(currVel.z) && horizInput == 0 && verticalInput != 0){
                verticalForce *= 2;
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
        if (other.gameObject.CompareTag("PickUp") || other.gameObject.CompareTag("ArrowPickUp") ) // if collision with cube
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
            }
            
        } 
        else if (other.gameObject.CompareTag("FallThruPlane"))
        {
            if (playerStatus == Status.Playing)
            {
                gameOver();
            }
        } 
        else if (other.gameObject.CompareTag("Stage1CompleteTrigger"))
        {
            beginStage2();
            // FinishPlatform.makeVisible();
            // make the finalPlat visible.
        }
        else if (other.gameObject.CompareTag("FinishPlane"))
        {
            finishLevel();
        }
    }

    private void beginStage2()
    {
        GameObject.Find("Area1").SetActive(false);
        facingFront = false;
        area2.SetActive(true);
        // spotLight.transform.position += new Vector3(0, -20, 0);
        // mainCamera.transform.position += new Vector3(0, 0, 10); 
        mainCamera.transform.Rotate(0, 180, 0);
        povCamera.transform.Rotate(0, 180, 0);

    }

    private void gameOver()
    {
        playerStatus = Status.GameOver;
        gameOverText.SetActive(true);
        repositionButton(resetButton, true);
        resetButton.gameObject.SetActive(true);
        StartCoroutine(FadeBlackOutSquare());
    }

    private void resetLevel()
    {
        BoundaryWall.reset();
        resetButton.gameObject.SetActive(false);
        timer = 0f;
        Cube.reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void finishLevel()
    {
        playerStatus = Status.GameWon;
        StartCoroutine(FadeBlackOutSquare());
        wonText.SetActive(true);
        repositionButton(resetButton, false);
        resetButton.gameObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);

    }

    private void nextLevel()
    {
        playerStatus = Status.Playing;
        currLevel++;
        SceneManager.LoadScene(currLevel);
        wonText.SetActive(false);
    }

    public IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, int fadeSpeed = 10)
    {
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        if (fadeToBlack)
        {
            while (blackOutSquare.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed/10 * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        } else 
        {
            while (blackOutSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed/10 * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        yield return new WaitForEndOfFrame();
    }

    private void repositionButton(Button button, bool singleButton) {
        RectTransform buttonTransform = resetButton.gameObject.GetComponent<RectTransform>();
        if (singleButton){
            buttonTransform.anchorMin = new Vector2(.5f, .5f);
            buttonTransform.anchorMax = new Vector2(.5f, .5f);
        } else 
        {
            buttonTransform.anchorMin = new Vector2(0.25f, .5f);
            buttonTransform.anchorMax = new Vector2(0.25f, .5f);
        }
        
    }
}