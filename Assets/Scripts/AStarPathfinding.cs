using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    public static List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos, LayerMask obstacleMask, float gridSize)
    {
        Node startNode = new Node(SnapToGrid(startPos, gridSize), null, 0, GetHeuristic(startPos, targetPos));
        Node targetNode = new Node(SnapToGrid(targetPos, gridSize), null, 0, 0);

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);
        int maxIterations = 10000; // 최대 반복 횟수 제한
        int iterations = 0;

        while (openList.Count > 0)
        {
            iterations++;
            if (iterations > maxIterations)
            {
                Debug.LogError("경로 탐색 중 최대 반복 횟수 초과");
                return null; // 경로 탐색 실패
            }

            Node currentNode = openList[0];

            // fCost와 hCost가 최소인 노드 선택
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // 목표 지점에 도달했을 때
            if (currentNode.position == targetNode.position)
            {
                return RetracePath(startNode, currentNode); // 경로 반환
            }

            // 이웃 노드 탐색
            foreach (Node neighbor in GetNeighbors(currentNode, gridSize, obstacleMask))
            {
                if (closedList.Contains(neighbor)) continue;

                float newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode.position, neighbor.position);
                if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetHeuristic(neighbor.position, targetNode.position);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        Debug.LogError("경로를 찾지 못했습니다."); // 경로를 찾을 수 없는 경우
        return null; // 경로가 없을 때 null 반환
    }

    private static List<Node> GetNeighbors(Node node, float gridSize, LayerMask obstacleMask)
    {
        List<Node> neighbors = new List<Node>();

        Vector3[] directions = {
            new Vector3(gridSize, 0, 0),
            new Vector3(-gridSize, 0, 0),
            new Vector3(0, 0, gridSize),
            new Vector3(0, 0, -gridSize)
        };

        foreach (Vector3 direction in directions)
        {
            Vector3 neighborPos = node.position + direction;
            if (!Physics.CheckSphere(neighborPos, gridSize * 0.5f, obstacleMask)) // 장애물이 없으면
            {
                neighbors.Add(new Node(neighborPos, node));
            }
        }

        return neighbors;
    }

    private static List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        path.Reverse(); // 역순으로 반환
        return path;
    }

    private static float GetDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    private static float GetHeuristic(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }

    public static Vector3 SnapToGrid(Vector3 position, float gridSize)
    {
        return new Vector3(Mathf.Round(position.x / gridSize) * gridSize, position.y, Mathf.Round(position.z / gridSize) * gridSize);
    }

    private class Node
    {
        public Vector3 position;
        public Node parent;
        public float gCost;
        public float hCost;
        public float fCost { get { return gCost + hCost; } }

        public Node(Vector3 pos, Node parent, float gCost = 0, float hCost = 0)
        {
            this.position = pos;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }
    }
}
