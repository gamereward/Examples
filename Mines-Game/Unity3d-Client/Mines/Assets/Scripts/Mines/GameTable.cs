using System.Collections.Generic;
using UnityEngine;

public class GameTable : MonoBehaviour {

    public GameObject tilePrefab;
    private List<int> minesIndex;
    List<int> tiles = new List<int>();

	// Use this for initialization
	void Start () {
        MinesResponse init = new MinesResponse();
        init.mines = 1;
        init.minesIndex = new List<int>();
        init.minesIndex.Add(1);
        InitTable(init,MinesTileStatus.DISABLE);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitTable(MinesResponse response, MinesTileStatus status){
        ClearTable();
        minesIndex = response.minesIndex;
        for (int i = 0; i < 25;i++){
            MinesTile tile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity).GetComponent<MinesTile>();
            tile.transform.parent = transform;
            tile.status = status;
            tile.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            tile.pos = i;
            tiles.Add(i);
            if (minesIndex.Contains(i)) tile.type = MinesTileType.BOM;
            else tile.type = MinesTileType.GEM;
        }
    }

    void ClearTable(){
        tiles.Clear();
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
    }

    void RandomMines(int tableSize,int number){
        minesIndex = new List<int>();
        while(minesIndex.Count<number){
            int index = Random.Range(0, tableSize);
            if (!minesIndex.Contains(index)) minesIndex.Add(index);
        }
    }

    public void ShowTable(MinesResponse result){
        foreach(MinesTile tile in transform.GetComponentsInChildren<MinesTile>()){
            if (tile.status == MinesTileStatus.HIDDEN)
            {
                if(result.minesIndex.Contains(tile.pos)){
                    tile.ShowTile(MinesTileType.BOM);
                }else{
                    tile.ShowTile(MinesTileType.GEM);
                }
            }
        }
    }

    public void OpenRandomTile()
    {
        int index = Random.Range(0,tiles.Count);
        int posOpen = tiles[index];
        transform.GetChild(posOpen).GetComponent<MinesTile>().OnClick();
    }

    public void RemoveTile(int pos){
        tiles.RemoveAt(tiles.IndexOf(pos));
    }
}
