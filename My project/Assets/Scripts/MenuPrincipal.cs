using UnityEngine;

public class MenuPrincipalUI : MonoBehaviour
{
    public void Iniciar()
    {
        GameManager.Instance.LoadScene("SampleScene");
    }

    public void Sair()
    {
        GameManager.Instance.QuitGame();
    }
}