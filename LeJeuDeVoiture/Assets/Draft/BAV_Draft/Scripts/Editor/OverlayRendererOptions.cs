using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor.EditorTools;
using UnityEditor.Toolbars;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor;


public enum FrameRateOption
{
    Default,
    FPS30,
    FPS60,
    FPS120
}
[EditorToolbarElement(id, typeof(SceneView))]
class QualitySettingsToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
{
    public const string id = "ExampleToolbar/QualitySettingsToggle";

    public EditorWindow containerWindow { get; set; }

    QualitySettingsToggle()
    {
        UpdateToggleText();
        tooltip = "Select quality settings.";

        dropdownClicked += ShowQualitySettingsMenu;

        // Additional setup, if needed
    }

    void ShowQualitySettingsMenu()
    {
        var menu = new GenericMenu();

        string[] qualityOptions = QualitySettings.names;
        for (int i = 0; i < qualityOptions.Length; i++)
        {
            int index = i; // Capture the index in the closure
            menu.AddItem(new GUIContent(qualityOptions[i]), false, () => SetQualitySettings(index));
        }

        menu.ShowAsContext();
    }

    void SetQualitySettings(int index)
    {
        QualitySettings.SetQualityLevel(index);
        UpdateToggleText();
    }

    void UpdateToggleText()
    {
        int selectedQualityIndex = QualitySettings.GetQualityLevel();
        text = $"Quality: {QualitySettings.names[selectedQualityIndex]}";
    }
}


[EditorToolbarElement(id, typeof(SceneView))]
class FrameRateToggle : EditorToolbarDropdownToggle, IAccessContainerWindow
{
    public const string id = "OutlineRender/FrameRateToggle";

    public EditorWindow containerWindow { get; set; }

    private FrameRateOption selectedOption = FrameRateOption.Default;

    FrameRateToggle()
    {
        UpdateToggleText();
        tooltip = "Select a frame rate option.";

        dropdownClicked += ShowFrameRateMenu;
    }

    void ShowFrameRateMenu()
    {
        var menu = new GenericMenu();
        AddMenuItem(menu, FrameRateOption.Default, "Default");
        AddMenuItem(menu, FrameRateOption.FPS30, "30 FPS");
        AddMenuItem(menu, FrameRateOption.FPS60, "60 FPS");
        AddMenuItem(menu, FrameRateOption.FPS120, "120 FPS");

        menu.ShowAsContext();
    }

    void AddMenuItem(GenericMenu menu, FrameRateOption option, string optionText)
    {
        menu.AddItem(new GUIContent(optionText), option == selectedOption, () => SetApplicationFrameRate(option));
    }

    void SetApplicationFrameRate(FrameRateOption option)
    {
        selectedOption = option;
        UpdateToggleText();

        switch (option)
        {
            case FrameRateOption.Default:
                Application.targetFrameRate = -1;
                break;
            case FrameRateOption.FPS30:
                Application.targetFrameRate = 30;
                break;
            case FrameRateOption.FPS60:
                Application.targetFrameRate = 60;
                break;
            case FrameRateOption.FPS120:
                Application.targetFrameRate = 120;
                break;
        }
    }

    void UpdateToggleText()
    {
        text = $"Frame Rate: {selectedOption}";
    }
}
[EditorToolbarElement(id, typeof(SceneView))]
class OutlineRenderToggle : EditorToolbarToggle
{
    public const string id = "OutlineRender/OutlineRenderToggle";
    private RendererToolbar toolbar; // Reference to the ToolbarOverlay


    public OutlineRenderToggle()
    {
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Draft/BAV_Draft/Icon/Icon_KC.png");
        text = "Outline ON";
        tooltip = "Enable or disable outline rendering.";
        
        // Register the value changed callback
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




[EditorToolbarElement(id, typeof(SceneView))]
class GrayscaleRenderToggle : EditorToolbarToggle
{
    public const string id = "OutlineRender/GrayscaleRenderToggle";
    private RendererToolbar toolbar;

    public GrayscaleRenderToggle()
    {
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Draft/BAV_Draft/Icon/Icon_KC.png");
        text = "Grayscale OFF";
        tooltip = "Enable or disable grayscale rendering.";
        
        this.RegisterValueChangedCallback(OutlineRenderState);
    }

    void OutlineRenderState(ChangeEvent<bool> evt)
    {
        text = evt.newValue ? "Grayscale ON" : "Grayscale OFF";
        ApplyCustomDrawMode(evt.newValue);
    }

    private static void ApplyCustomDrawMode(bool isEnabled)
    {
        UniversalRenderPipelineUtils.SetRendererFeatureActive("FullScreenPassRendererFeature", isEnabled);
    }
}



[Overlay(typeof(SceneView), "Renderers Options")]
[Icon("Assets/unity.png")]
public class RendererToolbar : ToolbarOverlay
{
    private List<string> elementIds = new List<string>();

    RendererToolbar() : base(
        OutlineRenderToggle.id,
        FrameRateToggle.id,
        QualitySettingsToggle.id,
        GrayscaleRenderToggle.id
    )
    {  }
    
    protected override Layout supportedLayouts => Layout.HorizontalToolbar | Layout.VerticalToolbar;
}

