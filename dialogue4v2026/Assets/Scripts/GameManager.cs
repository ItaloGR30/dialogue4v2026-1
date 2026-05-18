using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("GameManager iniciou");

        SceneManager.LoadScene("GamePlay", LoadSceneMode.Additive);
        SceneManager.LoadScene("GUI", LoadSceneMode.Additive);
    }
}