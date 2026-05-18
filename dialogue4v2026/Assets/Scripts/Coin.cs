using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCoinCollector collector =
                other.GetComponent<PlayerCoinCollector>();

            collector.AddCoin();

            Destroy(gameObject);
        }
    }
}