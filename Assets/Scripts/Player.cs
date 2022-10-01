using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using BW = BoundaryWall;

public class Player : MonoBehaviour
{
    private PlayerStats playerStats = new PlayerStats();
    // The speed of the ball.
    public float Speed = 10;

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

    public Button resetButton;

    // Start is called before the first frame update.
    void Start()
    {

        body = GetComponent<Rigidbody>();
        gameOverText.SetActive(false);
        resetButton.gameObject.SetActive(false);
        wonText.SetActive(false);
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
        if (timer >= 3){
            float horizontalForce = Input.GetAxis("Horizontal") * Speed;
            float verticalForce = Input.GetAxis("Vertical") * Speed;
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
            gameOver();
        } 
        else if (other.gameObject.CompareTag("FinalPlatformGate"))
        {
            FinishPlatform.makeVisible();
            // make the finalPlat visible.
        }
        else if (other.gameObject.CompareTag("FinishPlane"))
        {
            finishLevel();
        }
    }

    private void gameOver()
    {
        gameOverText.SetActive(true);
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
        playerStats.addBestTime(timer);
        wonText.SetActive(true);
        StartCoroutine(FadeBlackOutSquare());
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
}