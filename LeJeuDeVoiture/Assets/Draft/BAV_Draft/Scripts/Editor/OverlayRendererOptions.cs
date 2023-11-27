using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor.EditorTools;
using UnityEditor.Toolbars;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor;

[EditorToolbarElement(id, typeof(SceneView))]
class OutlineRenderToggle : EditorToolbarToggle
{
    public const string id = "OutlineRender/OutlineRenderToggle";
    public OutlineRenderToggle()
    {
        text = "Outline ON";
        this.RegisterValueChangedCallback(OutlineRenderState);
    }

    void OutlineRenderState(ChangeEvent<bool> evt)
    {
        text = evt.newValue ? "Outline ON" : "Outline OFF";
        ApplyCustomDrawMode(evt.newValue);
    }
    
    private static void ApplyCustomDrawMode(bool isEnabled)
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("RenderObjects", isEnabled);
    }
}

[Overlay(typeof(SceneView), "Renderers Options")]
[Icon("Assets/unity.png")]

public class EditorToolbarExample : ToolbarOverlay
{
    EditorToolbarExample() : base(
        OutlineRenderToggle.id
    )
    { }
}