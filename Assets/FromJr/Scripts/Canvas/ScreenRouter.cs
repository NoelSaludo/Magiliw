using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneRouter : MonoBehaviour
{
    [Header("Canvas to Monitor")]
    public Canvas targetCanvas;

#if UNITY_EDITOR
    [Header("Scene to Load (Drag Scene Asset)")]
    public SceneAsset sceneToLoad;
    [HideInInspector] public string sceneName;
#else
    [SerializeField] private string sceneName;
#endif

    private bool hasSwitched = false;

    void Start()
    {
#if UNITY_EDITOR
        if (sceneToLoad != null)
        {
            sceneName = sceneToLoad.name;
        }
#endif

        if (targetCanvas == null)
        {
            Debug.LogError("SceneRouter: Target canvas is not assigned.");
        }
    }

    void Update()
    {
        if (hasSwitched || targetCanvas == null) return;

        // Trigger scene change when canvas becomes active
        if (targetCanvas.gameObject.activeInHierarchy)
        {
            hasSwitched = true;
            SceneManager.LoadScene(sceneName);
        }
    }
}
