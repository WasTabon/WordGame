using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupEscapeModeIteration7
{
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    private static readonly Color POPUP_BG_COLOR = Hex("#151E2B");
    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color SECONDARY_COLOR = Hex("#4A5568");
    private static readonly Color TEAL_COLOR = Hex("#4ECDC4");
    private static readonly Color SUCCESS_COLOR = Hex("#4FCC81");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");
    private static readonly Color TEXT_DARK = Hex("#1A2332");
    private static readonly Color OVERLAY_COLOR = new Color(0f, 0f, 0f, 0.65f);
    private static readonly Color HUD_BG_COLOR = Hex("#151E2B");

    [MenuItem("WordGame/Setup Escape Mode (Iteration 7)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH))
        {
            Debug.LogError("Game.unity not found. Run iterations 1-6 setup first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null) { Debug.LogError("GameController not found."); return; }
        var controllerGO = controllerTR.gameObject;

        var controller = controllerGO.GetComponent<GameController>();
        if (controller == null) { Debug.LogError("GameController component missing."); return; }

        var builder = controllerGO.GetComponent<WordBuilder>();
        if (builder == null) { Debug.LogError("WordBuilder component missing."); return; }

        var timer = GetOrAdd<EscapeTimer>(controllerGO);

        var timerHUD = BuildTimerHUD(canvasGO.transform, timer);

        var popupsRoot = FindOrCreateChild(canvasGO.transform, "Popups");
        SetFullStretch(popupsRoot.GetComponent<RectTransform>());

        var winPopup = BuildWinPopup(popupsRoot.transform);
        winPopup.gameObject.SetActive(false);

        controller.escapeTimer = timer;
        controller.timerHUDRoot = timerHUD.gameObject;
        builder.escapeTimer = timer;
        builder.winPopup = winPopup;

        timerHUD.gameObject.SetActive(false);

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(builder);
        EditorUtility.SetDirty(timer);
        EditorUtility.SetDirty(timerHUD);
        EditorUtility.SetDirty(winPopup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#4ECDC4><b>WordGame Iteration 7:</b> Escape mode setup complete.</color>");
    }

    private static TimerHUD BuildTimerHUD(Transform canvasParent, EscapeTimer timer)
    {
        var go = FindOrCreateChild(canvasParent, "TimerHUD");
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(380, 110);
        rt.anchoredPosition = new Vector2(0, -210);

        var bgImg = GetOrAdd<Image>(go);
        bgImg.color = HUD_BG_COLOR;
        bgImg.raycastTarget = false;

        var iconGO = FindOrCreateChild(go.transform, "Icon");
        var iconRT = iconGO.GetComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0, 0.5f);
        iconRT.anchorMax = new Vector2(0, 0.5f);
        iconRT.pivot = new Vector2(0, 0.5f);
        iconRT.sizeDelta = new Vector2(80, 80);
        iconRT.anchoredPosition = new Vector2(20, 0);
        var iconTMP = GetOrAdd<TextMeshProUGUI>(iconGO);
        iconTMP.text = "⏱";
        iconTMP.fontSize = 60;
        iconTMP.color = TEXT_LIGHT;
        iconTMP.alignment = TextAlignmentOptions.Center;
        iconTMP.raycastTarget = false;
        iconTMP.enableAutoSizing = false;

        var timeGO = FindOrCreateChild(go.transform, "TimeText");
        var timeRT = timeGO.GetComponent<RectTransform>();
        timeRT.anchorMin = new Vector2(0, 0.5f);
        timeRT.anchorMax = new Vector2(1, 0.5f);
        timeRT.pivot = new Vector2(0.5f, 0.5f);
        timeRT.sizeDelta = new Vector2(-100, 90);
        timeRT.anchoredPosition = new Vector2(50, 0);
        var timeTMP = GetOrAdd<TextMeshProUGUI>(timeGO);
        timeTMP.text = "0:00";
        timeTMP.fontSize = 70;
        timeTMP.fontStyle = FontStyles.Bold;
        timeTMP.color = PRIMARY_COLOR;
        timeTMP.alignment = TextAlignmentOptions.MidlineLeft;
        timeTMP.raycastTarget = false;
        timeTMP.enableAutoSizing = false;

        var hud = GetOrAdd<TimerHUD>(go);
        hud.timeText = timeTMP;
        hud.background = bgImg;
        hud.timer = timer;
        return hud;
    }

    private static WinPopup BuildWinPopup(Transform parent)
    {
        var root = FindOrCreateChild(parent, "WinPopup");
        var rootRT = root.GetComponent<RectTransform>();
        SetFullStretch(rootRT);
        GetOrAdd<CanvasGroup>(root);

        var blocker = FindOrCreateChild(root.transform, "Blocker");
        var blockerRT = blocker.GetComponent<RectTransform>();
        SetFullStretch(blockerRT);
        var blockerImg = GetOrAdd<Image>(blocker);
        blockerImg.color = OVERLAY_COLOR;
        blockerImg.raycastTarget = true;

        var panel = FindOrCreateChild(root.transform, "Panel");
        var panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(900, 1150);
        panelRT.anchoredPosition = Vector2.zero;
        var panelImg = GetOrAdd<Image>(panel);
        panelImg.color = POPUP_BG_COLOR;
        panelImg.raycastTarget = true;

        var titleGO = FindOrCreateChild(panel.transform, "Title");
        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 1f);
        titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.sizeDelta = new Vector2(800, 130);
        titleRT.anchoredPosition = new Vector2(0, -70);
        var titleTMP = GetOrAdd<TextMeshProUGUI>(titleGO);
        titleTMP.text = "ESCAPED!";
        titleTMP.fontSize = 95;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = SUCCESS_COLOR;
        titleTMP.raycastTarget = false;
        titleTMP.enableAutoSizing = false;

        var accent = FindOrCreateChild(panel.transform, "TitleAccent");
        var accentRT = accent.GetComponent<RectTransform>();
        accentRT.anchorMin = new Vector2(0.5f, 1f);
        accentRT.anchorMax = new Vector2(0.5f, 1f);
        accentRT.pivot = new Vector2(0.5f, 1f);
        accentRT.sizeDelta = new Vector2(160, 8);
        accentRT.anchoredPosition = new Vector2(0, -210);
        var accentImg = GetOrAdd<Image>(accent);
        accentImg.color = SUCCESS_COLOR;
        accentImg.raycastTarget = false;

        var scoreLabelGO = FindOrCreateChild(panel.transform, "ScoreLabel");
        var sLabelRT = scoreLabelGO.GetComponent<RectTransform>();
        sLabelRT.anchorMin = new Vector2(0.5f, 0.5f);
        sLabelRT.anchorMax = new Vector2(0.5f, 0.5f);
        sLabelRT.pivot = new Vector2(0.5f, 0.5f);
        sLabelRT.sizeDelta = new Vector2(700, 80);
        sLabelRT.anchoredPosition = new Vector2(0, 220);
        var sLabelTMP = GetOrAdd<TextMeshProUGUI>(scoreLabelGO);
        sLabelTMP.text = "TOTAL SCORE";
        sLabelTMP.fontSize = 45;
        sLabelTMP.color = TEXT_LIGHT;
        sLabelTMP.alignment = TextAlignmentOptions.Center;
        sLabelTMP.raycastTarget = false;
        sLabelTMP.enableAutoSizing = false;

        var scoreGO = FindOrCreateChild(panel.transform, "Score");
        var scoreRT = scoreGO.GetComponent<RectTransform>();
        scoreRT.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRT.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRT.pivot = new Vector2(0.5f, 0.5f);
        scoreRT.sizeDelta = new Vector2(700, 200);
        scoreRT.anchoredPosition = new Vector2(0, 90);
        var scoreTMP = GetOrAdd<TextMeshProUGUI>(scoreGO);
        scoreTMP.text = "0";
        scoreTMP.fontSize = 180;
        scoreTMP.fontStyle = FontStyles.Bold;
        scoreTMP.color = SUCCESS_COLOR;
        scoreTMP.alignment = TextAlignmentOptions.Center;
        scoreTMP.raycastTarget = false;
        scoreTMP.enableAutoSizing = false;

        var bonusGO = FindOrCreateChild(panel.transform, "TimeBonus");
        var bonusRT = bonusGO.GetComponent<RectTransform>();
        bonusRT.anchorMin = new Vector2(0.5f, 0.5f);
        bonusRT.anchorMax = new Vector2(0.5f, 0.5f);
        bonusRT.pivot = new Vector2(0.5f, 0.5f);
        bonusRT.sizeDelta = new Vector2(700, 60);
        bonusRT.anchoredPosition = new Vector2(0, -30);
        var bonusTMP = GetOrAdd<TextMeshProUGUI>(bonusGO);
        bonusTMP.text = "+ 0 time bonus";
        bonusTMP.fontSize = 40;
        bonusTMP.color = PRIMARY_COLOR;
        bonusTMP.alignment = TextAlignmentOptions.Center;
        bonusTMP.raycastTarget = false;
        bonusTMP.enableAutoSizing = false;

        var newRecordGO = FindOrCreateChild(panel.transform, "NewRecordBadge");
        var nrRT = newRecordGO.GetComponent<RectTransform>();
        nrRT.anchorMin = new Vector2(0.5f, 0.5f);
        nrRT.anchorMax = new Vector2(0.5f, 0.5f);
        nrRT.pivot = new Vector2(0.5f, 0.5f);
        nrRT.sizeDelta = new Vector2(500, 60);
        nrRT.anchoredPosition = new Vector2(0, -100);
        var nrTMP = GetOrAdd<TextMeshProUGUI>(newRecordGO);
        nrTMP.text = "★ NEW BEST! ★";
        nrTMP.fontSize = 45;
        nrTMP.fontStyle = FontStyles.Bold;
        nrTMP.color = PRIMARY_COLOR;
        nrTMP.alignment = TextAlignmentOptions.Center;
        nrTMP.raycastTarget = false;
        nrTMP.enableAutoSizing = false;
        newRecordGO.SetActive(false);

        var hsGO = FindOrCreateChild(panel.transform, "HighScore");
        var hsRT = hsGO.GetComponent<RectTransform>();
        hsRT.anchorMin = new Vector2(0.5f, 0.5f);
        hsRT.anchorMax = new Vector2(0.5f, 0.5f);
        hsRT.pivot = new Vector2(0.5f, 0.5f);
        hsRT.sizeDelta = new Vector2(700, 60);
        hsRT.anchoredPosition = new Vector2(0, -170);
        var hsTMP = GetOrAdd<TextMeshProUGUI>(hsGO);
        hsTMP.text = "Best: 0";
        hsTMP.fontSize = 45;
        hsTMP.color = TEXT_LIGHT;
        hsTMP.alignment = TextAlignmentOptions.Center;
        hsTMP.raycastTarget = false;
        hsTMP.enableAutoSizing = false;

        var restartBtn = CreateButton(panel.transform, "RestartButton", "PLAY AGAIN", SUCCESS_COLOR, TEXT_DARK, new Vector2(700, 140), 60);
        var restartRT = restartBtn.GetComponent<RectTransform>();
        restartRT.anchorMin = new Vector2(0.5f, 0f);
        restartRT.anchorMax = new Vector2(0.5f, 0f);
        restartRT.pivot = new Vector2(0.5f, 0f);
        restartRT.sizeDelta = new Vector2(700, 140);
        restartRT.anchoredPosition = new Vector2(0, 230);

        var menuBtn = CreateButton(panel.transform, "MainMenuButton", "MAIN MENU", SECONDARY_COLOR, TEXT_LIGHT, new Vector2(700, 130), 50);
        var menuRT = menuBtn.GetComponent<RectTransform>();
        menuRT.anchorMin = new Vector2(0.5f, 0f);
        menuRT.anchorMax = new Vector2(0.5f, 0f);
        menuRT.pivot = new Vector2(0.5f, 0f);
        menuRT.sizeDelta = new Vector2(700, 130);
        menuRT.anchoredPosition = new Vector2(0, 70);

        var popup = GetOrAdd<WinPopup>(root);
        popup.panel = panelRT;
        popup.titleText = titleTMP;
        popup.scoreText = scoreTMP;
        popup.timeBonusText = bonusTMP;
        popup.highScoreText = hsTMP;
        popup.newRecordBadge = nrTMP;
        popup.restartButton = restartBtn;
        popup.mainMenuButton = menuBtn;
        return popup;
    }

    private static Button CreateButton(Transform parent, string name, string label, Color bgColor, Color textColor, Vector2 size, float labelFontSize)
    {
        var go = FindOrCreateChild(parent, name);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        var img = GetOrAdd<Image>(go);
        img.color = bgColor;
        var btn = GetOrAdd<Button>(go);
        btn.targetGraphic = img;
        GetOrAdd<UIButtonFeedback>(go);

        var labelGO = FindOrCreateChild(go.transform, "Label");
        var labelRT = labelGO.GetComponent<RectTransform>();
        SetFullStretch(labelRT);
        var labelTMP = GetOrAdd<TextMeshProUGUI>(labelGO);
        labelTMP.text = label;
        labelTMP.fontSize = labelFontSize;
        labelTMP.fontStyle = FontStyles.Bold;
        labelTMP.color = textColor;
        labelTMP.alignment = TextAlignmentOptions.Center;
        labelTMP.raycastTarget = false;
        labelTMP.enableAutoSizing = false;
        return btn;
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
