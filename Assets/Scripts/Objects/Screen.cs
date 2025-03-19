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
    private const float BOUDNARY = 0.01f;

    public void Move(Vector3 destination)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveCoroutine(destination));
    }

    private IEnumerator MoveCoroutine(Vector3 destination)
    {
        while(Vector3.Distance(transform.position, destination) >= BOUDNARY)
        {
            transform.position += (transform.position - destination).normalized * MOVE_SPEED;

            yield return null;
        }

        transform.position = destination;
    }

    
    public void SetScreenText(string text, float time)
    {
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