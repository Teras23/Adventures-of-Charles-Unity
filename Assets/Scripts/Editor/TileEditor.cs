#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;


[System.Serializable]
public class TileEditor : EditorWindow {

    public bool showProjectSettings;
    public string prefabFolder;
    public string parentName;
    public int tileSize;
    public int layers;

    public bool showEditorSettings;
    public static bool enableGrid;
    public static bool editmode;

    public Object tilemap;
    public Vector2 selectedTile = new Vector2(0, 0);
    public static Object parent;
    public static int currentLayer;
    public Sprite[] sprites;
    

    [MenuItem("Window/Tile Editor")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(TileEditor));
    }

    void OnGUI() {
        showProjectSettings = EditorGUILayout.Foldout(showProjectSettings, "Project settings");

        if(showProjectSettings) {
            EditorGUI.indentLevel++;
            layers = EditorGUILayout.IntField("Amount of layers", layers);
            tileSize = EditorGUILayout.IntField("Tile scale", tileSize);
            parentName = EditorGUILayout.TextField("Parent name", parentName);
            prefabFolder = EditorGUILayout.TextField("Prefabs' location", prefabFolder);
            //parent = EditorGUILayout.ObjectField("Select parent of tiles:", parent, typeof(GameObject), true);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();

        showEditorSettings = EditorGUILayout.Foldout(showEditorSettings, "Editor settings");

        if(showEditorSettings) {
            EditorGUI.indentLevel++;
            enableGrid = EditorGUILayout.Toggle("Enable grid", enableGrid);
            editmode = EditorGUILayout.Toggle("Enable editing", editmode);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        currentLayer = EditorGUILayout.IntSlider("Current layer", currentLayer, 0, layers - 1);

        Debug.Log(layers);

        tilemap = EditorGUILayout.ObjectField("Select Tilemap:", tilemap, typeof(Texture), false);

        if(Selection.activeGameObject != null)
            GUILayout.Label(Selection.activeGameObject.name);
        else
            GUILayout.Label("No object selected, select object");

        /*if(GUILayout.Button("Create prefabs")) {
            //check if folder is there if not create required folders
            foreach(Sprite tile in sprites) {
                GameObject tilePrefab = new GameObject();
                tilePrefab.AddComponent<SpriteRenderer>().sprite = tile;
                tilePrefab.AddComponent<BoxCollider2D>();
                tilePrefab.transform.localScale = new Vector3(2, 2, 2);
                PrefabUtility.CreatePrefab("Assets/" + prefabFolder + "/" + tilemap.name + "/" + tile.name + ".prefab", tilePrefab);
                Object.DestroyImmediate(tilePrefab);
            }
        }*/

        if(parent == null && parentName != null) {
            parent = GameObject.Find(parentName);
            if(parent == null) {
                parent = new GameObject(parentName);
            }
        }

        if(parent != null) {
            if(layers > (parent as GameObject).transform.childCount + 1) {
                for(int i = 0; i < layers - (parent as GameObject).transform.childCount; ++i) {
                    GameObject layer = new GameObject("Layer " + (i + layers - 1));
                    layer.transform.parent = (parent as GameObject).transform;
                }
            }
        }

        if(tilemap != null) {
            sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tilemap)).Select(x => x as Sprite).Where(x => x != null).ToArray();	


        }
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
    static void RenderCustomGizmo(Transform objectTransform, GizmoType gizmoType) {
        Vector2 start = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(Vector3.zero);
        Vector2 end = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(new Vector3(SceneView.currentDrawingSceneView.camera.pixelWidth, SceneView.currentDrawingSceneView.camera.pixelHeight, 0));

        if(enableGrid && SceneView.currentDrawingSceneView.in2DMode) {
            for(int i = 0; i < end.x - start.x; ++i) {
                Gizmos.DrawLine(new Vector3((int)start.x + i, start.y, 0), new Vector3((int)start.x + i, end.y, 0));
            }

            for(int i = 0; i < end.y - start.y; ++i) {
                Gizmos.DrawLine(new Vector3(start.x, (int)start.y + i, 0), new Vector3(end.x, (int)start.y + i, 0));
            }
        }
    }

    void OnEnable() {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    void OnDisable() {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSelectionChange() {
        Repaint();
    }

    static void OnSceneGUI(SceneView aView) {
        if(editmode) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        Event evt = Event.current;
        if(evt.type == EventType.mouseDown && evt.button == 0 && editmode && Selection.activeGameObject != null) {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePos.y;
            Vector2 realPos = SceneView.currentDrawingSceneView.camera.ScreenPointToRay(mousePos).origin;
            
            bool isDoubleTile = false;

            RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(realPos.x, realPos.y), Vector2.zero);

            Debug.Log(realPos.x + " " + realPos.y + "" + hits.Length);

            foreach(RaycastHit2D hit in hits) {
                if(hit.transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder == currentLayer) {
                    isDoubleTile = true;
                }
            }
            
            if(!isDoubleTile){
                GameObject newTile = PrefabUtility.InstantiatePrefab(Selection.activeGameObject) as GameObject;
                
                newTile.transform.position = new Vector3(Mathf.Floor(realPos.x), Mathf.Floor(realPos.y), -currentLayer);
                newTile.name = Selection.activeGameObject.name;

                if(parent != null) {
                    if(Selection.activeGameObject.tag == "World Object") {
                        newTile.transform.parent = GameObject.Find("Objects").transform;
                    }
                    else {
                        newTile.transform.parent = GameObject.Find("Layer " + currentLayer).transform;
                        newTile.GetComponent<SpriteRenderer>().sortingOrder = currentLayer;
                    }
                }               
            }
        }
    }
}
#endif