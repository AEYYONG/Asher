using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MapGenerator : EditorWindow
{
    private MapContext _ctx;
    private GridFeatures _gridSection;
    private TileFeatures _tileSection;
    private TilePaintFeatures _tilePaintSection;
    private TileTypes _tileTypesSection;
    private CameraFeatures _cameraSection;
    
    private Vector2 scrollPos;

    
    //윈도우 메뉴에 "Map Generator"
    [MenuItem("Window/Map Generator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapGenerator));
    }

    private void OnEnable()
    {
        if (_ctx == null)
        {
            _ctx = new MapContext();
        }
        
        _gridSection = new GridFeatures(_ctx);
        _tileSection = new TileFeatures(_ctx);
        _tilePaintSection = new TilePaintFeatures(_ctx);
        _tileTypesSection = new TileTypes();
        _cameraSection = new CameraFeatures();
        

        // 씬 뷰에서 이벤트를 수신하기 위해 duringSceneGui 이벤트에 핸들러 추가
        SceneView.duringSceneGui += OnSceneGUI;
    }

    //커스텀 에디터 창이 끌 때
    private void OnDisable()
    {
        //타일 매니저에 해당 타일의 정보 저장
        _tileSection.SaveTileData(_ctx);
        _tilePaintSection.DestroyTexParent(_ctx);
        
        EditorUtility.SetDirty(_ctx.tileManager);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        
        // 이벤트 핸들러 제거
        SceneView.duringSceneGui -= OnSceneGUI;
    }
    
    void OnSceneGUI(SceneView sceneView)
    {
        // 마우스 클릭 감지
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0) // 마우스 왼쪽 버튼 클릭
        {
            if (_ctx.curDrawMode != DrawMode.DEFAULT)
            {
                _tilePaintSection.DrawTile(e.mousePosition, _ctx);
            }

            if (_ctx.curTypeMode != TypeMode.DEFAULT)
            {
                _tilePaintSection.SetTileType(e.mousePosition, _ctx);
            }
            e.Use(); // 이벤트 처리 완료 표시
        }
    }

    private void OnGUI()
    {
        //스크롤 포지션 할당
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        _gridSection.DrawGridSection(_ctx);
        _tileSection.DrawTileSection(_ctx);
        _tilePaintSection.DrawTilePaintFeatures(_ctx, this);
        _tileTypesSection.DrawTileTypesSection(_ctx,this);
        _cameraSection.DrawCameraSection(_ctx);
        
        EditorGUILayout.EndScrollView();
    }
    

    public void AddTileType(GameObject obj)
    {
        _tileTypesSection.AddTileType(_ctx,obj);
    }
}
