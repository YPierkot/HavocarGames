using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilingSetter))]
public class TilingSetterEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update")) ((TilingSetter)target).SetTiling();
    }
}