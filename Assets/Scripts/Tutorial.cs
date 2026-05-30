using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject controls;
    private bool isTutorialActive = true;

    void Start()
    {
        controls.SetActive(true);
        CursorManager.Instance.SetDefaultCursor();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (isTutorialActive)
        {
            controls.SetActive(true);
            CursorManager.Instance.SetDefaultCursor();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }


    public void CloseTutorial()
    {
        controls.SetActive(false);
        isTutorialActive = false;
        CursorManager.Instance.SetDefaultCursor();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        gameObject.SetActive(false);
    }
}
