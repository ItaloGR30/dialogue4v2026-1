using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    const string PrefKey = "PlayModeBootper_TargetSceneName";

    IEnumerator Start()
    {
        // Only run this logic in the editor play session where the editor placed _Boot as the play scene
        #if UNITY_EDITOR
        string targetPath = UnityEditor.EditorPrefs.GetString(PrefKey, null);
        if (string.IsNullOrEmpty(targetPath))
        {
            Debug.LogWarning("[BootLoader] No target scene specified in EditorPrefs. Nothing to load.");
            yield break;
        }

        // Wait a frame for things to initialize
        yield return null;

        string targetName = System.IO.Path.GetFileNameWithoutExtension(targetPath);

        Debug.Log($"[BootLoader] Attempting to load target scene. Path: '{targetPath}', Name: '{targetName}'");

        // If the target scene is already loaded by name, just unload the boot scene
        if (SceneManager.GetSceneByName(targetName).isLoaded)
        {
            Debug.Log($"[BootLoader] Target scene '{targetName}' already loaded. Unloading _Boot.");
            var bootScene = SceneManager.GetSceneByName("_Boot");
            if (bootScene.IsValid() && bootScene.isLoaded)
                yield return SceneManager.UnloadSceneAsync(bootScene);
            yield break;
        }

        AsyncOperation op = null;

        // First try to load by name (works if scene is in Build Settings)
        try
        {
            op = SceneManager.LoadSceneAsync(targetName, LoadSceneMode.Additive);
            if (op != null)
                Debug.Log($"[BootLoader] Started LoadSceneAsync by name for '{targetName}'.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[BootLoader] LoadSceneAsync by name threw exception for '{targetName}': {ex.Message}");
            op = null;
        }

        // If couldn't start a normal load (e.g., scene not in Build Settings), fallback to Editor-only API to load by path in play mode
        if (op == null)
        {
            try
            {
                var loadParams = new LoadSceneParameters(LoadSceneMode.Additive);
                op = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(targetPath, loadParams);
                if (op != null)
                    Debug.Log($"[BootLoader] Started EditorSceneManager.LoadSceneAsyncInPlayMode for path '{targetPath}'.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[BootLoader] Failed to start Editor load for '{targetPath}': {ex.Message}");
                op = null;
            }
        }

        if (op == null)
        {
            Debug.LogError($"[BootLoader] Failed to start loading scene '{targetName}' (path '{targetPath}').");
            yield break;
        }

        while (!op.isDone)
            yield return null;

        // After load completes, find the loaded scene by name and activate it
        Scene loaded = SceneManager.GetSceneByName(targetName);
        if (loaded.IsValid() && loaded.isLoaded)
        {
            SceneManager.SetActiveScene(loaded);
            Debug.Log($"[BootLoader] Loaded and set active scene '{targetName}'. Unloading _Boot.");
        }
        else
        {
            Debug.LogError($"[BootLoader] Scene '{targetName}' was not found after load operation.");
        }

        // Unload the boot scene by name
        var boot = SceneManager.GetSceneByName("_Boot");
        if (boot.IsValid() && boot.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(boot);
        }
        #endif
    }
}





