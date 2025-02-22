using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private string[] NPCtileOrderNames;
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

    // 리스트 중 빈 인덱스 번호
    
    // 뺏기 비교를 위한 리스트
    [SerializeField] private List<Tile> _plyertile = new List<Tile>();
    [SerializeField] private List<Tile> _npctile = new List<Tile>();

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

                        // 뺏기 아이템 사용 전
                        if (!isSteal)
                        {
                            if (tileName == tileOrderNames[currentOrderIndex])
                            {
                                Debug.Log("지금 이름: " + tileOrderNames[currentOrderIndex]);
                                // 순서가 맞다면
                                _curSelectCnt++;
                                curTile.tileSO.selectNum = _curSelectCnt;
                                _tiles.Add(curTile);
                                AsherSuccess = true;
                                ActivateChildObjects(currentOrderIndex);
                                AsherSuccess = false;
                                _plyertile[currentOrderIndex] = curTile;
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
                                isNPCTurn = true;
                                AddWrongTile(curTile);

                                StartCoroutine(ReturnTile(curTile));

                            }
                        }
                        // 뺏기 아이템 사용 후 돌아온 턴, 플레이어와 npc의 타일 리스트에서 empty를 찾고 그걸 우선 비교
                        else 
                        {
                             for (int i = 0; i < _plyertile.Count; i++)
                            {
                                if (_plyertile[i] == null) // 비어있는 경우(빼앗기거나 아직 안채워졌거나)
                                {
                                    if (tileName == tileOrderNames[i])
                                    {
                                        AsherSuccess = true;
                                        ActivateChildObjects(i);
                                        AsherSuccess = false;
                                        _plyertile[i] = curTile;
                                        curTile.isSelected = true;
                                        break;
                                    }
                                    else
                                    {
                                        Debug.Log("지금 뒤집은 타일 이름: " + tileName);
                                        Debug.Log("지금 이름틀림: " + tileOrderNames[currentOrderIndex]);
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
        }
        else // NPC 턴일 때
        {
            if (isNPCTurn)
            {
                isNPCTurn = false;
                StartCoroutine(NPCTurn());
                RemoveSelecting();

            }
             
        }

    }
    private IEnumerator NPCTurn()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("npc의 턴입니다");
        Tile chosenTile = null;
        // 뺏기 아이템 사용 전

        // wrongtile 리스트에서 현재 뒤집어야 하는 타일이 있는지 확인
        foreach (Tile tile in _wrongtile)
        {
            Debug.Log("wrongtile" + _wrongtile);
            Debug.Log("npc의 턴 - wrongtile 리스트 크기: " + _wrongtile.Count);
            Debug.Log("npc의 턴-뒤집기가능 타일이 wrongtile에 있는지 확인");
            Debug.Log("npc의 턴 - wrongtile 리스트에서 검사 중: " + tile.name);
            Debug.Log("npc의 턴 - wrongtile 리스트의 현재 비교 대상: " + tile.name.Split(':')[1].Trim());
            Debug.Log("npc의 턴 - NPCtileOrderNames안의 비교 대상: " + NPCtileOrderNames[currentNPCOrderIndex]);
            if (tile.name.Split(':')[1].Trim() == NPCtileOrderNames[currentNPCOrderIndex])
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
                Debug.Log("랜덤으로 하나 선택하는 부분, 선택된 타일: "+chosenTile);
            }
        }

        if (chosenTile != null)
        {
            Debug.Log("npc의 턴입니다2");
            Debug.Log("선택된 타일: " + chosenTile);
            chosenTile._animator.SetTrigger("Select");
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxDictionary["SFX_TileFlip"]);
            _tiles.Add(chosenTile);
            chosenTile.isSelected = true;

            if (chosenTile.name.Split(':')[1].Trim() == NPCtileOrderNames[currentNPCOrderIndex]) // npc가 옳은 타일 선택한 경우
            {
                Debug.Log("npc가 옳게 선택함");
                yield return new WaitForSeconds(1f);
                ActivateChildObjects(currentNPCOrderIndex);
                _plyertile[currentNPCOrderIndex] = chosenTile;
                currentNPCOrderIndex++;
                StartCoroutine(NPCTurn());
            }
            else
            {
                Debug.Log("npc가 틀림");
                yield return new WaitForSeconds(1f);
                StartCoroutine(ReturnTile(chosenTile));
                isPlayerTurn = true;
            }
        }

        else
        {
            Debug.Log("비어있는거야 뭐야");
        }

        // 뺏기 아이템 사용 후
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
