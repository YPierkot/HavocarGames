using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using PrefabStageUtility = UnityEditor.SceneManagement.PrefabStageUtility;

/// <summary>
/// Class to draw custom element inside the Hierarchy
/// </summary>
[InitializeOnLoad]
public class HierarchyDrawer {
    private static bool useCustomHierarchy = true;
    
    private static List<InstanceInfos> sceneObjectInformations = new List<InstanceInfos>();
    private static InstanceInfos actualInstance = new InstanceInfos();

    private static bool drawDarkerBackground = true;
    private static float selectionRectY = 0;
    
    #region MethodInit
    /// <summary>
    /// Method called on Initialization
    /// </summary>
    static HierarchyDrawer() {
        ResetHierarchyDrawer();
    }

    /// <summary>
    /// Reset the Hierarchy
    /// </summary>
    private static void ResetHierarchyDrawer() {
        CustomHierarchyInit();
        InitHierarchyEvents();
    }
    
    /// <summary>
    /// Initialize the CustomHierarchy
    /// </summary>
    private static void CustomHierarchyInit() {
        sceneObjectInformations.Clear();
        sceneObjectInformations = CustomHierarchyHelper.GetObjectElementFromScene();
    }

    /// <summary>
    /// Initialize all the events for the Hierarchy
    /// </summary>
    private static void InitHierarchyEvents() {
        //Remove the methods from the events if they are already in
        EditorApplication.hierarchyChanged -= CustomHierarchyInit;
        EditorApplication.hierarchyChanged -= EditorApplication.RepaintHierarchyWindow;
        EditorApplication.hierarchyWindowItemOnGUI -= DrawCustomHierarchy;
        Selection.selectionChanged -= ResetHierarchyDrawer;

        //Add the methods to the events
        EditorApplication.hierarchyChanged += CustomHierarchyInit;
        EditorApplication.hierarchyChanged += EditorApplication.RepaintHierarchyWindow;
        EditorApplication.hierarchyWindowItemOnGUI += DrawCustomHierarchy;
        Selection.selectionChanged += ResetHierarchyDrawer;
    }
    #endregion MethodInit
    
    /// <summary>
    /// Draw the Hierarchy
    /// </summary>
    /// <param name="instanceID"></param>
    /// <param name="selectionRect"></param>
    private static void DrawCustomHierarchy(int instanceID, Rect selectionRect) {
        if (sceneObjectInformations.Count == 0 || PrefabStageUtility.GetCurrentPrefabStage() != null) return;
        if(selectionRect.y < selectionRectY) drawDarkerBackground = true;
        if (EditorUtility.InstanceIDToObject(instanceID) == null) {
            drawDarkerBackground = true;
            DrawToolbar(instanceID, selectionRect);
            return;
        }

        if (Event.current.type != EventType.Repaint) return;
        if (!useCustomHierarchy) return;

        selectionRectY = selectionRect.y;
        actualInstance = sceneObjectInformations.GetInstanceFromID(instanceID);

        actualInstance.drawingRect = selectionRect.GetDrawingRect(actualInstance);
        actualInstance.fullSizeRect = selectionRect.GetFullsizeRect(actualInstance);

        DrawBackground(actualInstance, selectionRect);
        DrawForeground(actualInstance, selectionRect);
        DrawTreeObject(actualInstance, selectionRect);
    }
    
    #region Toolbar
    /// <summary>
    /// Draw the toolbar elements
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawToolbar(int instanceID, Rect selectionRect) {
        if (EditorSceneManager.GetSceneAt(0).GetHashCode() == instanceID) EnableOrDisableCustomHierarchy(selectionRect);
        if (EditorSceneManager.GetActiveScene().GetHashCode() == instanceID) DrawMainActivScene(selectionRect);
    }
    
