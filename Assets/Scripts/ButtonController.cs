using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    public void StartGame()
    {
        // Load the game scene when the button is clicked
        SceneManager.LoadScene(gameSceneName);
    }
}
