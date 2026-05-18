using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PlayModeBootper
{
    [InitializeOnLoad]
    static class PlayModeBootperEditor
    {
        const string PrefKey = "PlayModeBootper_TargetSceneName";
        const string BootSceneName = "_Boot";

        // keep the previous playModeStartScene so we can restore it
        static SceneAsset _previousStartScene;

        static PlayModeBootperEditor()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                var active = EditorSceneManager.GetActiveScene();
                if (string.IsNullOrEmpty(active.path))
                {
                    Debug.LogWarning("[PlayModeBootper] Active scene has no path; aborting boot override.");
                    return;
                }

                // Save the active scene path so the boot scene can load it at runtime
                EditorPrefs.SetString(PrefKey, active.path);

                // Try to find the _Boot scene asset in the project
                string[] guids = AssetDatabase.FindAssets(BootSceneName + " t:Scene");
                SceneAsset bootAsset = null;
                foreach (var g in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(g);
                    if (System.IO.Path.GetFileNameWithoutExtension(path) == BootSceneName)
                    {
                        bootAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                        break;
                    }
                }

                if (bootAsset == null)
                {
                    Debug.LogWarning("[PlayModeBootper] Couldn't find a scene named '_Boot' in the project. Play will proceed normally.");
                    return;
                }

                  // Ensure the boot scene has a BootLoader component instance so runtime can load the target scene
                  try
                  {
                      string bootPath = AssetDatabase.GetAssetPath(bootAsset);
                      // Open boot scene additively so we can inspect/edit it
                      var opened = EditorSceneManager.OpenScene(bootPath, OpenSceneMode.Additive);
                      bool hasBootLoader = false;
                      foreach (var root in opened.GetRootGameObjects())
                      {
                          if (root.GetComponentInChildren(typeof(BootLoader)) != null)
                          {
                              hasBootLoader = true;
                              break;
                          }
                      }

                      if (!hasBootLoader)
                      {
                          var go = new GameObject("BootLoader");
                          go.AddComponent<BootLoader>();
                          // Move the new object into the opened boot scene
                          UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, opened);
                          EditorSceneManager.MarkSceneDirty(opened);
                          EditorSceneManager.SaveScene(opened);
                          Debug.Log("[PlayModeBootper] Added BootLoader component to _Boot scene.");
                      }

                      // Restore the previously active scene so we don't change the user's open scene
                      EditorSceneManager.OpenScene(active.path, OpenSceneMode.Single);
                  }
                  catch (System.Exception ex)
                  {
                      Debug.LogWarning($"[PlayModeBootper] Could not ensure BootLoader in _Boot scene: {ex.Message}");
                  }

                  _previousStartScene = EditorSceneManager.playModeStartScene;
                  EditorSceneManager.playModeStartScene = bootAsset;
                  Debug.Log($"[PlayModeBootper] PlayMode start scene set to '{BootSceneName}'. Target scene '{active.path}' saved.");
            }

            // When returning to the editor (play stopped) restore previous state and clear the pref
            if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.ExitingPlayMode)
            {
                try
                {
                    if (_previousStartScene != null)
                    {
                        EditorSceneManager.playModeStartScene = _previousStartScene;
                        _previousStartScene = null;
                    }
                    else
                    {
                        EditorSceneManager.playModeStartScene = null;
                    }
                }
                catch
                {
                    // ignore any issues restoring the start scene
                }

                if (EditorPrefs.HasKey(PrefKey))
                    EditorPrefs.DeleteKey(PrefKey);
            }
        }
    }
}