    /// <summary>
    /// Draw a toggle button to press to enable or disable the custom hierarchy
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void EnableOrDisableCustomHierarchy(Rect selectionRect) {
        GUI.backgroundColor = useCustomHierarchy ? Color.green : Color.red;
        useCustomHierarchy = GUI.Toggle( selectionRect.GetVisibiltyButtonRect(), useCustomHierarchy, new GUIContent(EditorGUIUtility.IconContent(useCustomHierarchy?"d_animationvisibilitytoggleon@2x" : "d_animationvisibilitytoggleoff@2x")), CustomHierarchySettings.normalButtonStyle);
        GUI.backgroundColor = Color.white;
        EditorGUI.DrawRect(selectionRect.GetFullsizeRect(actualInstance).GetLineRectBottom(1), CustomHierarchySettings.sceneLineColor);
    }

    /// <summary>
    /// Draw a star after the name of the scene which is the activ scene
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawMainActivScene(Rect selectionRect) {
        GUI.color = new Color(255f/255f,200/255f,0, 1);
        GUI.Label(selectionRect.GetMainSceneRect(EditorSceneManager.GetActiveScene().name), new GUIContent(EditorGUIUtility.IconContent("Favorite Icon").image, "this is the activ scene"), CustomHierarchySettings.mainSceneStyle);
        GUI.color = Color.white;
    }
    #endregion Toolbar
    
    #region Background
    /// <summary>
    /// Draw everything in the background
    /// </summary>
    /// <param name="instance"></param>
    private static void DrawBackground(InstanceInfos instance, Rect selectionRect) {
        if (instance.Gam == null) return;
        EraseGamText(instance, selectionRect);
        DrawFolderBackground(instance);
        DrawAlternateBackground(instance);
        DrawCustomTagBackground(instance);
        DrawInstanceName(instance, selectionRect);
        DrawStrikeForName(instance, selectionRect.GetRectWithoutIcon());
        
        drawDarkerBackground = !drawDarkerBackground;
    }

    /// <summary>
    /// Add a Rect over all the object in the Hierarchy
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void EraseGamText(InstanceInfos instance, Rect selectionRect) {
        switch (instance.IsSeparator) {
            //Normal GameObject
            case false when instance.Name != " ":
                EditorGUI.DrawRect(selectionRect, CustomHierarchySettings.backgroundColor);
                DrawSelectionRect(instance, selectionRect);
                if(!instance.IsObjectFirstRootObjectOfHisScene()) EditorGUI.DrawRect(instance.fullSizeRect.GetLineRect(1), CustomHierarchySettings.separatorColor);
                break;
            //GameObject just before Separator
            case false when instance.Name == " ":
                EditorGUI.DrawRect(instance.fullSizeRect, CustomHierarchySettings.backgroundColor);
                EditorGUI.DrawRect(instance.fullSizeRect.GetLineRect(), CustomHierarchySettings.separatorColor);
                break;
            //Separator GameObject
            case true:
                EditorGUI.DrawRect(instance.fullSizeRect, CustomHierarchySettings.separatorColor);
                break;
        }
    }

    /// <summary>
    /// Add a background Rect to the object of tag player
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawCustomTagBackground(InstanceInfos instance) {
        if (instance.Gam != null && instance.Gam.CompareTag("Player")) EditorGUI.DrawRect(instance.drawingRect, CustomHierarchySettings.playerTagColor);
    }

    /// <summary>
    /// Draw custom background for folders
    /// </summary>
    /// <param name="instance"></param>
    private static void DrawFolderBackground(InstanceInfos instance) {
        if (!instance.Name.StartsWith("->")) return;
        EditorGUI.DrawRect(instance.drawingRect, drawDarkerBackground ? CustomHierarchySettings.folderBackgroundColor : CustomHierarchySettings.folderBackgroundLightColor);
    }
    
    /// <summary>
    /// Draw all the Rect for the Hover Objects and Selected Objects
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawSelectionRect(InstanceInfos instance, Rect selectionRect) {
        if (instance.fullSizeRect.Contains(Event.current.mousePosition)) EditorGUI.DrawRect(selectionRect, CustomHierarchySettings.mouseHoverColor);

        if (Selection.gameObjects.Contains(instance.Gam) && EditorWindow.focusedWindow != null) {
            EditorGUI.DrawRect(selectionRect, EditorWindow.focusedWindow.ToString() == " (UnityEditor.SceneHierarchyWindow)" ? CustomHierarchySettings.objectSelectedColor : CustomHierarchySettings.objectSelectedUnfocusedColor);
        }
    }

