using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinesBetController : MonoBehaviour {

    private static MinesBetController _instance;

    public static MinesBetController Instance {
        get { return _instance; }
    }

    public GameObject onPlayingPanel;
    public GameObject onBettingPanel;
    public InfoPanel infoPanel;
    public OnPlayPanel onPlayPanel;
	public GameTable gameTable;
    public BalancePanel balancePanel;

    public InputField noMineInput;
    public InputField betAmountInput;
    public int numberOfMine=3;
    public int betAmount = 0;
    public int openedGems =0;
    public double currentProfit;
    public int currentGameId;


    private void Awake()
    {
        _instance = this;
        noMineInput.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    private void ValueChangeCheck()
    {
        try{
            numberOfMine = Mathf.Clamp(Int32.Parse(noMineInput.text), 1, 24);
			noMineInput.text = numberOfMine.ToString();
			infoPanel.UpdateMines(numberOfMine);
        }catch(Exception e){
            return;
        }
    }

    public void SetNumberOfMine(int number){
        noMineInput.text = number+"";
        numberOfMine = number;
        infoPanel.UpdateMines(number);
    }

    public void DivideBetAmount(){
        betAmount = betAmount / 2;
        betAmountInput.text = betAmount.ToString();
    }

    public void MultiBetAmount(){
        if (betAmount == 0) betAmount = 1;
        else betAmount = betAmount * 2;
		betAmountInput.text = betAmount.ToString();
    }

    public void StartBet(){
        clearBet();
        betAmount = Int32.Parse(betAmountInput.text);
		numberOfMine = Int32.Parse(noMineInput.text);
        GrdManager.Instance.CallServerScript("mines", "newgame", new object[]{betAmount,numberOfMine},(error, data) => {
            if(error==0){
                MinesResponse result = MiniJSON.Json.GetObject<MinesResponse>(data);
				if (result.error == 0)
				{
					gameTable.InitTable(result, MinesTileStatus.HIDDEN);
					numberOfMine = result.mines;
					infoPanel.UpdateMines(numberOfMine);
					currentGameId = result.gameid;
					if (betAmount > 0)
					{
                        GrdManager.Instance.User.balance = GrdManager.Instance.User.balance - (decimal)result.bet;
                        balancePanel.UpdateBalance((double)GrdManager.Instance.User.balance);
					}
					OnStartPlay();
				}
            }else{
                Debug.Log(data);
            }
        });
    }

    public void Cashout(){
        GrdManager.Instance.CallServerScript("mines","payout",null,(error, data) => {
			MinesResponse result = MiniJSON.Json.GetObject<MinesResponse>(data);
			if (result.error == 0)
			{
                GrdManager.Instance.User.balance = GrdManager.Instance.User.balance + (decimal)result.payout;
				balancePanel.UpdateBalance((double)GrdManager.Instance.User.balance);
				OnEndPlay();
				gameTable.ShowTable(result);
			}
        });
    }

    public void GameOver(MinesResponse result){
        OnEndPlay();
        gameTable.ShowTable(result);
    }

    public void OnStartPlay(){
        onPlayingPanel.SetActive(true);
        onBettingPanel.SetActive(false);
    }

    public void OnEndPlay(){
        onPlayingPanel.SetActive(false);
		onBettingPanel.SetActive(true);
    }

    public void OnGemOpen(MinesResponse response, int pos){
        openedGems++;
        gameTable.RemoveTile(pos);
        currentProfit = response.payout;
        onPlayPanel.SetPayOut(currentProfit);
        onPlayPanel.SetNextProfit(response.nextProfit);
    }

    public void Win(MinesResponse result){
		openedGems++;
        currentProfit = result.payout;
		onPlayPanel.SetPayOut(currentProfit);
        GrdManager.Instance.User.balance = GrdManager.Instance.User.balance + (decimal)currentProfit;
        balancePanel.UpdateBalance((double)GrdManager.Instance.User.balance);
		OnEndPlay();
        gameTable.ShowTable(result);

    }

    public void OpenRandomTile(){
        gameTable.OpenRandomTile();
    }


    public double PayoutCal(int mines, int diamonds){
        var house_edge = 0.01f;
        return (1 - house_edge) * nCr(25,diamonds) / nCr(25 - mines, diamonds);
    }

    double nCr(int n, int r){
        double result = Factorial(n) / Factorial(r) / Factorial(n - r);
        return result;

	}

    double Factorial(int i)
	{
		if (i <= 1)
			return 1;
		return i * Factorial(i - 1);
	}

    void clearBet(){
        openedGems = 0;
        currentProfit = 0;
		onPlayPanel.SetPayOut(0);
		onPlayPanel.SetNextProfit((PayoutCal(numberOfMine, openedGems + 1) - PayoutCal(numberOfMine, openedGems)) * Double.Parse(betAmountInput.text));
    }

}
