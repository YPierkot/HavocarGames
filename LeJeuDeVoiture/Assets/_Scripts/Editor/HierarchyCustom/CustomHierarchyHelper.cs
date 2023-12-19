using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class with methods which will help the HierarchyDrawer class
/// </summary>
public static class CustomHierarchyHelper {
    private const int leftTabHierarchySize = 28;
    private const int rightTabHierarchySize = 16;
    private const int childTabSize = 14;
    private const int leftColumnHierarchySize = 32;
    private const int leftPositionTag = 280;
    private const int leftPositionComponent = 370;

    private const int iconObjectSize = 20;
    private const int toolbarButtonSize = 17;

    private const int treeHorizontalSize = 8;

    private static EditorWindow hierarchyWindow = null;
    
    #region GetInstance
    /// <summary>
    /// Get all the gameObjects from the scene
    /// </summary>
    public static List<InstanceInfos> GetObjectElementFromScene() {
        List<InstanceInfos> returnList = new List<InstanceInfos>();
        List<GameObject> sceneRootsGameObects = new List<GameObject>();
        
        for (int i = 0; i < EditorSceneManager.sceneCount; i++) sceneRootsGameObects.AddRange(EditorSceneManager.GetSceneAt(i).GetRootGameObjects());
        foreach (GameObject gam in sceneRootsGameObects) returnList.AddRange(AddInstanceInfos(gam, sceneRootsGameObects.IndexOf(gam), 0));

        return returnList;
    }

    /// <summary>
    /// Add the InstanceInfos to the List that will be return
    /// </summary>
    /// <param name="InstanceList"></param>
    /// <param name="gam"></param>
    /// <param name="sceneRootsGameObects"></param>
    private static IEnumerable<InstanceInfos> AddInstanceInfos(GameObject gam, int groupID, int childLevel) {
        List<InstanceInfos> instanceInfos = new List<InstanceInfos> {gam.CreateInstanceInfos(groupID, childLevel)};
        if (!gam.HasChild()) return instanceInfos;
        
        for (int i = 0; i < gam.transform.childCount; i++) instanceInfos.AddRange(AddInstanceInfos(gam.transform.GetChild(i).gameObject, groupID, childLevel+1));
        return instanceInfos;
    }
    #endregion GetInstance
    
    #region MethodDataHelper
    /// <summary>
    /// Return the InstanceInfos base on the GameObject
    /// </summary>
    /// <param name="gam"></param>
    /// <param name="groupID"></param>
    /// <returns></returns>
    private static InstanceInfos CreateInstanceInfos(this GameObject gam, int groupID, int childLevel) => new (gam.name, gam.GetInstanceID(), gam, groupID, childLevel, gam.scene, gam != null && gam.transform.parent != null ? gam.transform.parent.gameObject : null, new Rect(), new Rect(), PrefabUtility.IsPartOfAnyPrefab(gam), gam.CompareTag("EditorOnly"));

    /// <summary>
    /// Retrieve an instance Info base of an InstanceID
    /// </summary>
    /// <param name="sceneList"></param>
    /// <param name="instanceID"></param>
    /// <returns></returns>
    public static InstanceInfos GetInstanceFromID(this List<InstanceInfos> sceneList, int instanceID) {
        foreach (InstanceInfos instance in sceneList) {
            if (instance.InstanceID == instanceID) return instance;
        }

        return new InstanceInfos();
    }
    
    /// <summary>
    /// Check if the GameObject has a child
    /// </summary>
    /// <param name="gam"></param>
    /// <returns></returns>
    private static bool HasChild(this GameObject gam) => gam.transform.childCount > 0;
    #endregion MethodDataHelper
    
    #region MethodRectHelper
    /// <summary>
    /// Get the full Rect from the instance
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static Rect GetDrawingRect(this Rect selectionRect, InstanceInfos instance) {
        return new Rect() {
            position = new Vector2(selectionRect.x - leftTabHierarchySize - (childTabSize * instance.ChildLevel), selectionRect.y), 
            size = new Vector2(selectionRect.width + leftTabHierarchySize + (childTabSize * instance.ChildLevel) + rightTabHierarchySize, selectionRect.height)
        };
    }
    
    /// <summary>
    /// Get the Rect from full left to full right
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static Rect GetFullsizeRect(this Rect selectionRect, InstanceInfos instance) {
        return new Rect() {
            position = new Vector2(selectionRect.x - leftTabHierarchySize - (childTabSize * instance.ChildLevel) - leftColumnHierarchySize, selectionRect.y), 
            size = new Vector2(selectionRect.width + leftTabHierarchySize + (childTabSize * instance.ChildLevel) + rightTabHierarchySize + leftColumnHierarchySize, selectionRect.height)
        };
    }

