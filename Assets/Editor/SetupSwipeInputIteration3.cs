using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupSwipeInputIteration3
{
    private const string SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");

    [MenuItem("WordGame/Setup Swipe Input (Iteration 3)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(SCENE_PATH))
        {
            Debug.LogError("Game.unity not found at " + SCENE_PATH + ". Run 'WordGame > Setup Game Scene (Iteration 2)' first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null)
        {
            Debug.LogError("Canvas not found in Game.unity. Run iteration 2 setup first.");
            return;
        }

        var gridTR = canvasGO.transform.Find("GridContainer");
        if (gridTR == null)
        {
            Debug.LogError("GridContainer not found. Run iteration 2 setup first.");
            return;
        }
        var gridRT = gridTR as RectTransform;

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null)
        {
            Debug.LogError("GameController not found. Run iteration 2 setup first.");
            return;
        }
        var controllerGO = controllerTR.gameObject;
        var hexGrid = controllerGO.GetComponent<HexGrid>();
        if (hexGrid == null)
        {
            Debug.LogError("HexGrid component not found on GameController. Run iteration 2 setup first.");
            return;
        }

        var linesGO = FindOrCreateChild(canvasGO.transform, "LinesContainer");
        var linesRT = linesGO.GetComponent<RectTransform>();
        CopyRectTransform(gridRT, linesRT);

        var previewGO = FindOrCreateChild(canvasGO.transform, "WordPreview");
        var previewRT = previewGO.GetComponent<RectTransform>();
        previewRT.anchorMin = new Vector2(0.5f, 0.5f);
        previewRT.anchorMax = new Vector2(0.5f, 0.5f);
        previewRT.pivot = new Vector2(0.5f, 0.5f);
        previewRT.sizeDelta = new Vector2(1000, 180);
        previewRT.anchoredPosition = new Vector2(0, 600);

        var textTMP = CreateText(previewGO.transform, "Text", "", 110, PRIMARY_COLOR, TextAlignmentOptions.Center);
        textTMP.fontStyle = FontStyles.Bold;
        textTMP.characterSpacing = 10f;
        textTMP.enableAutoSizing = true;
        textTMP.fontSizeMin = 60;
        textTMP.fontSizeMax = 110;
        SetFullStretch(textTMP.GetComponent<RectTransform>());

        var previewUI = GetOrAdd<WordPreviewUI>(previewGO);
        previewUI.text = textTMP;

        var bgTR = canvasGO.transform.Find("Background");
        if (bgTR != null) bgTR.SetSiblingIndex(0);
        linesGO.transform.SetSiblingIndex(1);
        gridRT.SetSiblingIndex(2);

        var builder = GetOrAdd<WordBuilder>(controllerGO);
        builder.grid = hexGrid;
        builder.linesContainer = linesRT;
        builder.preview = previewUI;

        EditorUtility.SetDirty(builder);
        EditorUtility.SetDirty(previewUI);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#E8A745><b>WordGame Iteration 3:</b> Swipe input setup complete.</color>");
    }

    private static void CopyRectTransform(RectTransform src, RectTransform dst)
    {
        dst.anchorMin = src.anchorMin;
        dst.anchorMax = src.anchorMax;
        dst.pivot = src.pivot;
        dst.sizeDelta = src.sizeDelta;
        dst.anchoredPosition = src.anchoredPosition;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions align)
    {
        var go = FindOrCreateChild(parent, name);
        var tmp = GetOrAdd<TextMeshProUGUI>(go);
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = align;
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = false;
        return tmp;
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

    private static void SetFullStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static Color Hex(string hex)
    {
        Color c;
        ColorUtility.TryParseHtmlString(hex, out c);
        return c;
    }
}
