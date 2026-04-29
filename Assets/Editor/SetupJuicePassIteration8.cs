using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class SetupJuicePassIteration8
{
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";
    private const string SPRITE_PATH = "Assets/WordGame/Sprites/Hex.png";

    [MenuItem("WordGame/Setup Juice Pass (Iteration 8)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH))
        {
            Debug.LogError("Game.unity not found.");
            return;
        }

        var hexSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SPRITE_PATH);
        if (hexSprite == null)
        {
            Debug.LogError("Hex.png not found at " + SPRITE_PATH + ". Run iteration 2 setup first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var canvasRT = canvasGO.GetComponent<RectTransform>();

        var shaker = GetOrAdd<ScreenShaker>(canvasGO);
        shaker.target = canvasRT;

        var floatingScoresGO = FindOrCreateChild(canvasGO.transform, "FloatingScores");
        var fsRT = floatingScoresGO.GetComponent<RectTransform>();
        var gridTR = canvasGO.transform.Find("GridContainer");
        if (gridTR != null)
        {
            var gridRT = gridTR as RectTransform;
            fsRT.anchorMin = gridRT.anchorMin;
            fsRT.anchorMax = gridRT.anchorMax;
            fsRT.pivot = gridRT.pivot;
            fsRT.sizeDelta = gridRT.sizeDelta;
            fsRT.anchoredPosition = gridRT.anchoredPosition;
            int gridIdx = gridTR.GetSiblingIndex();
            floatingScoresGO.transform.SetSiblingIndex(gridIdx + 1);
        }

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null) { Debug.LogError("GameController not found."); return; }

        var edge = GetOrAdd<EdgeHighlighter>(controllerTR.gameObject);
        var hexGrid = controllerTR.GetComponent<HexGrid>();
        edge.grid = hexGrid;
        edge.hexSprite = hexSprite;

        var controller = controllerTR.GetComponent<GameController>();
        controller.edgeHighlighter = edge;

        var builder = controllerTR.GetComponent<WordBuilder>();
        builder.floatingScoresParent = fsRT;

        EditorUtility.SetDirty(shaker);
        EditorUtility.SetDirty(edge);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(builder);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#4FCC81><b>WordGame Iteration 8:</b> Juice Pass setup complete.</color>");
    }

    private static GameObject FindOrCreateChild(Transform parent, string name)
    {
        var existing = parent.Find(name);
        bool parentIsUI = parent.GetComponent<RectTransform>() != null;

        if (existing != null)
        {
            if (parentIsUI && existing.GetComponent<RectTransform>() == null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }
            else
            {
                return existing.gameObject;
            }
        }

        GameObject go = parentIsUI
            ? new GameObject(name, typeof(RectTransform))
            : new GameObject(name);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) c = go.AddComponent<T>();
        return c;
    }
}
