using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuShowCoins : MonoBehaviour
{
    private void OnEnable()
    {
        ShowCoins();
    }

    void ShowCoins()
    {
        GetComponent<Text>().text = "Coins: " + GameManager.Instance.totalCoins.ToString();
    }
}
