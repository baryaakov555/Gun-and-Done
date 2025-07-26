using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    public static PerformanceManager Instance { get; private set; }

    [Range(30, 240)] public int targetFPS = 60;
    [Tooltip("0 = off, 1 = every VBlank, 2 = every second VBlank")]
    public int vSyncCount = 0; // default to off

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scenes
        }
        else
        {
            Destroy(gameObject); // Remove duplicate
            return;
        }

        ApplySettings();
    }

    public void ApplySettings()
    {
        QualitySettings.vSyncCount = vSyncCount;   // If > 0, targetFrameRate is ignored
        if (vSyncCount == 0)
            Application.targetFrameRate = targetFPS;
    }
}
