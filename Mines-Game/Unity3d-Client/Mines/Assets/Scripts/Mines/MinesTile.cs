using System.Collections;
using System.Collections.Generic;
using Grd;
using UnityEngine;
using UnityEngine.UI;

public enum MinesTileType{ BOM, GEM}
public enum MinesTileStatus {DISABLE,HIDDEN,OPENED}

public class MinesTile : MonoBehaviour {


    public Image image;

    public int pos;
    public MinesTileStatus status;
    public MinesTileType type;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { OnClick(); });
	}

    public void OnClick(){
        if (status != MinesTileStatus.HIDDEN) return;
        GrdManager.CallServerScript("mines","open",new object[]{pos},(error, data) => {
            MinesResponse result = MiniJSON.Json.GetObject<MinesResponse>(data.Data);
			if (result.win == -1)
			{
				image.enabled = true;
				image.sprite = Resources.Load<Sprite>("Mines/bom");
				status = MinesTileStatus.OPENED;
				MinesBetController.Instance.GameOver(result);
			}
			else if (result.win == 0)
			{
				image.enabled = true;
				image.sprite = Resources.Load<Sprite>("Mines/gem");
				status = MinesTileStatus.OPENED;
				MinesBetController.Instance.OnGemOpen(result, pos);
			}
			else
			{
				image.enabled = true;
				image.sprite = Resources.Load<Sprite>("Mines/gem");
				status = MinesTileStatus.OPENED;
				MinesBetController.Instance.Win(result);
			}
        });
       
    }

    public void ShowTile(MinesTileType type){
		switch (type)
		{
			case MinesTileType.BOM:
				image.enabled = true;
				image.sprite = Resources.Load<Sprite>("Mines/bom");
                image.rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.35f);
                status = MinesTileStatus.OPENED;
				break;
			case MinesTileType.GEM:
				image.enabled = true;
				image.sprite = Resources.Load<Sprite>("Mines/gem");
                image.rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.35f);
                status = MinesTileStatus.OPENED;
				break;

		}
    }
}
