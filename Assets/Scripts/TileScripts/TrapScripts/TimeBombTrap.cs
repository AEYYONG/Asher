using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBombTrap : Tile
{
    private float _timer = 0;
    private bool _isCountDownStart = false;
    private bool _isCompleteMission = false;
    
    private Transform _playerTransform;
    private GameObject _selectTile;
    private Tile _tile;
    
    private Vector2Int _targetPos;
    private RaycastHit _hit;
    private Vector3 _rayPos;

    private StageUIManager _uiManager;
    
    void Update()
    {
        if (_isCountDownStart && !_isCompleteMission)
        {
            _rayPos = new Vector3(_playerTransform.position.x, _playerTransform.position.y, _playerTransform.position.z - 0.5f);
            Debug.DrawRay(_rayPos,Vector3.down * 1f, Color.red);
            //정해진 타일 위치로 이동하여 타일을 뒤집는다면
            if (Physics.Raycast(_rayPos, Vector3.down, out _hit, 1f))
            {
                if (Input.GetButtonUp("FlipTile") && _hit.collider.GetComponent<Tile>().ReturnPos() == _targetPos)
                {
                    _isCompleteMission = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (_isCountDownStart)
        {
            _timer += Time.fixedDeltaTime;

            if (_isCompleteMission)
            {
                Debug.Log("정해진 시간 내에 타일 뒤집기를 완수함");
                //뒤집은 타일 원상복귀
                _selectTile.GetComponent<Renderer>().material.SetTexture("_TopTex",_tile.tileSO.originTopTex);
                _isCompleteMission = false;
                _isCountDownStart = false;
            }

            if (_timer >= tileSO.duration)
            {
                Debug.Log("정해진 시간 내에 타일을 뒤집지 못함");
                //뒤집은 타일 원상복귀
                _selectTile.GetComponent<Renderer>().material.SetTexture("_TopTex",_tile.tileSO.originTopTex);
                ResetTiles();
                _isCountDownStart = false;
            }
        }
    }
    
    //리셋 함수
    void ResetTiles()
    {
        //타일 리셋
        foreach (var tile in tileManager._tiles)
        {
            Tile tileScript = tile.Value.GetComponent<Tile>();
            if (tileScript.isSelected)
            {
                StartCoroutine(tileScript.ReturnTile());
                StartCoroutine(playerInteract.InvokeInitValue());
            }
        }
        //마음의 조각 리셋
        _uiManager.LoseAllHeartStones();
    }

    public override void TrapUse(StageUIManager uiManager)
    {
        base.TrapUse(uiManager);
        _playerTransform = uiManager.player.transform;
        _uiManager = uiManager;
        Debug.Log("시한폭탄 아이템 사용");
        
        //일반 타일 중 랜덤으로 하나 선택
        int random = Random.Range(0, tileManager.generalTileList.Count);
        _selectTile = tileManager.generalTileList[random];
        _tile = _selectTile.GetComponent<Tile>();
        _targetPos = _tile.ReturnPos();
        
        //선택한 타일의 윗면을 빛나는 텍스쳐로 변경
        _selectTile.GetComponent<Renderer>().material.SetTexture("_TopTex",_tile.tileSO.timeBombTopTex);
        
        //제한 시간 시작
        _timer = 0f;
        _isCountDownStart = true;
        
        //vfx 실행
        Animator effectAnimator = transform.GetChild(0).GetComponent<Animator>();
        effectAnimator.SetTrigger("TrapMatch");
    }
}
