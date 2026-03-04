using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    /// <summary>Lance le jeu.</summary>
    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>Quitte le jeu.</summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>Affiche les crédits.</summary>
    public void Credits()
    {
        // À implémenter
    }
}
