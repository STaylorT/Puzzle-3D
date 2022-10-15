using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject player;

    public GameObject gameOverText;

    public GameObject blackOutSquare;

    public GameObject wonText;

    public GameObject nextLevelText;

    public GameObject area2;

    public Button resetButton;

    public Button nextLevelButton;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeBlackOutSquare(false));
        gameOverText.SetActive(false);
        wonText.SetActive(false);
        resetButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        nextLevelText.SetActive(false);
        resetButton.onClick.AddListener(player.GetComponent<Player>().resetLevel);
        nextLevelButton.onClick.AddListener(player.GetComponent<Player>().nextLevel); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void handleEvent(Status playerStatus)
    {
        switch (playerStatus){
            case Status.BeginGame:
                StartCoroutine(FadeBlackOutSquare(true));
                StartCoroutine(FadeBlackOutSquare(false));
                break;
            case Status.Playing:
                StartCoroutine(FadeBlackOutSquare(false));
                gameOverText.SetActive(false);
                wonText.SetActive(false);
                resetButton.gameObject.SetActive(false);
                nextLevelButton.gameObject.SetActive(false);
                nextLevelText.SetActive(false);
                break;
            case Status.LevelComplete:
                StartCoroutine(FadeBlackOutSquare());
                nextLevelText.GetComponent<TextMeshProUGUI>().text = "level " + player.GetComponent<Player>().currLevel + 1;
                nextLevelText.SetActive(true);
                nextLevelButton.gameObject.SetActive(true);
                resetButton.gameObject.SetActive(true);
                break;
            case Status.GameOver:
                gameOverText.SetActive(true);
                repositionButton(resetButton, true);
                resetButton.gameObject.SetActive(true);
                nextLevelText.SetActive(true);
                StartCoroutine(FadeBlackOutSquare());
                break;
            case Status.GameWon:
                wonText.SetActive(true);
                break;
            default:
                print("Default Switch");
                break;
        }
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
