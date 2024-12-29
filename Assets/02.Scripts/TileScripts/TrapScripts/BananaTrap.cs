using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaTrap : Tile
{
    public GameObject bananaVFX;
    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        Debug.Log("바나나 아이템 사용");
        StartCoroutine(StartSlip(uiManager));
    }

    IEnumerator StartSlip(StageUIManager uiManager)
    {
        //타일 위치에 바나나 VFX 프리팹 생성하기
        Vector3 pos = new Vector3(transform.position.x, 0.3f, transform.position.z);
        GameObject banana = Instantiate(bananaVFX);
        banana.transform.position = pos;
        
        VFXManager.Instance.PlayVFX("UseDebuffItem",uiManager.player.transform);
        yield return new WaitForSeconds(1.5f);
        Player_Move player = uiManager.player.GetComponent<Player_Move>();
        if (!player.isSlip)
        {
            player.StartSlip();
        }

        bool isSlipEnd = false;
        player.OnSlipEnd += () => isSlipEnd = true;
        yield return new WaitUntil(() => isSlipEnd);
        
        //바나나 vfx 제거
        Destroy(banana);
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
