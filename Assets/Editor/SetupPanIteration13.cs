using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupPanIteration13
{
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    private static readonly Color PAN_OFF_COLOR = Hex("#4A5568");
    private static readonly Color PAN_ON_COLOR = Hex("#E8A745");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");

    [MenuItem("WordGame/Setup Pan Controls (Iteration 13)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH))
        {
            Debug.LogError("Game.unity not found. Run iterations 1-12 first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var canvasRT = canvasGO.GetComponent<RectTransform>();

        var gridTR = canvasGO.transform.Find("GridContainer");
        if (gridTR == null) { Debug.LogError("GridContainer not found."); return; }
        var gridRT = gridTR.GetComponent<RectTransform>();

        var floatingTR = canvasGO.transform.Find("FloatingScores");
        RectTransform floatingRT = floatingTR != null ? floatingTR.GetComponent<RectTransform>() : null;

        var panGO = FindOrCreateChild(canvasGO.transform, "PanController");
        var pan = GetOrAdd<PanController>(panGO);
        pan.gridContainer = gridRT;
        pan.floatingScoresContainer = floatingRT;
        pan.canvasRect = canvasRT;

        var hudTR = canvasGO.transform.Find("HUD");
        if (hudTR == null) { Debug.LogError("HUD not found."); return; }

        var hud = hudTR.GetComponent<GameHUD>();
        if (hud == null) { Debug.LogError("GameHUD missing."); return; }

        var backBtnTR = hudTR.Find("BackButton");
        if (backBtnTR != null)
        {
            var backRT = backBtnTR.GetComponent<RectTransform>();
            backRT.anchorMin = new Vector2(0, 0.5f);
            backRT.anchorMax = new Vector2(0, 0.5f);
            backRT.pivot = new Vector2(0, 0.5f);
            backRT.sizeDelta = new Vector2(170, 130);
            backRT.anchoredPosition = new Vector2(30, 0);
            var backLabel = backBtnTR.GetComponentInChildren<TextMeshProUGUI>();
            if (backLabel != null) backLabel.text = "←";
        }

        var panBtnGO = FindOrCreateChild(hudTR, "PanButton");
        var panBtnRT = panBtnGO.GetComponent<RectTransform>();
        panBtnRT.anchorMin = new Vector2(0, 0.5f);
        panBtnRT.anchorMax = new Vector2(0, 0.5f);
        panBtnRT.pivot = new Vector2(0, 0.5f);
        panBtnRT.sizeDelta = new Vector2(170, 130);
        panBtnRT.anchoredPosition = new Vector2(220, 0);

        var panImg = GetOrAdd<Image>(panBtnGO);
        panImg.color = PAN_OFF_COLOR;
        var panBtn = GetOrAdd<Button>(panBtnGO);
        panBtn.targetGraphic = panImg;
        GetOrAdd<UIButtonFeedback>(panBtnGO);

        var panLabelGO = FindOrCreateChild(panBtnGO.transform, "Label");
        var panLabelRT = panLabelGO.GetComponent<RectTransform>();
        SetFullStretch(panLabelRT);
        var panLabel = GetOrAdd<TextMeshProUGUI>(panLabelGO);
        panLabel.text = "✥";
        panLabel.fontSize = 80;
        panLabel.fontStyle = FontStyles.Bold;
        panLabel.color = TEXT_LIGHT;
        panLabel.alignment = TextAlignmentOptions.Center;
        panLabel.raycastTarget = false;
        panLabel.enableAutoSizing = false;

        hud.panToggleButton = panBtn;
        hud.panToggleBackground = panImg;
        hud.panToggleLabel = panLabel;
        hud.panController = pan;
        hud.panOffColor = PAN_OFF_COLOR;
        hud.panOnColor = PAN_ON_COLOR;

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR != null)
        {
            var ctrl = controllerTR.GetComponent<GameController>();
            if (ctrl != null) ctrl.panController = pan;
            EditorUtility.SetDirty(ctrl);
        }

        EditorUtility.SetDirty(pan);
        EditorUtility.SetDirty(hud);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#E8A745><b>WordGame Iteration 13:</b> Pan controls setup complete. Move button placed next to Back.</color>");
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
