using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _z) { isWall = _isWall; x = _x; z = _z; }

    public bool isWall;
    public Node ParentNode;

    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
    public int x, z, G, H;
    public int F { get { return G + H; } }
}

public class NPCMove : MonoBehaviour
{
    public int state = 1;

    public Vector3Int bottomLeft, topRight; // Vector3Int�� ����
    public List<Node> FinalNodeList;
    public float moveSpeed = 1f; // NPC �̵� �ӵ�
    private TileManager tileManager; // TileManager ����

    public Transform target; // ������ ��ǥ
    public Transform start;
    public Vector3Int targetPos; // �浹�� ������Ʈ�� ��ġ�� ����
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

        tileManager = FindObjectOfType<TileManager>(); // TileManager ��������
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

        PathFinding(); // ��� Ž�� ����
        StartCoroutine(MoveAlongPath()); // ��ο� ���� NPC �̵�
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
            // idle ���¿����� �������� ����
            case 1:
                break;

            // chase ����: �浹�� ������Ʈ ��ġ�� �̵�
            case 2:
             
                GameObject FindObject = GameObject.Find("Asher");
                target = FindObject.transform;
                


                // targetPos�� ���� �� A* ��� Ž��
                targetPos = new Vector3Int(
                    Mathf.FloorToInt(target.position.x),
                    Mathf.FloorToInt(target.position.y-0.5f),
                    Mathf.FloorToInt(target.position.z - 0.5f)
                );
                Debug.Log("Ÿ���� ��ġ: " + targetPos);

                // A* ��� Ž��
                PathFinding();
                StartCoroutine(MoveAlongPath());
                break;

            // backtrack ����: ��������� �̵�
            case 3:
               

                // startPos�� ����
                // start ������Ʈ = Sphere
                StopMoving();


                StartPathFinding();
                break;
        }
    }



    public void PathFinding()
    {
        // NodeArray�� ũ�� �����ְ�, isWall, x, z ����
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeZ = topRight.z - bottomLeft.z + 1;
        NodeArray = new Node[sizeX, sizeZ];

        FinalNodeList.Clear();

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                // Ÿ���� ��ġ�� ���� ���� ����
                Vector3Int tilePosition = new Vector3Int(i + bottomLeft.x, 0, j + bottomLeft.z); // z�� �������� ����
                bool isWall = tileManager._tiles.ContainsKey(new Vector2Int(tilePosition.x, tilePosition.z)) && tileManager._tiles[new Vector2Int(tilePosition.x, tilePosition.z)].CompareTag("Wall");

                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.z);
            }
        }

        // ���۰� �� ��� ����
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.z - bottomLeft.z];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.z - bottomLeft.z];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        // A* �˰���
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

            OpenListAdd(CurNode.x, CurNode.z + 1); // ����
            OpenListAdd(CurNode.x + 1, CurNode.z); // ������
            OpenListAdd(CurNode.x, CurNode.z - 1); // �Ʒ���
            OpenListAdd(CurNode.x - 1, CurNode.z); // ����
        }
    }

    void OpenListAdd(int checkX, int checkZ)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭 ���� ����Ʈ�� ���� ���
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

    // NPC�� ��ο� ���� �̵��ϴ� �ڷ�ƾ
    IEnumerator MoveAlongPath()
    {
        List<Node> pathCopy = new List<Node>(FinalNodeList);
        Node currentNode = null;

        foreach (Node node in pathCopy)
        {
            currentNode = node;

            Vector3 targetPosition = new Vector3(node.x, 0, node.z); // 3D �������� Ÿ�� ��ǥ�� �̵�

            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;

                if (detectingPlayer.isCollidingWithAsher)
                {
                    startPos = new Vector3Int(node.x, 0, node.z); // ���� ��ǥ ��� ��ǥ�� startPos�� ����
                    Debug.Log("�浹 �� ��ǥ ��� ��ǥ�� startPos ����: " + startPos);
                    FinalNodeList.Clear();
                    yield break;
                }
            }
        }

        if (state == 2)
        {
            if(detectingPlayer.isCollidingWithAsher){
                Debug.Log("�浹��, ������ ����մϴ�");
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