using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesResponse : BaseResponse {

    public int gameid;
    public double bet;
    public double payout;
    public double nextProfit;
    public int mines;
    public int status;
    public int win;

    public List<int> minesIndex;
    public List<int> playIndex;

}