    /// <summary>
    /// Get a Rect without the icon of the Instance
    /// </summary>
    /// <param name="instanceRect"></param>
    /// <returns></returns>
    public static Rect GetRectWithoutIcon(this Rect selectionRect) {
        return new Rect() {
            position = new Vector2(selectionRect.x + iconObjectSize, selectionRect.y),
            size = new Vector2(selectionRect.width - iconObjectSize, selectionRect.height),
        };
    }

    /// <summary>
    /// Get a Rect to draw the Object Icon
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <returns></returns>
    public static Rect GetObjectIconRect(this Rect selectionRect) {
        return new Rect() {
            position = new Vector2(selectionRect.x + 1, selectionRect.y),
            size = new Vector2(selectionRect.height, selectionRect.height),
        };
    }

    /// <summary>
    /// Get the Rect to draw the tag name
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Rect GetTagRect(this Rect selectionRect) {
        return new Rect() {
            position = new Vector2(leftPositionTag , selectionRect.y),
            size = new Vector2(leftPositionComponent - leftPositionTag, selectionRect.height),
        };
    }
    
    /// <summary>
    /// Get the Rect for a Component Icon
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Rect GetComponentIconRect(this Rect selectionRect, int id) {
        return new Rect() {
            position = new Vector2(leftPositionComponent + (iconObjectSize * id), selectionRect.y - 1),
            size = new Vector2(iconObjectSize, iconObjectSize),
        };
    }

    /// <summary>
    /// Get a Rect to draw a line to Separate two blocks
    /// </summary>
    /// <param name="fullsizeRect"></param>
    /// <returns></returns>
    public static Rect GetLineRect(this Rect fullsizeRect, int width = 3) {
        return new Rect() {
            position = new Vector2(fullsizeRect.x, fullsizeRect.y),
            size = new Vector2(fullsizeRect.width, width)
        };
    }
    
    /// <summary>
    /// Get a Rect to draw a line to Separate two blocks
    /// </summary>
    /// <param name="fullsizeRect"></param>
    /// <returns></returns>
    public static Rect GetLineRectBottom(this Rect fullsizeRect, int width = 4) {
        return new Rect() {
            position = new Vector2(fullsizeRect.x, fullsizeRect.y + fullsizeRect.height - 1),
            size = new Vector2(fullsizeRect.width, width)
        };
    }
    

    /// <summary>
    /// Get a Rect to draw the visibility button inside the toolbar
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <returns></returns>
    public static Rect GetVisibiltyButtonRect(this Rect selectionRect) {
        return new Rect() {
            position = new Vector2(selectionRect.x + selectionRect.width - 45, selectionRect.y - 1),
            size = new Vector2(toolbarButtonSize * 2, toolbarButtonSize)
        };
    }

    /// <summary>
    /// Get the rect to draw a star after the name of the scene
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <returns></returns>
    public static Rect GetMainSceneRect(this Rect selectionRect, string sceneName) {
        return new Rect() {
            position = new Vector2(selectionRect.x + iconObjectSize + 5 + GUI.skin.label.CalcSize(new GUIContent(sceneName)).x, selectionRect.y),
            size = new Vector2(toolbarButtonSize, toolbarButtonSize)
        };
    }
    #endregion MethodRectHelper
    
    #region MethodTreeHelper
    /// <summary>
    /// Get the Horizontal Line for an object
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <returns></returns>
    public static Rect GetHorizontalRect(this Rect selectionRect) {
        return new Rect() {
            position = new Vector2(selectionRect.x - treeHorizontalSize - childTabSize, selectionRect.y + (selectionRect.height / 2)),
            size = new Vector2(treeHorizontalSize, 1)
        };
    }
    
    /// <summary>
    /// Get the Horizontal Line for an object
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <returns></returns>
    public static Rect GetHorizontalLongRect(this Rect selectionRect, int level) {
        return new Rect() {
            position = new Vector2(selectionRect.x - treeHorizontalSize - (childTabSize * (level+ 1)), selectionRect.y + (selectionRect.height / 2)),
            size = new Vector2(childTabSize, 1)
        };
    }
    
    /// <summary>
    /// Get the Vertical Line for an object
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <returns></returns>
    public static Rect GetVerticalRect(this Rect selectionRect, bool firstChild) {
        return new Rect() {
            position = new Vector2(selectionRect.x - treeHorizontalSize - childTabSize, selectionRect.y - (!firstChild? selectionRect.height / 2 : 0)),
            size = new Vector2(1, (selectionRect.height / 2) + (!firstChild? selectionRect.height / 2 : 0))
        };
    }

