using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Call this function via the button OnClick event.
    public void StartGame()
    {
        // Replace "Game" with the exact name of your Game scene.
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        // This will quit the application. Note that this won't work in the editor.
        Application.Quit();

        // If you want to stop playing in the editor, uncomment the line below:
        // #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
        // #endif
    }

    public void ReturnToMainMenu()
    {
        // Replace "MainMenu" with the exact name of your Main Menu scene.
        SceneManager.LoadScene("MainMenu");
    }
}
