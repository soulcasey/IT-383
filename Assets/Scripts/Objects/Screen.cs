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
    public Button startButton;
    private Coroutine moveCoroutine;
    private Coroutine screenTextCoroutine;
    
    private const float MOVE_SPEED = 1f;
    private const float BOUDNARY = 0.05f;

    public void Move(Vector3 destination, Action onComplete = null)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

       moveCoroutine = StartCoroutine(MoveCoroutine(destination, onComplete));
    }

    private IEnumerator MoveCoroutine(Vector3 destination, Action onComplete = null)
    {
        while (Vector3.Distance(transform.position, destination) >= BOUDNARY)
        {
            transform.position += MOVE_SPEED * Time.deltaTime * (destination - transform.position).normalized;

            yield return null;
        }

        transform.position = destination;

        onComplete?.Invoke();
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
        startButton.gameObject.SetActive(show);
        screenText.gameObject.SetActive(show == false);
    } 
}