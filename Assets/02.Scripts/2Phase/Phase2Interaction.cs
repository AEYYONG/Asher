using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase2Interaction : MonoBehaviour
{
    //타일을 뒤집기 위한 레이캐스트
    private RaycastHit _hit;
    private Vector3 _rayPos;

    private bool canInteract = true;
    private int _curSelectCnt = 0;
    private List<Tile> _tiles = new List<Tile>();
    private List<Tile> _wrongtile = new List<Tile>()
;
    private TileManager _tileManager;

    [SerializeField] private string[] tileOrderNames;
    [SerializeField] private string[] NPCtileOrderNames;
    private int currentOrderIndex = 0;

    // 플레이어의 순서인지
    public bool isPlayerTurn = true;

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red); // 디버그용 레이 시각화

        if (isPlayerTurn)
        {
            if (Physics.Raycast(ray, out _hit, 100f))
            {
                Tile curTile = _hit.collider.GetComponent<Tile>();
                //Debug.Log("타일 이름: " +curTile.name);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(ray, out _hit, 100f))
                {
                    Tile curTile = _hit.collider.GetComponent<Tile>();
                    Debug.Log("타일 이름_: " + curTile.name);

                    Debug.Log("isselected : " + curTile.isSelected);
                    Debug.Log("caninteract : " + canInteract);
                    Debug.Log("tiletype : " + curTile.tileType);
                    string tileName = curTile.name.Split(':')[1].Trim();
                    if (!curTile.isSelected && canInteract && curTile.tileType != TileType.RandomNotAvail)
                    {   //선택되지 않은 타일이라면 && 상호작용 가능하다면



                        //뒤집기 애니메이션 시작
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_TileFlip"]);
                        curTile._animator.SetTrigger("Select");
                        Debug.Log("지금 뒤집은 타일 이름0: " + tileName);
                        Debug.Log("지금 이름틀림0: " + tileOrderNames[currentOrderIndex]);
                        //타일 아이디 값 저장
                        _tiles.Add(curTile);
                        if (tileName == tileOrderNames[currentOrderIndex])
                        {
                            Debug.Log("지금 이름: " + tileOrderNames[currentOrderIndex]);
                            // 순서가 맞다면
                            _curSelectCnt++;
                            curTile.tileSO.selectNum = _curSelectCnt;
                            _tiles.Add(curTile);
                            currentOrderIndex++; // 다음 타일로 이동
                                                 //선택 여부 true로 변경
                            curTile.isSelected = true;

                            // 최대 5개를 모두 뒤집었을 경우 상호작용 불가 설정
                            if (_curSelectCnt >= tileOrderNames.Length)
                            {
                                canInteract = false;
                            }
                        }
                        else
                        {
                            Debug.Log("지금 뒤집은 타일 이름: " + tileName);
                            Debug.Log("지금 이름틀림: " + tileOrderNames[currentOrderIndex]);
                            isPlayerTurn = false;
                            _wrongtile.Add(curTile);
                            _tileManager.ReturnTile(_wrongtile);

                        }
                    }

                }
            }
        }
        else //npc의 턴일 경우
        {
            //  전체 타일 중 선택 가능한 타일 중 하나를 선택

            // 뒤집기

            // 만약 NPCtileOrderNames 순서에 맞지 않는 경우 다시 뒤집음
        }

    }
}