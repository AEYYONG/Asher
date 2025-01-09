using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBallItem : Tile
{
    [SerializeField] private GameObject hairBallPrefab;

    public override void ItemUse(StageUIManager uiManager)
    {
        Player_Move.Instance.useBall = true;
        base.ItemUse(uiManager);
        Debug.Log("헤어볼 아이템 사용");

        GameObject hairBallInstance = Instantiate(hairBallPrefab, Player_Move.Instance.transform.position, Quaternion.identity);

        HairBallItemUse hairBallItemUse = hairBallInstance.GetComponent<HairBallItemUse>();
        hairBallItemUse.StartDirectionInput();
        Debug.Log("호출이 되고있나요");
        /*GameObject hairBallObject = null;
        hairBallItemUse = hairBallObject.GetComponent<HairBallItemUse>();
        Debug.Log("호출이 되고있나요1");
        if (hairBallItemUse != null)
        {
            hairBallItemUse.StartDirectionInput();
            Debug.Log("호출이 되고있나요");
        }
        else
        {
            Debug.LogWarning("HairBallItemUse 컴포넌트를 찾을 수 없습니다.");
        }
        if (hairBallObject != null)
        {
            Debug.Log("호출이 되고있나요1.1");
            hairBallItemUse = hairBallObject.GetComponent<HairBallItemUse>();
            Debug.Log("호출이 되고있나요1");
            if (hairBallItemUse != null)
            {
                hairBallItemUse.StartDirectionInput();
                Debug.Log("호출이 되고있나요");
            }
            else
            {
                Debug.LogWarning("HairBallItemUse 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("HairBallUse 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }*/
    }
}
