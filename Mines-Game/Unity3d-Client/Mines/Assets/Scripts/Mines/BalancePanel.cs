using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalancePanel : MonoBehaviour
{

    public Text balance;
    public GameObject wallet;

    private void Update()
    {
        UpdateBalance((double)GrdManager.Instance.User.balance);
    }

    public void UpdateBalance(double b){
        balance.text =b.ToString("#.##");
    }

    public void ShowWallet(){
        wallet.SetActive(true);
    }
}
