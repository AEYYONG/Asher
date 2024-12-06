using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletTrap : Tile
{
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("컨틀렛 아이템 사용");
        TileManager _tileManager = FindObjectOfType<TileManager>();
        
        //타일 셔플
        List<GameObject> _curTiles = new List<GameObject>();
        foreach (var tile in _tileManager._tiles)
        {
            _curTiles.Add(tile.Value);
        }
        List<GameObject> _shuffledTiles = new List<GameObject>();
        _shuffledTiles = _tileManager.ShuffleTileList(_curTiles);
        //셔플한 타일 배치
        StartCoroutine(ArrangeShuffleTileOnBoard(_tileManager,_shuffledTiles));

    }

    IEnumerator ArrangeShuffleTileOnBoard(TileManager _tileManager, List<GameObject> tiles)
    {
        int index = 0;
        _tileManager._tiles.Clear();

        yield return new WaitForSeconds(tileSO.duration);
        for (int z = 0; z < _tileManager.height; z++)
        {
            for (int x = 0; x < _tileManager.width; x++)
            {
                Vector2Int pos = new Vector2Int(x, z);
                if (!_tileManager.furnitureTilePosList.Contains(pos))
                {
                    tiles[index].GetComponent<Tile>().InitTile(x,z);
                    tiles[index].transform.position = new Vector3(x, 0, z);
                    int nameIndex = tiles[index].name.IndexOf(':');
                    tiles[index].name = $"Tile{pos} : {tiles[index].name.Substring(nameIndex+2,tiles[index].name.Length-nameIndex-2)}";
                    _tileManager._tiles.Add(pos,tiles[index++]);
                }
            }
        }
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
