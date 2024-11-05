using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBallItem : Tile
{
    private HairBallItemUse hairBallItemUse;

    public override void ItemUse(StageUIManager uiManager)
    {
        base.ItemUse(uiManager);
        Debug.Log("헤어볼 아이템 사용");

        GameObject hairBallObject = GameObject.FindWithTag("HairBall");
        if (hairBallObject != null)
        {
            hairBallItemUse = hairBallObject.GetComponent<HairBallItemUse>();
            if (hairBallItemUse != null)
            {
                hairBallItemUse.StartDirectionInput();
            }
            else
            {
                Debug.LogWarning("HairBallItemUse 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("HairBallUse 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }
}
