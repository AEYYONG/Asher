using UnityEditor;
using UnityEngine;

public class GridFeatures
{
    public GridFeatures(MapContext ctx)
    {
        ctx.gridPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ctx.prefabPath+"Grid.prefab");

        CheckGridParent(ctx);
    }

    // 씬에 Grid가 남아있다면, 갯수 계산해서 에디터 창 인풋필드와 동기화하는 함수
    void CalculateGridSize(MapContext ctx)
    {
        // grid parent 에 자식 오브젝트 없으면 0,0으로 설정
        if (ctx.gridParent.transform.childCount == 0)
        {
            ctx.gridWidth = 0;
            ctx.gridHeight = 0;
            return;
        }

        int maxX = -1;
        int maxZ = -1;
        // grid parent 자식 오브젝트 순회하면서 가장 큰 좌표 값으로 width, height 알아내기
        foreach (Transform grid in (ctx.gridParent.transform))
        {
            Vector3 pos = grid.position;

            int x = Mathf.RoundToInt(pos.x);
            int z = Mathf.RoundToInt(pos.z);

            if (x > maxX) maxX = x;
            if (z > maxZ) maxZ = z;
        }

        ctx.gridWidth = maxX + 1;
        ctx.gridHeight = maxZ + 1;
    }

    // Grid Parent 오브젝트가 있는지 없는지 체크하는 함수
    void CheckGridParent(MapContext ctx)
    {
        
    }
    
    // Grid 섹션 그리기
    public void DrawGridSection(MapContext ctx)
    {
        // 공통 스타일 적용
        GUIStyles.Ensure();
        
        EditorGUILayout.BeginVertical(GUIStyles.SectionBox);
        GUILayout.Label("Generate Grid", GUIStyles.HeaderLabel);

        ctx.gridWidth = EditorGUILayout.IntField("Grid width", ctx.gridWidth);
        ctx.gridHeight = EditorGUILayout.IntField("Grid height", ctx.gridHeight);

        //generate 버튼 클릭 시, grid 생성 함수 호출
        if (GUILayout.Button("Generate Grid", GUIStyles.MiniButton))
        {
            GenerateGrid(ctx,ctx.gridWidth,ctx.gridHeight);
        }
        //destroy 버튼 클릭 시, grid 삭제 함수 호출
        if (GUILayout.Button("Destroy Grid", GUIStyles.MiniButton))
        {
            DestroyGrid(ctx);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
    }

    // Grid Map 생성하기
    void GenerateGrid(MapContext ctx, int w, int h)
    {
        CheckGridParent(ctx);
        
        // 설정한 개수만큼 grid 생성하기
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                // grid를 gird parent의 자식으로 생성한 후, grid 리스트에 넣기
                GameObject grid = PrefabUtility.InstantiatePrefab(ctx.gridPrefab, ctx.gridParent.transform) as GameObject;
                if (grid != null)
                {
                    grid.transform.position = new Vector3(j, -0.1f, i);
                    ctx.gridList.Add(grid);
                }
            }
        }
    }
    
    // Grid Map 삭제하기
    void DestroyGrid(MapContext ctx)
    {
        // 씬에서 Grid Parent 이름의 오브젝트를 찾기
        if (ctx.gridParent == null)
        {
            Debug.Log("Grid Parent가 없습니다");
        }
        else
        {
            // Grid Parent를 삭제하며 리스트 초기화
            Object.DestroyImmediate(ctx.gridParent);
            InitGridData(ctx);
        }
    }

    void InitGridData(MapContext ctx)
    {
        ctx.gridList.Clear();
        ctx.gridWidth = 0;
        ctx.gridHeight = 0;
    }
}
