using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
// using BW = BoundaryWall;

public class Player : MonoBehaviour
{
    // The speed of the ball.
    public float Speed = 10;

    public TextMeshProUGUI countText;

    public GameObject winTextObject;

    public float offsetTime = 5f; // time for regular cubes to respawn
    
    public static int NUM_CUBES = Cube.TOTAL_CUBES;

    private GameObject[] cubeArr = new GameObject[NUM_CUBES]; 
    private float[] cubeTimers = new float[NUM_CUBES]; // keeping track of how long each cube has been disabled

    // A reference to the ball's RigitBody component.
    private Rigidbody body;

    // Start is called before the first frame update.
    void Start()
    {
        body = GetComponent<Rigidbody>();
        // winTextObject.SetActive(false);
        // SetCountText();
    }

    // void SetCountText()
    // {
    //     countText.text = "Count: " + count.ToString();
    //     if (count >= 12)
    //     {
    //         winTextObject.SetActive(true);
    //     }
    // }

    // FixedUpdate is called before each step in the physics engine.
    private void FixedUpdate()
    {
        float horizontalForce = Input.GetAxis("Horizontal") * Speed;
        float verticalForce = Input.GetAxis("Vertical") * Speed;
        body.AddForce(new Vector3(horizontalForce, 0, verticalForce));

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
            resetLevel();
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

    private void resetLevel()
    {
        BoundaryWall.reset();
        Cube.reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void finishLevel()
    {
        print("Congrats, you finished!");
    }
}