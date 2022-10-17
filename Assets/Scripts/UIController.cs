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

    public GameObject wonLevelText;

    public GameObject nextLevelText;

    public GameObject beginText;

    public GameObject wonGameText;

    public GameObject tipsText;

    public GameObject area2;

    public Button resetButton;

    public Button nextLevelButton;

    public Button beginGameButton;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeBlackOutSquare(true));
        resetUIComponenets();
        resetButton.onClick.AddListener(player.GetComponent<Player>().resetLevel);
        nextLevelButton.onClick.AddListener(player.GetComponent<Player>().nextLevel); 
        beginGameButton.onClick.AddListener(player.GetComponent<Player>().beginLevel);
    }

    private void resetUIComponenets(){
        gameOverText.SetActive(false);
        wonGameText.SetActive(false);
        wonLevelText.SetActive(false);
        resetButton.gameObject.SetActive(false);
        tipsText.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        // nextLevelText.SetActive(false);
        // beginGameButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void handleEvent(Status playerStatus)
    {
        switch (playerStatus){
            case Status.BeginGame:
                print("here begin");
                beginGameButton.gameObject.SetActive(true);
                tipsText.gameObject.SetActive(true);
                nextLevelText.SetActive(true);
                nextLevelText.GetComponent<TextMeshProUGUI>().text = "" + player.GetComponent<Player>().currentLevel;
                break;
            case Status.Playing:
                // print("playing");
                print("playing");
                StartCoroutine(FadeBlackOutSquare(false, .85f));
                resetUIComponenets();
                nextLevelText.SetActive(false);
                beginGameButton.gameObject.SetActive(false);
                break;
            case Status.LevelComplete: // right after level complete. 
                StartCoroutine(FadeBlackOutSquare(true));
                wonLevelText.SetActive(true);
                repositionButton(nextLevelButton, true);
                nextLevelButton.gameObject.SetActive(true);
                // resetButton.gameObject.SetActive(true);
                break;
            case Status.NextLevelLoad: // after user clicks "next level"
                wonLevelText.SetActive(false);
                nextLevelButton.gameObject.SetActive(false);
                resetButton.gameObject.SetActive(false);
                beginGameButton.gameObject.SetActive(true);
                nextLevelText.GetComponent<TextMeshProUGUI>().text = "" + player.GetComponent<Player>().currentLevel;
                nextLevelText.SetActive(true);
                tipsText.gameObject.SetActive(true);
                tipsText.GetComponent<TextMeshProUGUI>().text = "< click c on your keyboard to change perspective >";
                break;
            case Status.GameOver:
                StartCoroutine(FadeBlackOutSquare());
                gameOverText.SetActive(true);
                repositionButton(resetButton, true);
                resetButton.gameObject.SetActive(true);
                nextLevelText.SetActive(true);
                break;
            case Status.GameWon:
                wonLevelText.SetActive(false);
                resetButton.gameObject.SetActive(false);
                nextLevelButton.gameObject.SetActive(false);
                nextLevelText.SetActive(false);
                wonGameText.SetActive(true);
                break;
            default:
                print("Default Switch");
                break;
        }
    }

    public IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, float fadeSpeed = 1f)
    {
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        if (fadeToBlack)
        {
            while (blackOutSquare.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        } else 
        {
            while (blackOutSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        yield return new WaitForEndOfFrame();
    }

    private void repositionButton(Button button, bool singleButton) {
        RectTransform buttonTransform = button.gameObject.GetComponent<RectTransform>();
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
