using UnityEditor;
using UnityEngine;

public static class GUIStyles
{
    public static GUIStyle SectionBox { get; private set; }
    public static GUIStyle HeaderLabel { get; private set; }
    public static GUIStyle MiniButton { get; private set; }
    public static GUIStyle RowBox { get; private set; }
    
    public static Texture2D EmptyTex { get; private set; }

    public static void Ensure()
    {
        if (SectionBox != null) return;

        SectionBox = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 10, 8, 10)
        };

        HeaderLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12
        };

        MiniButton = new GUIStyle(GUI.skin.button)
        {
            fixedHeight = 22
        };

        RowBox = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(6, 6, 6, 6),
            margin  = new RectOffset(0, 0, 4, 4)
        };
        
        if (EmptyTex != null) return;
        EmptyTex = MakeTex(70, 70, new Color(82, 82, 82, 1));
    }
    
    private static Texture2D MakeTex(int w, int h, Color col)
    {
        var tex = new Texture2D(w, h);
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.SetPixel(0, 0, col);
        tex.Apply();
        return tex;
    }
}
