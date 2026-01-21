using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapContext
{
    //Grid 생성
    public int gridWidth; //grid 가로 개수
    public int gridHeight; //grid 세로 개수 
    public GameObject gridPrefab;
    public GameObject gridParent; //생성된 grid들이 들어갈 부모 오브젝트
    public List<GameObject> gridList = new List<GameObject>(); //grid 오브젝트들을 저장할 리스트
}
