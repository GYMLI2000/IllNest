using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public static DeathScreen Instance;
    private bool isShown = false;
    

    [SerializeField]
    private Light2D gameLight;

    [SerializeField]
    private GameObject deathScreenUI;

    private void Awake()
    {
        Instance = this;
        deathScreenUI.SetActive(false);
    }

    public void Show()
    {
        isShown = true;
    }

    private void Update()
    {
        if (isShown)
        {
            gameLight.intensity = Mathf.Lerp(gameLight.intensity, 0f, Time.deltaTime);

            if (gameLight.intensity <= 0.01f)
            {
                gameLight.intensity = 0f;
                deathScreenUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
                isShown = false;
            }
        }
    }

    public void RestartRun()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");

    }
}