    /// <summary>
    /// Get the Vertical Line to group two parents together
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static Rect GetVerticalLongRect(this Rect selectionRect, int level) {
        return new Rect() {
            position = new Vector2(selectionRect.x - treeHorizontalSize - (childTabSize * (level+ 1)), selectionRect.y - selectionRect.height / 2),
            size = new Vector2(1, selectionRect.height)
        };
    }
    #endregion MethodTreeHelper
    
    #region MethodTransformHelper
    /// <summary>
    /// Is the object the first child of his parent
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static bool IsObjectFirstChildOfHisParent(this Transform trans) => trans.parent.GetChild(0) == trans;

    /// <summary>
    /// Is the object the last child of his parent
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static bool IsObjectLastChildOfHisParent(this Transform trans) {
        if (trans.parent == null) return false;
        return trans.parent.GetChild(trans.parent.childCount - 1) == trans;
    }

    /// <summary>
    /// Get the parent of a transform at a certain child level
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="childLevel"></param>
    /// <returns></returns>
    public static Transform GetParentTransformBasedOnChildLevel(this Transform trans, int childLevel) {
        Transform parent = trans;
        for (int i = 0; i < childLevel; i++) {
            parent = parent.parent;
        }
        return parent;
    }

    /// <summary>
    /// Check if the Transform of the instance if the first Transform in his scene
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static bool IsObjectFirstRootObjectOfHisScene(this InstanceInfos instance) => instance.BaseScene.GetRootGameObjects()[0].transform == instance.Trans;
    #endregion MethodTransformHelper
    
    #region FoldoutExpansion
    /// <summary>
    /// Check if the target GameObject is expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static bool IsExpanded(this GameObject go) => GetExpandedGameObjects().Contains(go);

    /// <summary>
    /// Get a list of all GameObjects which are expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    private static List<GameObject> GetExpandedGameObjects() {
        object sceneHierarchy = GetSceneHierarchy();
        MethodInfo methodInfo = sceneHierarchy.GetType().GetMethod("GetExpandedGameObjects");
        object result = methodInfo?.Invoke(sceneHierarchy, Array.Empty<object>());
        return (List<GameObject>)result;
    }
    
    /// <summary>
    /// get the sceneHierarchy object
    /// </summary>
    /// <returns></returns>
    private static object GetSceneHierarchy() {
        if(hierarchyWindow == null) hierarchyWindow = GetHierarchyWindow();
        object sceneHierarchy = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow").GetProperty("sceneHierarchy")?.GetValue(hierarchyWindow);
        return sceneHierarchy;
    }
    
    /// <summary>
    /// Return the Hierarchy Window
    /// </summary>
    /// <returns></returns>
    private static EditorWindow GetHierarchyWindow() {
        EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        return EditorWindow.focusedWindow;
    }
    #endregion FoldoutExpansion
}

/// <summary>
/// Struct for the information about all gameObjects in the Hierarchy
/// </summary>
public struct InstanceInfos {
    //Base data
    private string name;
    public string Name => name; 
    private int instanceID;
    public int InstanceID => instanceID;
    private GameObject gam;
    public GameObject Gam => gam;
    public Transform Trans => gam.transform;
    
    //Group data
    private int groupeID;
    public int GroupeID => groupeID;
    private int childLevel;
    public int ChildLevel => childLevel;
    private Scene baseScene;
    public Scene BaseScene => baseScene;
    private GameObject parentGam;
    public GameObject ParentGam => parentGam;
    public Transform ParentTrans => parentGam.transform;

    //Precise Gam Infos
    private bool isPrefab;
    public bool IsPrefab => isPrefab;
    private bool isSeparator;
    public bool IsSeparator => isSeparator;
    
    //Editable Element
    public Rect drawingRect;
    public Rect fullSizeRect;

    /// <summary>
    /// Method to call to create a new InstanceInfos
    /// </summary>
    /// <param name="name"></param>
    /// <param name="instanceID"></param>
    /// <param name="gam"></param>
    /// <param name="groupeID"></param>
    /// <param name="childLevel"></param>
    /// <param name="parentGam"></param>
    /// <param name="drawingRect"></param>
    /// <param name="isSeparator"></param>
    public InstanceInfos(string name, int instanceID, GameObject gam, int groupeID, int childLevel, Scene baseScene, GameObject parentGam, Rect drawingRect, Rect fullSizeRect, bool isPrefab, bool isSeparator) {
        this.name = name;
        this.instanceID = instanceID;
        this.gam = gam;
        this.groupeID = groupeID;
        this.childLevel = childLevel;
        this.baseScene = baseScene;
        this.parentGam = parentGam;
        this.drawingRect = drawingRect;
        this.fullSizeRect = fullSizeRect;
        this.isPrefab = isPrefab;
        this.isSeparator = isSeparator;
    }
}