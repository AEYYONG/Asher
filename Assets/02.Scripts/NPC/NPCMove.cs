using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _z) { isWall = _isWall; x = _x; z = _z; }

    public bool isWall;
    public Node ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, z, G, H;
    public int F { get { return G + H; } }
}

public class NPCMove : MonoBehaviour
{
    public int state = 1;

    public Vector3Int bottomLeft, topRight; // Vector3Int로 변경
    public List<Node> FinalNodeList;
    public float moveSpeed = 1f; // NPC 이동 속도
    private TileManager tileManager; // TileManager 참조

    public Transform target; // 추적할 목표
    public Transform start;
    public Vector3Int targetPos; // 충돌한 오브젝트의 위치를 저장
    public Vector3Int startPos;


    private DetectingPlayer detectingPlayer;


    int sizeX, sizeZ;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;

    private Coroutine moveCoroutine;

    void Start()
    {
        detectingPlayer = GetComponentInChildren<DetectingPlayer>();

        tileManager = FindObjectOfType<TileManager>(); // TileManager 가져오기
        startPos = new Vector3Int(
           Mathf.FloorToInt(start.position.x),
           Mathf.FloorToInt(start.position.y),
           Mathf.FloorToInt(start.position.z)
        );

        targetPos = new Vector3Int(
           Mathf.FloorToInt(target.position.x),
           Mathf.FloorToInt(target.position.y),
           Mathf.FloorToInt(target.position.z)
        );

        PathFinding(); // 경로 탐색 시작
        StartCoroutine(MoveAlongPath()); // 경로에 따라 NPC 이동
    }

    public void StartPathFinding()
    {
        PathFinding();
        moveCoroutine = StartCoroutine(MoveAlongPath());
    }

    public void StopMoving()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
    }



    private void Update()
    {
        SwitchUpdate();
    }
    void SwitchUpdate()
    {
        switch (state)
        {
            // idle 상태에서는 움직이지 않음
            case 1:
                break;

            // chase 상태: 충돌한 오브젝트 위치로 이동
            case 2:
             
                GameObject FindObject = GameObject.Find("Asher");
                target = FindObject.transform;
                


                // targetPos를 갱신 후 A* 경로 탐색
                targetPos = new Vector3Int(
                    Mathf.FloorToInt(target.position.x),
                    Mathf.FloorToInt(target.position.y-0.5f),
                    Mathf.FloorToInt(target.position.z - 0.5f)
                );
                Debug.Log("타겟의 위치: " + targetPos);

                // A* 경로 탐색
                PathFinding();
                StartCoroutine(MoveAlongPath());
                break;

            // backtrack 상태: 출발점으로 이동
            case 3:
               

                // startPos로 복귀
                // start 컴포넌트 = Sphere
                StopMoving();


                StartPathFinding();
                break;
        }
    }



    public void PathFinding()
    {
        // NodeArray의 크기 정해주고, isWall, x, z 대입
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeZ = topRight.z - bottomLeft.z + 1;
        NodeArray = new Node[sizeX, sizeZ];

        FinalNodeList.Clear();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                // 타일의 위치에 따라 벽을 감지
                Vector3Int tilePosition = new Vector3Int(i + bottomLeft.x, 0, j + bottomLeft.z); // z를 기준으로 수정
                bool isWall = tileManager._tiles.ContainsKey(new Vector2Int(tilePosition.x, tilePosition.z)) && tileManager._tiles[new Vector2Int(tilePosition.x, tilePosition.z)].CompareTag("Wall");

                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.z);
            }
        }

        // 시작과 끝 노드 설정
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.z - bottomLeft.z];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.z - bottomLeft.z];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        // A* 알고리즘
        while (OpenList.Count > 0)
        {
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
                return;
            }

            OpenListAdd(CurNode.x, CurNode.z + 1); // 위쪽
            OpenListAdd(CurNode.x + 1, CurNode.z); // 오른쪽
            OpenListAdd(CurNode.x, CurNode.z - 1); // 아래쪽
            OpenListAdd(CurNode.x - 1, CurNode.z); // 왼쪽
        }
    }

    void OpenListAdd(int checkX, int checkZ)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서 닫힌 리스트에 없을 경우
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkZ >= bottomLeft.z && checkZ < topRight.z + 1 && !NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z]))
        {
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z];

            int MoveCost = CurNode.G + 10;
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.z - TargetNode.z)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }

    // NPC가 경로에 따라 이동하는 코루틴
    IEnumerator MoveAlongPath()
    {
        List<Node> pathCopy = new List<Node>(FinalNodeList);
        Node currentNode = null;

        foreach (Node node in pathCopy)
        {
            currentNode = node;

            Vector3 targetPosition = new Vector3(node.x, 0, node.z); // 3D 공간에서 타일 좌표로 이동

            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;

                if (detectingPlayer.isCollidingWithAsher)
                {
                    startPos = new Vector3Int(node.x, 0, node.z); // 현재 목표 노드 좌표를 startPos로 설정
                    Debug.Log("충돌 시 목표 노드 좌표로 startPos 설정: " + startPos);
                    FinalNodeList.Clear();
                    yield break;
                }
            }
        }

        if (state == 2)
        {
            if(detectingPlayer.isCollidingWithAsher){
                Debug.Log("충돌중, 추적을 계속합니다");
                state = 2; 
            }
            else
            {
                GameObject sphereObject = GameObject.Find("Sphere");
                if (sphereObject != null)
                {
                    Vector3 spherePosition = sphereObject.transform.position;
                    targetPos = (new Vector3Int(
                        Mathf.FloorToInt(spherePosition.x),
                        Mathf.FloorToInt(spherePosition.y),
                        Mathf.FloorToInt(spherePosition.z)
                    ));


                    startPos = new Vector3Int(currentNode.x, 0, currentNode.z);

                }
                state = 3;
            }

        }

        if (state == 3)
        {
            state = 1;
        }
    }

    void OnDrawGizmos()
    {
        if (FinalNodeList.Count > 0)
        {
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector3(FinalNodeList[i].x, 0, FinalNodeList[i].z), new Vector3(FinalNodeList[i + 1].x, 0, FinalNodeList[i + 1].z));
            }
        }
    }
}