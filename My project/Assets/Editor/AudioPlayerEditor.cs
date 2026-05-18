using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
    SerializedProperty myAudioCollectionProp;
    SerializedProperty playIndexProp;
    SerializedProperty playOnStartProp;

    void OnEnable()
    {
        myAudioCollectionProp = serializedObject.FindProperty("myAudioCollection");
        playIndexProp = serializedObject.FindProperty("playIndex");
        playOnStartProp = serializedObject.FindProperty("playOnStart");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(myAudioCollectionProp);
        EditorGUILayout.PropertyField(playIndexProp);
        EditorGUILayout.PropertyField(playOnStartProp);

        AudioPlayer player = (AudioPlayer)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);

        // Buttons that only work in Play mode
        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("Play Selected"))
        {
            player.PlaySelected();
        }

        if (GUILayout.Button("Pause"))
        {
            player.Pause();
        }

        if (GUILayout.Button("Stop"))
        {
            player.Stop();
        }

        if (GUILayout.Button("Resume"))
        {
            player.Resume();
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Play Index via AudioManager", EditorStyles.boldLabel);

        // Allow editing the index in inspector (works in edit mode and play mode)
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUILayout.IntField("Index to play at runtime", player.playIndex);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(player, "Change Play Index");
            player.playIndex = newIndex;
            EditorUtility.SetDirty(player);
        }

        // Button to trigger AudioManager singleton from the inspector while in Play mode
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Play Index via AudioManager Singleton"))
            {
                if (AudioManager.Instance == null)
                {
                    Debug.LogWarning("AudioManager.Instance is null. Make sure an AudioManager exists in the scene.");
                }
                else if (player.myAudioCollection == null || player.myAudioCollection.AudioClipCollection == null)
                {
                    Debug.LogWarning("AudioPlayer: No AudioCollection or clips available.");
                }
                else if (player.playIndex < 0 || player.playIndex >= player.myAudioCollection.AudioClipCollection.Count)
                {
                    Debug.LogWarning($"AudioPlayer: playIndex {player.playIndex} is out of range.");
                }
                else
                {
                    var clip = player.myAudioCollection.AudioClipCollection[player.playIndex];
                    AudioManager.Instance.PlaySound(clip);
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Enter Play mode to use the AudioManager singleton play button.", MessageType.Info);
        }

        // Show clip count and names (read-only)
        int count = player.GetClipCount();
        EditorGUILayout.LabelField("Clip Count:", count.ToString());
        if (player.myAudioCollection != null && player.myAudioCollection.AudioClipCollection != null)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < player.myAudioCollection.AudioClipCollection.Count; i++)
            {
                var c = player.myAudioCollection.AudioClipCollection[i];
                EditorGUILayout.LabelField($"[{i}]", c ? c.name : "(null)");
            }
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

