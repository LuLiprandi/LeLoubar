using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        creditsPanel.SetActive(false);
    }

    /// <summary>Lance le jeu.</summary>
    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>Affiche les crédits.</summary>
    public void Credits()
    {
        creditsPanel.SetActive(true);
    }

    /// <summary>Retour au menu.</summary>
    public void Back()
    {
        creditsPanel.SetActive(false);
    }

    /// <summary>Quitte le jeu.</summary>
    public void Quit()
    {
        Application.Quit();
    }
}
