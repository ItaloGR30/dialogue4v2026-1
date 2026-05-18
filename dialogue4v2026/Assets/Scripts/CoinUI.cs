using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    private static bool loaded = false;

    void Start()
    {
        if (loaded) return;

        loaded = true;

        SceneManager.LoadScene("GamePlay", LoadSceneMode.Additive);
        SceneManager.LoadScene("GUI", LoadSceneMode.Additive);
    }
}