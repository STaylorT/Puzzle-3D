using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPlatform : MonoBehaviour
{
    // A reference to the ball's RigitBody component.
    private Rigidbody body;

    private static GameObject finishPlatform;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        finishPlatform = this.gameObject;
        // finishPlatform.SetActive(false);
            
    }

    public static void makeVisible(){
        print("setting active..");
        finishPlatform.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
