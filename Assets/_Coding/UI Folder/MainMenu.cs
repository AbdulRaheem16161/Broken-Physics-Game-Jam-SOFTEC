using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene To Load When Play Is Pressed")]
    [SerializeField] private string sceneName = "GameScene";

    // Call this from Play Button
    public void PlayGame()
    {
        // Load the selected scene
        SceneManager.LoadScene(sceneName);
    }

    // Call this from Quit Button
    public void QuitGame()
    {
        // If running in editor, this won't quit play mode
        Debug.Log("Game Quit!");

        // Quit application (works in build)
        Application.Quit();
    }
}