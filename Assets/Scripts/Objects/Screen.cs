using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: Change move to tween
public class Screen : MonoBehaviour
{
    public GameObject titleText;
    public TextMeshProUGUI screenText;
    public Button startGameButton;
    public Button startTutorialButton;
    private Coroutine screenTextCoroutine;
    public float distance = 10f;
    public float fixedY = 0f; // Desired Y height

    private void Start()
    {
        startGameButton.onClick.AddListener(GameManager.Instance.StartNewRound);
        startTutorialButton.onClick.AddListener(GameManager.Instance.tutorial.StartTutorial);
    }

   
    public void SetScreenText(string text, float time)
    {
        ShowTitle(false);

        if (screenTextCoroutine != null)
        {
            StopCoroutine(screenTextCoroutine);
        }

        screenTextCoroutine = StartCoroutine(ScreenTextCoroutine(text, time));
    }

    private IEnumerator ScreenTextCoroutine(string text, float time)
    {
        screenText.text = text;
        yield return new WaitForSeconds(time);
        screenText.text = string.Empty;
    }

    public void ShowTitle(bool show)
    {
        titleText.SetActive(show);
        startGameButton.gameObject.SetActive(show);
        startTutorialButton.gameObject.SetActive(show);
        screenText.gameObject.SetActive(show == false);
    } 

    void Update()
    {
        // Get forward direction without vertical tilt
        Vector3 forwardFlat = Camera.main.transform.forward;
        forwardFlat.y = 0f;
        forwardFlat.Normalize();

        // Calculate target position at fixed Y
        Vector3 targetPosition = Camera.main.transform.position + forwardFlat * distance;
        targetPosition.y = fixedY;

        // Move object
        transform.position = targetPosition;
    }
}