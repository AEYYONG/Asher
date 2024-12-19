using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryItem : Tile
{
    private LinkedList<Tile> memoryLists = new LinkedList<Tile>();
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        memoryLists = uiManager.player.GetComponent<PlayerInteract>()._recentTiles;
        StartCoroutine(ShowRecentTiles(uiManager,memoryLists));
        Debug.Log("기억복원 아이템 사용");
    }

    IEnumerator ShowRecentTiles(StageUIManager uiManager, LinkedList<Tile> tiles)
    {
        VFXManager.Instance.PlayVFX("UseBuffItem",uiManager.player.transform);
        yield return new WaitForSeconds(1.5f);
        List<Tile> recentTiles = new List<Tile>();
        foreach (var tile in tiles)
        {
            recentTiles.Add(tile);
        }
        //투명 버전의 텍스쳐로 셰이더의 Top 변경
        foreach (var tile in recentTiles)
        {
            tile.GetComponent<Renderer>().material.SetTexture("_TopTex",tile.tileSO.transTopTex);
        }
        yield return new WaitForSeconds(tileSO.duration);
        //원래 버전의 텍스쳐로 셰이더의 Top 변경
        foreach (var tile in recentTiles)
        {
            tile.GetComponent<Renderer>().material.SetTexture("_TopTex",tile.tileSO.originTopTex);
        }
    }
}
