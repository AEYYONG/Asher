using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaTrap : Tile
{
    public GameObject bananaVFX;
    public GameObject stunVFX;
    public override void TrapUse(StageUIManager uiManager)
    {
        Player_Move.Instance.Sliptrue();
        Player_Move.Instance.isStart = false;
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
        
        //플레이어의 자식 오브젝트로 stun vfx 생성하기
        GameObject stun = Instantiate(stunVFX,uiManager.player.transform);
        
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
        //stun vfx 제거
        Destroy(stun);
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
