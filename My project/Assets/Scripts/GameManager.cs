using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ChangeState(GameState.Iniciando);

        // Após iniciar, ir para Splash
        LoadScene("Splash");
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        Debug.Log("Estado atual do jogo: " + currentState);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);

        // Mudança automática de estado dependendo da cena
        if (sceneName == "MenuPrincipal")
        {
            ChangeState(GameState.MenuPrincipal);
        }
        else if (sceneName == "SampleScene")
        {
            ChangeState(GameState.Gameplay);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}