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
        StartCoroutine(ArrangeShuffleTileOnBoard(uiManager,_tileManager,_shuffledTiles));

    }

    IEnumerator ArrangeShuffleTileOnBoard(StageUIManager uiManager, TileManager _tileManager, List<GameObject> tiles)
    {
        yield return new WaitForSeconds(1f);
        
        //Default UI 비활성화하기
        uiManager.defaultUI.SetActive(false);
        VFXManager.Instance.PlayVFX("ShuffleCurtain",uiManager.shuffleCanvas.transform);
        
        //플레이어와 NPC 움직임 멈추기
        StageManager.Instance.StopAllCharacterMove();
        
        yield return new WaitForSeconds(0.5f);
        
        //커튼 닫기
        int index = 0;
        _tileManager._tiles.Clear();
        
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
        
        yield return new WaitForSeconds(tileSO.duration);
        
        //커튼 열기
        //defaultUI 활성화하기
        uiManager.defaultUI.SetActive(true);
        //플레이어와 NPC 움직임 재개하기
        StageManager.Instance.StartAllCharacterMove();
        
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
