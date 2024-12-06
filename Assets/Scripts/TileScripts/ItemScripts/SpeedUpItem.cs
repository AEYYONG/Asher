using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpItem : Tile
{
    public List<Material> materials;
    string propertyName = "_enabled";
    
    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("속도증가 아이템 사용");
        StartCoroutine(SpeedUp(uiManager));
    }

    IEnumerator SpeedUp(StageUIManager uiManager)
    {
        VFXManager.Instance.PlayVFX("UseBuffItem",uiManager.player.transform);
        yield return new WaitForSeconds(1.5f);
        UpdateShaderProperties(true);
        Player_Move playerMove = uiManager.player.GetComponent<Player_Move>();
        playerMove.moveDuration *= 1/tileSO.power;
        
        yield return new WaitForSeconds(tileSO.duration);
        Debug.Log("속도증가 아이템 지속 시간 끝");
        UpdateShaderProperties(false);
        playerMove.moveDuration *= tileSO.power;
    }
    
    private void UpdateShaderProperties(bool isEnabled)
    {
        if (materials != null && materials.Count > 0)
        {
            int value = isEnabled ? 1 : 0;
            foreach (var material in materials)
            {
                if (material != null)
                {
                    material.SetInt(propertyName, value);
                }
            }
        }
    }
}
