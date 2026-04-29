using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupTutorialIteration9
{
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";
    private const string MAINMENU_SCENE_PATH = "Assets/WordGame/Scenes/MainMenu.unity";

    private static readonly Color OVERLAY_COLOR = new Color(0f, 0f, 0f, 0.78f);
    private static readonly Color PANEL_COLOR = Hex("#151E2B");
    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color SECONDARY_COLOR = Hex("#4A5568");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");
    private static readonly Color HINT_COLOR = new Color(1f, 1f, 1f, 0.6f);
    private static readonly Color SUCCESS_COLOR = Hex("#4FCC81");

    [MenuItem("WordGame/Setup Tutorial (Iteration 9)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH) || !File.Exists(MAINMENU_SCENE_PATH))
        {
            Debug.LogError("Required scenes not found.");
            return;
        }

        SetupGameScene();
        SetupSettingsPopup();

        Debug.Log("<color=#E8A745><b>WordGame Iteration 9:</b> Tutorial setup complete.</color>");
    }

    private static void SetupGameScene()
    {
        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var tutGO = FindOrCreateChild(canvasGO.transform, "Tutorial");
        var tutRT = tutGO.GetComponent<RectTransform>();
        SetFullStretch(tutRT);
        tutGO.transform.SetAsLastSibling();

        var canvasGroup = GetOrAdd<CanvasGroup>(tutGO);

        var dimGO = FindOrCreateChild(tutGO.transform, "Dim");
        var dimRT = dimGO.GetComponent<RectTransform>();
        SetFullStretch(dimRT);
        var dimImg = GetOrAdd<Image>(dimGO);
        dimImg.color = OVERLAY_COLOR;
        dimImg.raycastTarget = true;

        var calloutGO = FindOrCreateChild(tutGO.transform, "Callout");
        var calloutRT = calloutGO.GetComponent<RectTransform>();
        calloutRT.anchorMin = new Vector2(0.5f, 0.5f);
        calloutRT.anchorMax = new Vector2(0.5f, 0.5f);
        calloutRT.pivot = new Vector2(0.5f, 0.5f);
        calloutRT.sizeDelta = new Vector2(900, 500);
        calloutRT.anchoredPosition = Vector2.zero;
        var calloutImg = GetOrAdd<Image>(calloutGO);
        calloutImg.color = PANEL_COLOR;
        calloutImg.raycastTarget = false;

        var accentGO = FindOrCreateChild(calloutGO.transform, "Accent");
        var accentRT = accentGO.GetComponent<RectTransform>();
        accentRT.anchorMin = new Vector2(0.5f, 1f);
        accentRT.anchorMax = new Vector2(0.5f, 1f);
        accentRT.pivot = new Vector2(0.5f, 1f);
        accentRT.sizeDelta = new Vector2(160, 8);
        accentRT.anchoredPosition = new Vector2(0, -30);
        var accentImg = GetOrAdd<Image>(accentGO);
        accentImg.color = PRIMARY_COLOR;
        accentImg.raycastTarget = false;

        var textGO = FindOrCreateChild(calloutGO.transform, "Text");
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.pivot = new Vector2(0.5f, 0.5f);
        textRT.offsetMin = new Vector2(50, 100);
        textRT.offsetMax = new Vector2(-50, -80);
        var textTMP = GetOrAdd<TextMeshProUGUI>(textGO);
        textTMP.text = "";
        textTMP.fontSize = 50;
        textTMP.fontStyle = FontStyles.Bold;
        textTMP.color = TEXT_LIGHT;
        textTMP.alignment = TextAlignmentOptions.Center;
        textTMP.raycastTarget = false;
        textTMP.enableAutoSizing = false;
        textTMP.enableWordWrapping = true;

        var hintGO = FindOrCreateChild(calloutGO.transform, "Hint");
        var hintRT = hintGO.GetComponent<RectTransform>();
        hintRT.anchorMin = new Vector2(0.5f, 0f);
        hintRT.anchorMax = new Vector2(0.5f, 0f);
        hintRT.pivot = new Vector2(0.5f, 0f);
        hintRT.sizeDelta = new Vector2(800, 60);
        hintRT.anchoredPosition = new Vector2(0, 30);
        var hintTMP = GetOrAdd<TextMeshProUGUI>(hintGO);
        hintTMP.text = "Tap to continue";
        hintTMP.fontSize = 36;
        hintTMP.color = HINT_COLOR;
        hintTMP.alignment = TextAlignmentOptions.Center;
        hintTMP.raycastTarget = false;
        hintTMP.enableAutoSizing = false;

        var tutorial = GetOrAdd<Tutorial>(tutGO);
        tutorial.canvasGroup = canvasGroup;
        tutorial.dimImage = dimImg;
        tutorial.calloutPanel = calloutRT;
        tutorial.calloutText = textTMP;
        tutorial.hintText = hintTMP;

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR != null)
        {
            var hexGrid = controllerTR.GetComponent<HexGrid>();
            tutorial.grid = hexGrid;

            var controller = controllerTR.GetComponent<GameController>();
            if (controller != null) controller.tutorial = tutorial;
            EditorUtility.SetDirty(controller);
        }

        var previewTR = canvasGO.transform.Find("WordPreview");
        if (previewTR != null) tutorial.previewRT = previewTR as RectTransform;

        var timerTR = canvasGO.transform.Find("TimerHUD");
        if (timerTR != null) tutorial.timerHudRT = timerTR as RectTransform;

        tutGO.SetActive(false);

        EditorUtility.SetDirty(tutorial);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void SetupSettingsPopup()
    {
        var scene = EditorSceneManager.OpenScene(MAINMENU_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("MainMenu Canvas not found."); return; }

        var popupsTR = canvasGO.transform.Find("Popups");
        if (popupsTR == null) { Debug.LogError("Popups root not found."); return; }

        var settingsTR = popupsTR.Find("SettingsPopup");
        if (settingsTR == null) { Debug.LogError("SettingsPopup not found."); return; }

        var panel = settingsTR.Find("Panel");
        if (panel == null) { Debug.LogError("SettingsPopup panel not found."); return; }

        var settingsPopup = settingsTR.GetComponent<SettingsPopup>();
        if (settingsPopup == null) { Debug.LogError("SettingsPopup component missing."); return; }

        var closeBtnTR = panel.Find("CloseButton");
        if (closeBtnTR != null)
        {
            var closeBtnRT = closeBtnTR as RectTransform;
            closeBtnRT.anchoredPosition = new Vector2(0, -540);
        }

        var resetBtnGO = FindOrCreateChild(panel, "ResetTutorialButton");
        var resetRT = resetBtnGO.GetComponent<RectTransform>();
        resetRT.anchorMin = new Vector2(0.5f, 0.5f);
        resetRT.anchorMax = new Vector2(0.5f, 0.5f);
        resetRT.pivot = new Vector2(0.5f, 0.5f);
        resetRT.sizeDelta = new Vector2(600, 110);
        resetRT.anchoredPosition = new Vector2(0, -300);
        var resetImg = GetOrAdd<Image>(resetBtnGO);
        resetImg.color = SECONDARY_COLOR;
        var resetBtn = GetOrAdd<Button>(resetBtnGO);
        resetBtn.targetGraphic = resetImg;
        GetOrAdd<UIButtonFeedback>(resetBtnGO);

        var resetLabelGO = FindOrCreateChild(resetBtnGO.transform, "Label");
        var resetLabelRT = resetLabelGO.GetComponent<RectTransform>();
        SetFullStretch(resetLabelRT);
        var resetLabelTMP = GetOrAdd<TextMeshProUGUI>(resetLabelGO);
        resetLabelTMP.text = "RESET TUTORIAL";
        resetLabelTMP.fontSize = 45;
        resetLabelTMP.fontStyle = FontStyles.Bold;
        resetLabelTMP.color = TEXT_LIGHT;
        resetLabelTMP.alignment = TextAlignmentOptions.Center;
        resetLabelTMP.raycastTarget = false;
        resetLabelTMP.enableAutoSizing = false;

        var feedbackGO = FindOrCreateChild(panel, "ResetFeedback");
        var feedbackRT = feedbackGO.GetComponent<RectTransform>();
        feedbackRT.anchorMin = new Vector2(0.5f, 0.5f);
        feedbackRT.anchorMax = new Vector2(0.5f, 0.5f);
        feedbackRT.pivot = new Vector2(0.5f, 0.5f);
        feedbackRT.sizeDelta = new Vector2(600, 60);
        feedbackRT.anchoredPosition = new Vector2(0, -395);
        var feedbackTMP = GetOrAdd<TextMeshProUGUI>(feedbackGO);
        feedbackTMP.text = "Tutorial reset!";
        feedbackTMP.fontSize = 38;
        feedbackTMP.color = SUCCESS_COLOR;
        feedbackTMP.alignment = TextAlignmentOptions.Center;
        feedbackTMP.raycastTarget = false;
        feedbackTMP.enableAutoSizing = false;
        feedbackGO.SetActive(false);

        settingsPopup.resetTutorialButton = resetBtn;
        settingsPopup.resetTutorialFeedback = feedbackTMP;

        EditorUtility.SetDirty(settingsPopup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
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
