using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryItem : Tile
{
    private bool _isBlinking = false;
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
        
        //지속 시간 만료 코루틴 호출
        StartCoroutine(ExpiryWarningEffect(tileSO.duration,recentTiles));
        
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
    
    public IEnumerator ExpiryWarningEffect(float duration, List<Tile> recentTiles)
    {
        // 지속 시간 동안 대기
        yield return new WaitForSeconds(duration - 3f);

        // 종료 N초 전부터 깜빡이기 시작
        Coroutine _blinkCoroutine = StartCoroutine(BlinkTiles(recentTiles));

        // N초 대기 후 효과 종료
        yield return new WaitForSeconds(3f);

        // 종료 처리
        StopCoroutine(_blinkCoroutine);
        _isBlinking = false;
        //원래 버전의 텍스쳐로 셰이더의 Top 변경
        foreach (var tile in recentTiles)
        {
            tile.GetComponent<Renderer>().material.SetTexture("_TopTex",tile.tileSO.originTopTex);
        }
        Debug.Log("지속효과 종료 이펙트 끝");
    }

    IEnumerator BlinkTiles(List<Tile> recentTiles)
    {
        _isBlinking = true;
        while (_isBlinking)
        {
            //원래 버전의 텍스쳐로 셰이더의 Top 변경
            foreach (var tile in recentTiles)
            {
                tile.GetComponent<Renderer>().material.SetTexture("_TopTex",tile.tileSO.originTopTex);
            }
            yield return new WaitForSeconds(0.2f);
            //투명 버전의 텍스처로 셰이더의 Top 변경
            foreach (var tile in recentTiles)
            {
                tile.GetComponent<Renderer>().material.SetTexture("_TopTex",tile.tileSO.transTopTex);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
