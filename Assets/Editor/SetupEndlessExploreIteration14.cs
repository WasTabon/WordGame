using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupEndlessExploreIteration14
{
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color SUCCESS_COLOR = Hex("#4FCC81");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");
    private static readonly Color PANEL_COLOR = Hex("#151E2B");

    [MenuItem("WordGame/Setup Endless Explore (Iteration 14)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH))
        {
            Debug.LogError("Game.unity not found. Run iterations 1-13 first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var toast = BuildStageToast(canvasGO.transform);

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null) { Debug.LogError("GameController not found."); return; }

        var controller = controllerTR.GetComponent<GameController>();
        var validator = controllerTR.GetComponent<WordValidator>();
        if (controller == null || validator == null) { Debug.LogError("Required components missing."); return; }

        controller.stageToast = toast;
        controller.validator = validator;

        var hudTR = canvasGO.transform.Find("HUD");
        if (hudTR != null)
        {
            var hud = hudTR.GetComponent<GameHUD>();
            if (hud != null)
            {
                hud.gameController = controller;
                EditorUtility.SetDirty(hud);
            }
        }

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(toast);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#4FCC81><b>WordGame Iteration 14:</b> Endless Explore setup complete. Deadlock in Explore now triggers stage continue.</color>");
    }

    private static StageToast BuildStageToast(Transform parent)
    {
        var rootGO = FindOrCreateChild(parent, "StageToast");
        var rootRT = rootGO.GetComponent<RectTransform>();
        rootRT.anchorMin = new Vector2(0.5f, 0.5f);
        rootRT.anchorMax = new Vector2(0.5f, 0.5f);
        rootRT.pivot = new Vector2(0.5f, 0.5f);
        rootRT.sizeDelta = new Vector2(900, 360);
        rootRT.anchoredPosition = Vector2.zero;
        rootGO.transform.SetAsLastSibling();

        var canvasGroup = GetOrAdd<CanvasGroup>(rootGO);
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        var panelGO = FindOrCreateChild(rootGO.transform, "Panel");
        var panelRT = panelGO.GetComponent<RectTransform>();
        SetFullStretch(panelRT);
        var panelImg = GetOrAdd<Image>(panelGO);
        panelImg.color = PANEL_COLOR;
        panelImg.raycastTarget = false;

        var accentGO = FindOrCreateChild(panelGO.transform, "Accent");
        var accentRT = accentGO.GetComponent<RectTransform>();
        accentRT.anchorMin = new Vector2(0.5f, 1f);
        accentRT.anchorMax = new Vector2(0.5f, 1f);
        accentRT.pivot = new Vector2(0.5f, 1f);
        accentRT.sizeDelta = new Vector2(180, 8);
        accentRT.anchoredPosition = new Vector2(0, -30);
        var accentImg = GetOrAdd<Image>(accentGO);
        accentImg.color = SUCCESS_COLOR;
        accentImg.raycastTarget = false;

        var titleGO = FindOrCreateChild(panelGO.transform, "Title");
        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 0.5f);
        titleRT.anchorMax = new Vector2(0.5f, 0.5f);
        titleRT.pivot = new Vector2(0.5f, 0.5f);
        titleRT.sizeDelta = new Vector2(800, 150);
        titleRT.anchoredPosition = new Vector2(0, 40);
        var titleTMP = GetOrAdd<TextMeshProUGUI>(titleGO);
        titleTMP.text = "STAGE 1";
        titleTMP.fontSize = 110;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.color = SUCCESS_COLOR;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.raycastTarget = false;
        titleTMP.enableAutoSizing = false;

        var subGO = FindOrCreateChild(panelGO.transform, "Subtitle");
        var subRT = subGO.GetComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0.5f, 0.5f);
        subRT.anchorMax = new Vector2(0.5f, 0.5f);
        subRT.pivot = new Vector2(0.5f, 0.5f);
        subRT.sizeDelta = new Vector2(840, 80);
        subRT.anchoredPosition = new Vector2(0, -60);
        var subTMP = GetOrAdd<TextMeshProUGUI>(subGO);
        subTMP.text = "Board cleared! New stage starting...";
        subTMP.fontSize = 40;
        subTMP.color = TEXT_LIGHT;
        subTMP.alignment = TextAlignmentOptions.Center;
        subTMP.raycastTarget = false;
        subTMP.enableAutoSizing = false;

        var toast = GetOrAdd<StageToast>(rootGO);
        toast.root = rootRT;
        toast.titleText = titleTMP;
        toast.subtitleText = subTMP;
        toast.canvasGroup = canvasGroup;

        rootGO.SetActive(false);
        return toast;
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
