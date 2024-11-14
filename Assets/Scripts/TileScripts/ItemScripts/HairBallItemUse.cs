using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBallItemUse : MonoBehaviour
{
    public int BallDirection = -1;
    private NPC_Move npcMove;
    void Start()
    {
        npcMove = FindObjectOfType<NPC_Move>();
    }
    public void StartDirectionInput()
    {
        StartCoroutine(WaitForDirectionInput());
    }

    private IEnumerator WaitForDirectionInput()
    {
        float waitTime = 2f;
        float timer = 0f;
        while (timer < waitTime)
        {
            // 방향 입력을 즉시 감지하여 발사
            if (Input.GetKeyDown(KeyCode.W))
            {
                BallDirection = 0;
                FireImmediately();
                yield break; // 코루틴 종료
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                BallDirection = 1;
                FireImmediately();
                yield break; // 코루틴 종료
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                BallDirection = 2;
                FireImmediately();
                yield break; // 코루틴 종료
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                BallDirection = 3;
                FireImmediately();
                yield break; // 코루틴 종료
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 2초 동안 입력이 없으면 기본 방향으로 발사
        if (BallDirection == -1)
        {
            BallDirection = 0;
            FireImmediately();
            Player_Move player_Move = FindObjectOfType<Player_Move>();
            player_Move.useBall = false;
        }
    }

    private void FireImmediately()
    {
        FindObjectOfType<Player_Move>().StartFire(BallDirection);
    }



    // 언제 사라지는지? 일단은 5만큼 이동하면 사라지기 또는 장애물, npc에 맞으면 사라지게
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("장애물 충돌");
        Debug.Log("충돌한 태그: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("장애물 충돌 후 삭제");
            BallDirection = -1;
            //애니메이션 재생 필요
            Destroy(gameObject);

        }

        else if (collision.gameObject.tag == "NPC")
        {
            Debug.Log("npc를 맞춤");
            npcMove.AttackedHairBall();
            //애니메이션 재생 필요
            Destroy(gameObject);
        }
    }
}

