using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;

    [SerializeField]
    private GameObject settings;

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        var shape = ps.shape;
        RectTransform rt = GetComponent<RectTransform>();
        Canvas canvas = rt.GetComponent<Canvas>();
        shape.scale = new Vector3(rt.rect.width*canvas.scaleFactor,1f,1f);
    }

    public void ToggleSettings()
    {
        if (settings != null)
        {
            if (settings.activeSelf)
            {
                settings.SetActive(false);
            }
            else
            {
                settings.SetActive(true);
            }
        }
    }
}
