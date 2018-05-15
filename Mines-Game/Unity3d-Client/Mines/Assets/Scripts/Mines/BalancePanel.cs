using System.Collections;
using System.Collections.Generic;
using Grd;
using UnityEngine;
using UnityEngine.UI;

public class BalancePanel : MonoBehaviour
{

    public Text balance;
    public GameObject wallet;

    private void Update()
    {
        UpdateBalance((double)GrdManager.User.balance);
    }

    public void UpdateBalance(double b){
        balance.text =b.ToString("#.##");
    }

    public void ShowWallet(){
        wallet.SetActive(true);
    }
}
