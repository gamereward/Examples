using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

    public Text noMine;
    public Text noGem;

    public void UpdateMines(int noMine){
        this.noMine.text = noMine.ToString();
        this.noGem.text = (25 - noMine).ToString();
    }
}
