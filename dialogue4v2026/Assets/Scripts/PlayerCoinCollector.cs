using UnityEngine;

public class PlayerCoinCollector : MonoBehaviour
{
    private int coins = 0;

    public void AddCoin()
    {
        coins++;

        PlayerObserverManager.CoinCollected(coins);
    }
}