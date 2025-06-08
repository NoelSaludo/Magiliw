using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
        // Exit the application when the button is clicked
        Application.Quit();
        
        // If running in the editor, stop playing
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
