using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPlayPanel : MonoBehaviour {

    public Text nextProfitText;
    public Text payOutText;
    public Button payoutButton;

    public void clearPayout(){
        nextProfitText.text = "0";
        payOutText.text = "Cashout 0.00 GRD";
    }

    public void SetPayOut(double payOut){
        if (System.Math.Abs(payOut) < Mathf.Epsilon)
        {
            payoutButton.interactable = false;
			payOutText.text = "Cashout 0.00 GRD";
        }
        else
        {
            payoutButton.interactable = true;
            payOutText.text = "Cashout " + payOut.ToString("#.##") + " GRD";
        }
    }

    public void SetNextProfit(double profit){
        nextProfitText.text = profit.ToString("#.##");
    }
}
