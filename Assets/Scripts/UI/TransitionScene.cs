using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScene : MonoBehaviour
{
    public static TransitionScene Instance { get; private set; }

    [SerializeField] private List<GameObject> floors;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject screen;
    [SerializeField] private Image blackTransitionScreen;

    private int currentFloorIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionToNextFloor()
    {

        if (currentFloorIndex + 1 < floors.Count)
        {
            currentFloorIndex++;
            StartCoroutine(TransitionToFloorCoroutine());
        }
        else
        {
            Debug.LogWarning("No more floors left!");
        }
    }

    private IEnumerator TransitionToFloorCoroutine()
    {

        yield return Fade(0f, 1f);
        screen.SetActive(true);
        yield return Fade(1f, 0f);

        RoomManager.RM.LoadNextFloor();

        yield return new WaitForSeconds(1f);

        Vector3 targetPosition = floors[currentFloorIndex].transform.position;

        while (Vector3.Distance(player.transform.position, targetPosition) > 0.01f)
        {
            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                targetPosition,
                600f * Time.deltaTime 
            );
            yield return null;
        }
        player.transform.position = targetPosition;

        yield return new WaitForSeconds(0.5f);
        yield return Fade(0f, 1f);
        screen.SetActive(false);
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha)
    {
        blackTransitionScreen.gameObject.SetActive(true);
        float fadeDuration = 1f;
        float elapsedTime = 0f;
        Color color = blackTransitionScreen.color;

        color.a = startAlpha;
        blackTransitionScreen.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            blackTransitionScreen.color = color;

            yield return null;
        }
        color.a = targetAlpha;
        blackTransitionScreen.color = color;
        yield return new WaitForSeconds(fadeDuration);
        blackTransitionScreen.gameObject.SetActive(false);
    }
}