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
    
    public const float MOVE_TIME = 1f;
    public const float SCALE_TIME = 1f;
    private static readonly Vector3 START_POSITION = new Vector3(0, 1.5f, 4.4f);
    private static readonly Vector3 END_POSITION = new Vector3(0, 1.5f, 30f);
    private static readonly (float MIN, float MAX) SCALE = (0.02f, 0.08f);

    
    private void Start()
    {
        startButton.onClick.AddListener(GameManager.Instance.StartNewRound);
    }

    public void Move(bool isGameStart, Action isComplete = null)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

       moveCoroutine = StartCoroutine(MoveAndScale(isGameStart, isComplete));
    }

    private IEnumerator MoveAndScale(bool isGameStart, Action isComplete = null)
    {
        if (isGameStart)
        {
            yield return MoveCoroutine(END_POSITION);
            yield return ScaleCoroutine(SCALE.MAX);
        }
        else
        {
            yield return ScaleCoroutine(SCALE.MIN);
            yield return MoveCoroutine(START_POSITION);
        }

        isComplete?.Invoke();
    }

    private IEnumerator MoveCoroutine(Vector3 destination)
    {
        Vector3 initialPosition = transform.position;

        float elapsedTime = 0f;

        // First, move to the target position over MOVE_TIME
        while (elapsedTime < MOVE_TIME)
        {
            float t = elapsedTime / MOVE_TIME;
            transform.position = Vector3.Lerp(initialPosition, destination, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
    }

    private IEnumerator ScaleCoroutine(float scale)
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.one * scale;

        float elapsedTime = 0f;
        while (elapsedTime < SCALE_TIME)
        {
            float t = elapsedTime / SCALE_TIME;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
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