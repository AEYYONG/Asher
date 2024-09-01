using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isSelected;
    [SerializeField] private Animator _animator;
    private TileManager _tileManager;

    void Awake()
    {
        isSelected = false;
        _animator = GetComponent<Animator>();
        _tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
    }

    void OnMouseDown()
    {
        if (!isSelected)
        {
            Debug.Log("Clicked");
            isSelected = true;
            //뒤집기 애니메이션 시작
            _animator.SetTrigger("Select");
            StartCoroutine(TileReturn());
        }
    }

    IEnumerator TileReturn()
    {
        yield return new WaitForSeconds(_tileManager.tileReturnTime);
        _animator.SetTrigger("Return");
        isSelected = false;
    }
}
