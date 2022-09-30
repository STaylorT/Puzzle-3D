using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // The speed of the ball.
    public float Speed = 10;

    public TextMeshProUGUI countText;

    public GameObject winTextObject;

    public float offsetTime = 7f;

    private GameObject[] cubeArr = new GameObject[12];
    private float[] cubeTimers = new float[12];

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

        for (int i = 0; i < 12 ; i++){
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
        if (other.gameObject.CompareTag("PickUp"))
        {
            int cubeNum = other.gameObject.GetComponent<RotateAroundZ>().cubeNum;
            // Debug.Log(cubeNum);
            cubeArr[cubeNum] = other.gameObject;
            cubeTimers[cubeNum] = .00001f;
            other.gameObject.SetActive(false);

        }   
    }
}