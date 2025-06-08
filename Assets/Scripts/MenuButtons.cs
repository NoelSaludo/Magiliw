using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Scenes/VisualNovel");
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        // Stop play mode in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // Quit the application in a build
    Application.Quit();
#endif
    }
}