    /// <summary>
    /// Draw a darker background 
    /// </summary>
    /// <param name="instance"></param>
    private static void DrawAlternateBackground(InstanceInfos instance) {
        if (!drawDarkerBackground || instance.IsSeparator || instance.Name == " " || instance.Name.StartsWith("->")) return;
        EditorGUI.DrawRect(instance.drawingRect, CustomHierarchySettings.alternateBackgroundColor);
    }
    
    /// <summary>
    /// Draw the name of each Instance
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawInstanceName(InstanceInfos instance, Rect selectionRect) {
        switch (instance.IsSeparator) {
            case true:
                GUI.Label(instance.fullSizeRect, instance.Name.ToUpper(), CustomHierarchySettings.separatorInstanceStyle);
                break;
            case false:
                if (instance.Name.StartsWith("->")) {
                    GUI.contentColor = instance.IsPrefab ? CustomHierarchySettings.prefabColor : Color.white;
                    GUI.Label(selectionRect.GetRectWithoutIcon(), instance.Name[3..]);
                    GUI.contentColor = Color.white;
                }
                else {
                    GUI.contentColor = instance.IsPrefab ? CustomHierarchySettings.prefabColor : Color.white;
                    GUI.Label(selectionRect.GetRectWithoutIcon(), instance.Name);
                    GUI.contentColor = Color.white;
                }
                break;
        }
    }

    /// <summary>
    /// Draw a strike over the name of the object if disable
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawStrikeForName(InstanceInfos instance, Rect selectionRect) {
        if (instance.Gam.activeSelf) return;
        Vector2 width = GUI.skin.label.CalcSize(new GUIContent(instance.Gam.name.StartsWith("-> ")? instance.Gam.name[3..] : instance.Gam.name));
        EditorGUI.DrawRect(new Rect(selectionRect.x, selectionRect.y + (width.y / 2), width.x, 1), Color.white);
    }
    #endregion Background

    #region Foreground
    /// <summary>
    /// Draw everything in the foreground
    /// </summary>
    /// <param name="instance"></param>
    private static void DrawForeground(InstanceInfos instance, Rect selectionRect) {
        if (instance.IsSeparator || instance.Name == " " || instance.Gam == null) return;
        DrawObjectIcon(instance, selectionRect);
        
        if (instance.Name.StartsWith("->")) return;
        DrawTagName(instance, selectionRect);
        DrawComponentIcon(instance, selectionRect);
    }
    
