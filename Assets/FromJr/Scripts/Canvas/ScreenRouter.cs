using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneRouter : MonoBehaviour
{
    [Header("Canvas to Monitor")]
    public Canvas targetCanvas;

    [Header("Scene to Load (Name as string)")]
    [SerializeField] private string sceneName;

#if UNITY_EDITOR
    [Header("Optional: Drag Scene Asset for Editor")]
    public SceneAsset sceneAsset;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }
#endif

    private bool hasSwitched = false;

    void Start()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("SceneRouter: Target canvas is not assigned.");
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SceneRouter: Scene name is not specified.");
        }
    }

    void Update()
    {
        if (hasSwitched || targetCanvas == null) return;

        // Check if canvas is enabled and active in hierarchy
        if (targetCanvas.enabled && targetCanvas.gameObject.activeInHierarchy)
        {
            hasSwitched = true;
            SceneManager.LoadScene(sceneName);
        }
    }
}