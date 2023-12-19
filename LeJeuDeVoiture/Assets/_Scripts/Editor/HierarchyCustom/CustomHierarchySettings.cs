using UnityEditor;
using UnityEngine;

public static class CustomHierarchySettings{
    public static Color backgroundColor = new (57 / 256f, 57 / 256f, 57 / 256f, 1);
    public static Color separatorColor = new (45 / 256f, 45 / 256f, 45 / 256f, 1);
    public static Color playerTagColor = new (256 / 256f, 195 / 256f, 0 / 256f, .1f);
    public static Color sceneLineColor = new (35 / 256f, 35 / 256f, 35 / 256f, 1);
    public static Color prefabColor = new (127 / 256f, 214 / 256f, 252 / 256f, 1);
    public static Color tagNameColor = new(1, 1, 1, .65f);
    
    public static Color alternateBackgroundColor = new (0, 0, 0, 20 / 256f);
    public static Color folderBackgroundColor = new (0, 0, 0, 55 / 256f);
    public static Color folderBackgroundLightColor = new (0, 0, 0, 40 / 256f);
    
    public static Color treeBranchColor = new (120 / 256f, 120 / 256f, 120 / 256f, 1);
    public static Color treeBranchPrefabColor = new (81 / 256f, 125 / 256f, 160 / 256f, 1);
    
    public static Color mouseHoverColor = new (.8f, .8f, .8f, .1f);
    public static Color objectSelectedColor = new (42 / 256f, 97 / 256f, 144 / 256f, .9f);
    public static Color objectSelectedUnfocusedColor = new (.95f, .95f, .95f, .1f);
    
    public static GUIStyle separatorInstanceStyle = new (GUI.skin.label) {
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleCenter
    };
    public static GUIStyle folderNameStyle = new (GUI.skin.label) {
        fontStyle = FontStyle.Bold,
    };
    
    public static GUIStyle tagNameStyle = new (GUI.skin.label) {
        fontSize = 9,
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleCenter
    };
    
    public static GUIStyle mainSceneStyle = new (GUI.skin.label) {
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleCenter,
        padding = new RectOffset(1, 1, 1, 1)
    };

    public static GUIStyle normalButtonStyle = new(EditorStyles.toolbarButton) {
        padding = new RectOffset(0, 0, 0, 0),
        alignment = TextAnchor.MiddleCenter,
        fixedHeight = 16
    };
}
