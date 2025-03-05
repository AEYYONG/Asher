using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Phase2PlayerInteract : MonoBehaviour
{
    //타일을 뒤집기 위한 레이캐스트
    private RaycastHit _hit;
    private Vector3 _rayPos;

    private bool canInteract = true;
    private int _curSelectCnt = 0;
    private List<Tile> _tiles = new List<Tile>();
    [SerializeField] private int maxWrongTiles = 1;
    [SerializeField] private List<Tile> _wrongtile = new List<Tile>(); // 플레이어 또는 NPC가 잘못 뒤집은 타일
    
    private TileManager _tileManager;

    [SerializeField] private string[] tileOrderNames;
    [SerializeField] private string[] NPCTileOrderNames;
    private int currentOrderIndex = 0;
    private int currentNPCOrderIndex = 0;

    // 플레이어의 순서인지
    public bool isPlayerTurn = true;
    public bool isNPCTurn = false;

    // UI 음계 표시
    public GameObject AsherPitch;
    public GameObject NPCPitch;
    private bool AsherSuccess = false;

    // 선택할 타일 표시 시각화
    public GameObject Selecting;
    private GameObject currentSelect;

    // 뺏기 아이템 사용 유무 변수
    public bool isSteal = false;
    public bool isStealDone = false;

    // 리스트 중 빈 인덱스 번호

    // 뺏기 비교를 위한 리스트
    public List<Tile> _plyertile = new List<Tile>();
    public List<Tile> _npctile = new List<Tile>();

    // 뻿기 선택한 인덱스
    public int stealIndex = 0;

    // 게임 클리어 변수
    public bool isGameOver = false;
    public bool isGameClear = false;

    // 말풍선 제어위한 스크립트
    [SerializeField] private Balloon balloon;

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
                ShowSelecting(curTile);
            }

            else
            {
                RemoveSelecting();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(ray, out _hit, 100f))
                {
                    Tile curTile = _hit.collider.GetComponent<Tile>();
                    Debug.Log("타일 이름_: " + curTile.name);
                    
                    Debug.Log("isselected : " + curTile.isSelected);
                    Debug.Log("caninteract : " +canInteract);
                    Debug.Log("tiletype : " + curTile.tileType);
                    string tileName = curTile.name.Split(':')[1].Trim();
                    if (!curTile.isSelected && canInteract && curTile.tileType != TileType.RandomNotAvail)
                    {   //선택되지 않은 타일이라면 && 상호작용 가능하다면
                        //뒤집기 애니메이션 시작
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_TileFlip"]);
                        curTile._animator.SetTrigger("Select");
                        Debug.Log("지금 뒤집은 타일 이름0: " + tileName);
                        Debug.Log("지금 이름틀림0: " + tileOrderNames[currentOrderIndex]);


                        // 플레이어와 npc의 타일 리스트에서 empty를 찾고 그걸 우선 비교
                        for (int i = 0; i < _plyertile.Count; i++)
                        {
                            if (_plyertile[i] == null) // 비어있는 경우(빼앗기거나 아직 안채워졌거나)
                            {
                                if (tileName == tileOrderNames[i])
                                {
                                     if(_plyertile.Count(Item => Item != null) == 1) // 첫번째로 맞춘 경우
                                    {
                                        balloon.isPlayer = 1;
                                        balloon.Condition = "first_find";
                                    }
                                    _tiles.Add(curTile);
                                    AsherSuccess = true;
                                    ActivateChildObjects(i);
                                    AsherSuccess = false;
                                    _plyertile[i] = curTile;
                                    curTile.isSelected = true;
                                    if (_plyertile.Count(Item => Item != null) == 5)
                                    {
                                        // 게임 종료 사인 넣기
                                        isPlayerTurn = false;
                                        isNPCTurn = false;
                                        isGameClear = true;
                                        break;
                                    }
                                    break;
                                }
                                else
                                {
                                    Debug.Log("지금 뒤집은 타일 이름: " + tileName);
                                    Debug.Log("지금 이름틀림: " + tileOrderNames[currentOrderIndex]);
                                    balloon.isPlayer = 1;
                                    balloon.Condition = "wrong_choice";
                                    isPlayerTurn = false;
                                    isNPCTurn = true;
                                    AddWrongTile(curTile);

                                    StartCoroutine(ReturnTile(curTile));
                                    break;
                                }
                            }
                        }

                    }

                }
            }
        }
        else // NPC 턴일 때
        {
            if (isNPCTurn)
            {
                isNPCTurn = false;
                StartCoroutine(NPCTurn());
                RemoveSelecting();

            }

            else if(isStealDone)// 뻿기 선택, 플레이어가 아이템을 사용해서 성공한 경우
            {
                /// *** npc 승리시 경우 추가 필요
                isStealDone = false;

                // 애셔 승리시
                NPCPitch.transform.GetChild(stealIndex).gameObject.SetActive(false);
                // 애셔 승리시 미니게임 승리 대사
                balloon.Condition = "mg_win";
                // 뺏은 타일이 필요한지 확인
                Tile stolenTile = _npctile[stealIndex]; // 뻿은 타일
                string stolenTileName = stolenTile.name.Split(':')[1].Trim();

                _npctile[stealIndex] = null;
                Debug.Log("여기는?3");
                bool tilePlaced = false;
                Debug.Log("여기는?");

                for (int i = 0; i < _plyertile.Count; i++)
                {
                    if (_plyertile[i] == null) // 빈 슬롯 발견
                    {
                        string requiredTileName = tileOrderNames[i]; // Asher가 필요로 하는 타일

                        Debug.Log($"Asher가 필요한 타일: {requiredTileName} (빈 자리 인덱스: {i})");

                        if (stolenTileName == requiredTileName)
                        {
                            Debug.Log("타일이 Asher에게 필요함! 정상적으로 가져옴.");
                            if (_plyertile.Count(Item => Item != null) == 1) // 첫번째로 맞춘 경우
                            {
                                balloon.isPlayer = 1;
                                balloon.Condition = "first_find";
                            }

                            if (_plyertile.Count(Item => Item != null) == 4) // 애셔 승리 얼마 안남은 경우
                            {
                                balloon.Condition = "almostwin";
                            }

                            _plyertile[i] = stolenTile;
                            AsherSuccess = true;
                            ActivateChildObjects(i);
                            AsherSuccess = false;
                            tilePlaced = true;
                            isPlayerTurn = true;
                            break; // 맞는 자리에 배치했으므로 종료
                        }
                        else
                        {
                            Debug.Log("현재의 타일이 Asher에게 필요하지 않음! 다시 뒤집기");

                            tilePlaced = false;
                            break;
                        }
                        
                    }

                }

                if (!tilePlaced)
                {
                    Debug.Log("타일이 Asher에게 전혀 필요하지 않음! 다시 뒤집기");

                    // 필요 없는 경우 다시 뒤집기
                    StartCoroutine(ReturnTile(stolenTile));

                    // 다시 선택 가능하도록 변수 수정
                    stolenTile.isSelected = false;
                    isPlayerTurn = true;
                    isNPCTurn = false;
                }

            }

            // 뺏기 선택, npc가 사용해서 npc가 승리한 경우
             
            // 게임 오버

            else if (isGameOver)
            {
                // 게임오버 UI 띄우기
                Debug.Log("게임오버!!");
                SceneManager.LoadScene("GameOver");
                RemoveSelecting();
            }
            // 게임 클리어 

            else if (isGameClear)
            {
                Debug.Log("게임 클리어!");
                SceneManager.LoadScene("GameClear");
            }
        }

    }
    public void SetStealIndex(int index)
    {
        stealIndex = index;
        isPlayerTurn = false;
    }


    private IEnumerator NPCTurn()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("npc의 턴입니다");
        Tile chosenTile = null;
        // 뺏기 아이템 사용 전
        for (int i = 0; i < _npctile.Count; i++)
        {
            if (_npctile[i] == null)
            {
                Debug.Log("*** 빈 타일의 순서는 :" + i);
                foreach (Tile tile in _wrongtile)
                {
                    Debug.Log("npc의 턴-뒤집기가능 타일이 wrongtile에 있는지 확인");
                    Debug.Log("npc의 턴 - wrongtile 리스트에서 검사 중: " + tile.name);
                    Debug.Log("npc의 턴 - wrongtile 리스트의 현재 비교 대상: " + tile.name.Split(':')[1].Trim());
                    Debug.Log("npc의 턴 - NPCtileOrderNames안의 비교 대상: " + NPCTileOrderNames[i]);
                    if (tile.name.Split(':')[1].Trim() == NPCTileOrderNames[i])
                    {

                        chosenTile = tile;
                        Debug.Log("wrongtile리스트에서 찾음!" + chosenTile);
                        _wrongtile.Remove(chosenTile);
                        break;
                    }
                }

                if (chosenTile == null)
                {
                    // 상호작용 가능 타일 랜덤으로 하나 선택
                    Debug.Log("npc의 턴-랜덤으로 하나 선택");
                    Tile[] allTiles = FindObjectsOfType<Tile>();
                    List<Tile> availableTiles = new List<Tile>();

                    foreach (Tile tile in allTiles)
                    {
                        if (!tile.isSelected && tile.tileType != TileType.RandomNotAvail)
                        {
                            availableTiles.Add(tile);
                        }
                    }

                    if (availableTiles.Count > 0)
                    {

                        chosenTile = availableTiles[Random.Range(0, availableTiles.Count)];
                        Debug.Log("랜덤으로 하나 선택하는 부분, 선택된 타일: " + chosenTile);
                    }
                }

                if (chosenTile != null)
                {
                    Debug.Log("npc의 턴입니다2");
                    Debug.Log("선택된 타일: " + chosenTile);
                    chosenTile._animator.SetTrigger("Select");
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_TileFlip"]);
                    _tiles.Add(chosenTile);
                    

                    if (chosenTile.name.Split(':')[1].Trim() == NPCTileOrderNames[i]) // npc가 옳은 타일 선택한 경우
                    {
                        Debug.Log("npc가 옳게 선택함");
                        if (_npctile.Count(Item => Item != null) == 1) // 첫번째로 맞춘 경우
                        {
                            balloon.isPlayer = 2;
                            balloon.Condition = "first_find";
                        }

                        if (_npctile.Count(Item => Item != null) == 4) // npc 승리 얼마 안남은 경우
                        {
                            balloon.Condition = "almostlose";
                        }
                        chosenTile.isSelected = true;
                        yield return new WaitForSeconds(1f);
                        ActivateChildObjects(i);
                        _npctile[i] = chosenTile;
                        Debug.Log("***npc 타일 갯수 :" + _npctile.Count);
                        Debug.Log("***npc 타일 null이 아닌 갯수 :" + _npctile.Count(Item => Item != null));

                        // npc가 리스트의 개수와 지정한 리스트의 수가 동일한지 확인
                        if(_npctile.Count(Item => Item != null) == 5)
                        {
                            // 게임 종료 사인 넣기
                            isPlayerTurn = false;
                            isNPCTurn = false;
                            isGameOver = true;
                            break;
                        }
                        StartCoroutine(NPCTurn());
                        break;
                    }
                    else
                    {
                        Debug.Log("npc가 틀림");
                        balloon.isPlayer = 2; // 틀린 타일을 뒤집었을 때
                        balloon.Condition = "wrong_choice";
                        yield return new WaitForSeconds(1f);
                        StartCoroutine(ReturnTile(chosenTile));
                        isPlayerTurn = true;
                        break;
                    }
                }

                else
                {

                    isPlayerTurn = true;

                    StartCoroutine(ReturnTile(chosenTile));
                    break;
                }

            }
        }
       
        
    }


    private IEnumerator ReturnTile(Tile tile)
    {
        yield return new WaitForSeconds(1f);
        tile._animator.SetTrigger("Return");
    }

    private void AddWrongTile(Tile tile)
    {
        if (_wrongtile.Count >= maxWrongTiles)
        {
            _wrongtile.RemoveAt(0); // 가장 오래된 타일 제거
        }
        _wrongtile.Add(tile);
    }
    // 음계 표시
    private void ActivateChildObjects(int index)
    {
        if (AsherSuccess) // 애셔가 맞춘 경우
        {
            if (index < AsherPitch.transform.childCount)
            {
                AsherPitch.transform.GetChild(index).gameObject.SetActive(true);
            }
        }
        

        else //npc가 맞춘 경우
        {
            if (index < NPCPitch.transform.childCount)
            {
                NPCPitch.transform.GetChild(index).gameObject.SetActive(true);
            }
        }
    }

    // 선택할 타일 표시
    private void ShowSelecting(Tile tile)
    {
        if (currentSelect == null)
        {
            currentSelect = Instantiate(Selecting, tile.transform.position + new Vector3(0, 0.1f, 0), Quaternion.Euler(90, 0, 0));
        }
        else
        {
            currentSelect.transform.position = tile.transform.position + new Vector3(0, 0.1f, 0);
        }
    }

    private void RemoveSelecting()
    {
        Destroy(currentSelect);
        currentSelect = null;
    }


}