    /// <summary>
    /// Draw the icon for every instance
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawObjectIcon(InstanceInfos instance, Rect selectionRect) {
        if (instance.Name.StartsWith("->")) {
            GUI.DrawTexture(selectionRect.GetObjectIconRect(), AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/YOP_CustomTools/Hierarchy Custom/Texture/Folder.png"));
        }
        else {
            GUIContent content = EditorGUIUtility.ObjectContent(instance.Gam, null);
            if (content.image == null || content.image.name == "d_GameObject Icon") GUI.color = new Color(1, 1, 1, .15f);
            GUI.DrawTexture(selectionRect.GetObjectIconRect(), content.image);
            GUI.color = Color.white;
        }
    }

    /// <summary>
    /// Draw the name of the tag if it's anything else than Untagged
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawTagName(InstanceInfos instance, Rect selectionRect) {
        GUI.contentColor = CustomHierarchySettings.tagNameColor;
        GUI.Label(selectionRect.GetTagRect(), new GUIContent((instance.Gam.CompareTag("Untagged")? "-" : instance.Gam.tag) + (instance.Gam.layer != 0? " *" : ""), $"Tag : {instance.Gam.tag} \nLayer : {LayerMask.LayerToName(instance.Gam.layer)}"), CustomHierarchySettings.tagNameStyle);
        GUI.contentColor = Color.white;
    }

    /// <summary>
    /// Draw the icons of every component of the instance
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawComponentIcon(InstanceInfos instance, Rect selectionRect) {
        List<Component> componentsList = new List<Component>(instance.Gam.GetComponents(typeof(Component))).OrderBy(gam => gam.name).ToList();
        
        int id = 0;
        foreach (Component comp in componentsList) {
            if (comp == null) {
                GUI.Label(selectionRect.GetComponentIconRect(id), new GUIContent(EditorGUIUtility.IconContent("Error@2X").image, "Script is empty. Please remove it!"));
                id++;
                continue;
            }

            Type compType = comp.GetType();
            string componentName = compType.ToString();
            if (compType == typeof(Transform)) continue;
            if (compType.IsSubclassOf(typeof(MonoBehaviour)) && componentName.StartsWith("UnityEngine.")) continue;
            
            string tooltipName = componentName.StartsWith("UnityEngine.") ? componentName.Split(new[] {"UnityEngine."}, StringSplitOptions.None)[1] : componentName;
            GUI.Label(selectionRect.GetComponentIconRect(id), new GUIContent(EditorGUIUtility.ObjectContent(comp, compType).image, tooltipName));
            id++;
        }
    }
    #endregion Foreground
    
    #region treeDrawer
    /// <summary>
    /// Draw tree to show which object is the child of which object
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawTreeObject(InstanceInfos instance, Rect selectionRect) {
        if (instance.ChildLevel == 0 || instance.Gam == null) return;
        
        DrawHorizontalTree(instance, selectionRect);
        DrawVerticalTree(instance, selectionRect);
    }

    /// <summary>
    /// Draw the Horizontal line for the selection
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawHorizontalTree(InstanceInfos instance, Rect selectionRect) {
        EditorGUI.DrawRect(selectionRect.GetHorizontalRect(), instance.IsPrefab? CustomHierarchySettings.treeBranchPrefabColor : CustomHierarchySettings.treeBranchColor);
        
        if (instance.Trans.childCount > 0) {
            if(instance.Gam.IsExpanded()) return;
            
            bool isLastChildOfParents = true;
            for (int i = 0; i < instance.ChildLevel; i++) {
                if (!instance.Trans.GetParentTransformBasedOnChildLevel(i).IsObjectLastChildOfHisParent()) isLastChildOfParents = false;
            }

            if (!isLastChildOfParents) return;
            for (int i = 1; i < instance.ChildLevel; i++) {
                EditorGUI.DrawRect(selectionRect.GetHorizontalLongRect(i), instance.IsPrefab? CustomHierarchySettings.treeBranchPrefabColor : CustomHierarchySettings.treeBranchColor);
            }
            
            return;
        }
        
        for (int i = 1; i < instance.ChildLevel; i++) {
            if (!instance.Trans.GetParentTransformBasedOnChildLevel(i).IsObjectLastChildOfHisParent()) continue;
            if (!instance.Trans.IsObjectLastChildOfHisParent()) continue;
            EditorGUI.DrawRect(selectionRect.GetHorizontalLongRect(i), instance.IsPrefab? CustomHierarchySettings.treeBranchPrefabColor : CustomHierarchySettings.treeBranchColor);
        }
    }
    
    /// <summary>
    /// Draw the Vertical line for the selection
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="selectionRect"></param>
    private static void DrawVerticalTree(InstanceInfos instance, Rect selectionRect) {
        EditorGUI.DrawRect(selectionRect.GetVerticalRect(instance.Trans.IsObjectFirstChildOfHisParent()), instance.IsPrefab? CustomHierarchySettings.treeBranchPrefabColor : CustomHierarchySettings.treeBranchColor);
        
        for (int i = 1; i < instance.ChildLevel; i++) {
            //if (instance.Trans.GetParentTransformBasedOnChildLevel(i).IsObjectLastChildOfHisParent()) continue;
            EditorGUI.DrawRect(selectionRect.GetVerticalLongRect(i), instance.IsPrefab? CustomHierarchySettings.treeBranchPrefabColor : CustomHierarchySettings.treeBranchColor);
        }
    }
    
    #endregion treeDrawer
}