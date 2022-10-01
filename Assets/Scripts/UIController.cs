using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject blackOutSquare;

    public GameObject mainText;

    private string prevState = "";

    private string currState = "";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeBlackOutSquare(false));
        // mainText.SetActive(false);
    }

    // void setMainText(string text)
    // {
    //     mainText.text = text;
    //     // if (count >= 12)
    //     // {
    //     //     winTextObject.SetActive(true);
    //     // }
    // }

    // Update is called once per frame
    void Update()
    {
        prevState = currState;
        // currState = Player.state;

        if (prevState != currState){
            print("not equal!");
            if (currState == "dead"){
                print("curr state is dead!");
                gameOver();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            print("starting co");
            StartCoroutine(FadeBlackOutSquare());
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            print("starting down co");
            StartCoroutine(FadeBlackOutSquare(false));
        }
        
    }

    public void gameOver()
    {
        // setMainText("Game Over");
        // mainText.SetActive(true);
        StartCoroutine(FadeBlackOutSquare());
    }

    public void nextLevel()
    {
        // setMainText("Level x");
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
