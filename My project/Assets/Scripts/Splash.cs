using UnityEngine;
using System.Collections;

public class SplashManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SplashTimer());
    }

    IEnumerator SplashTimer()
    {
        yield return new WaitForSeconds(2f);

        GameManager.Instance.LoadScene("MenuPrincipal");
    }
}