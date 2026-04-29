using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupEndgameHighScoreIteration6
{
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";
    private const string MAINMENU_SCENE_PATH = "Assets/WordGame/Scenes/MainMenu.unity";

    private static readonly Color POPUP_BG_COLOR = Hex("#151E2B");
    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color SECONDARY_COLOR = Hex("#4A5568");
    private static readonly Color TEAL_COLOR = Hex("#4ECDC4");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");
    private static readonly Color TEXT_DARK = Hex("#1A2332");
    private static readonly Color OVERLAY_COLOR = new Color(0f, 0f, 0f, 0.65f);
    private static readonly Color SUCCESS_COLOR = Hex("#4FCC81");

    [MenuItem("WordGame/Setup Endgame + HighScore (Iteration 6)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH) || !File.Exists(MAINMENU_SCENE_PATH))
        {
            Debug.LogError("Required scenes not found. Run iterations 1-5 setup first.");
            return;
        }

        SetupGameScene();
        SetupMainMenuScene();

        Debug.Log("<color=#E8A745><b>WordGame Iteration 6:</b> Endgame + HighScore setup complete.</color>");
    }

    private static void SetupGameScene()
    {
        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found in Game.unity."); return; }

        var popupsRoot = FindOrCreateChild(canvasGO.transform, "Popups");
        var popupsRT = popupsRoot.GetComponent<RectTransform>();
        SetFullStretch(popupsRT);

        var popup = BuildGameOverPopup(popupsRoot.transform);
        popup.gameObject.SetActive(false);

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null) { Debug.LogError("GameController not found."); return; }
        var builder = controllerTR.GetComponent<WordBuilder>();
        if (builder == null) { Debug.LogError("WordBuilder not found."); return; }
        builder.gameOverPopup = popup;

        EditorUtility.SetDirty(builder);
        EditorUtility.SetDirty(popup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void SetupMainMenuScene()
    {
        var scene = EditorSceneManager.OpenScene(MAINMENU_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found in MainMenu.unity."); return; }

        var popupsTR = canvasGO.transform.Find("Popups");
        if (popupsTR == null) { Debug.LogError("Popups root not found."); return; }

        var modeTR = popupsTR.Find("ModeSelectPopup");
        if (modeTR == null) { Debug.LogError("ModeSelectPopup not found."); return; }

        var modePopup = modeTR.GetComponent<ModeSelectPopup>();
        if (modePopup == null) { Debug.LogError("ModeSelectPopup component missing."); return; }

        var panel = modeTR.Find("Panel");
        if (panel == null) { Debug.LogError("ModeSelectPopup panel not found."); return; }

        var escapeBtnTR = panel.Find("EscapeButton");
        var exploreBtnTR = panel.Find("ExploreButton");

        TextMeshProUGUI escapeBest = null, exploreBest = null;
        if (escapeBtnTR != null) escapeBest = AddOrUpdateBestLabel(escapeBtnTR, TEXT_DARK);
        if (exploreBtnTR != null) exploreBest = AddOrUpdateBestLabel(exploreBtnTR, TEXT_DARK);

        modePopup.escapeBestText = escapeBest;
        modePopup.exploreBestText = exploreBest;

        EditorUtility.SetDirty(modePopup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static TextMeshProUGUI AddOrUpdateBestLabel(Transform buttonTR, Color color)
    {
        var go = FindOrCreateChild(buttonTR, "BestText");
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);
        rt.sizeDelta = new Vector2(280, 50);
        rt.anchoredPosition = new Vector2(-20, 15);

        var tmp = GetOrAdd<TextMeshProUGUI>(go);
        tmp.text = "Best: 0";
        tmp.fontSize = 30;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.MidlineRight;
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = false;
        return tmp;
    }

    private static GameOverPopup BuildGameOverPopup(Transform parent)
    {
        var root = FindOrCreateChild(parent, "GameOverPopup");
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
        panelRT.sizeDelta = new Vector2(900, 1100);
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
        titleTMP.text = "NO MORE WORDS";
        titleTMP.fontSize = 75;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = PRIMARY_COLOR;
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
        accentImg.color = PRIMARY_COLOR;
        accentImg.raycastTarget = false;

        var scoreLabelGO = FindOrCreateChild(panel.transform, "ScoreLabel");
        var sLabelRT = scoreLabelGO.GetComponent<RectTransform>();
        sLabelRT.anchorMin = new Vector2(0.5f, 0.5f);
        sLabelRT.anchorMax = new Vector2(0.5f, 0.5f);
        sLabelRT.pivot = new Vector2(0.5f, 0.5f);
        sLabelRT.sizeDelta = new Vector2(700, 80);
        sLabelRT.anchoredPosition = new Vector2(0, 200);
        var sLabelTMP = GetOrAdd<TextMeshProUGUI>(scoreLabelGO);
        sLabelTMP.text = "FINAL SCORE";
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
        scoreRT.anchoredPosition = new Vector2(0, 80);
        var scoreTMP = GetOrAdd<TextMeshProUGUI>(scoreGO);
        scoreTMP.text = "0";
        scoreTMP.fontSize = 180;
        scoreTMP.fontStyle = FontStyles.Bold;
        scoreTMP.color = PRIMARY_COLOR;
        scoreTMP.alignment = TextAlignmentOptions.Center;
        scoreTMP.raycastTarget = false;
        scoreTMP.enableAutoSizing = false;

        var newRecordGO = FindOrCreateChild(panel.transform, "NewRecordBadge");
        var nrRT = newRecordGO.GetComponent<RectTransform>();
        nrRT.anchorMin = new Vector2(0.5f, 0.5f);
        nrRT.anchorMax = new Vector2(0.5f, 0.5f);
        nrRT.pivot = new Vector2(0.5f, 0.5f);
        nrRT.sizeDelta = new Vector2(500, 70);
        nrRT.anchoredPosition = new Vector2(0, -30);
        var nrTMP = GetOrAdd<TextMeshProUGUI>(newRecordGO);
        nrTMP.text = "★ NEW BEST! ★";
        nrTMP.fontSize = 50;
        nrTMP.fontStyle = FontStyles.Bold;
        nrTMP.color = SUCCESS_COLOR;
        nrTMP.alignment = TextAlignmentOptions.Center;
        nrTMP.raycastTarget = false;
        nrTMP.enableAutoSizing = false;
        newRecordGO.SetActive(false);

        var hsGO = FindOrCreateChild(panel.transform, "HighScore");
        var hsRT = hsGO.GetComponent<RectTransform>();
        hsRT.anchorMin = new Vector2(0.5f, 0.5f);
        hsRT.anchorMax = new Vector2(0.5f, 0.5f);
        hsRT.pivot = new Vector2(0.5f, 0.5f);
        hsRT.sizeDelta = new Vector2(700, 70);
        hsRT.anchoredPosition = new Vector2(0, -120);
        var hsTMP = GetOrAdd<TextMeshProUGUI>(hsGO);
        hsTMP.text = "Best: 0";
        hsTMP.fontSize = 50;
        hsTMP.color = TEXT_LIGHT;
        hsTMP.alignment = TextAlignmentOptions.Center;
        hsTMP.raycastTarget = false;
        hsTMP.enableAutoSizing = false;

        var restartBtn = CreateButton(panel.transform, "RestartButton", "PLAY AGAIN", PRIMARY_COLOR, TEXT_DARK, new Vector2(700, 140), 60);
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

        var popup = GetOrAdd<GameOverPopup>(root);
        popup.panel = panelRT;
        popup.titleText = titleTMP;
        popup.scoreText = scoreTMP;
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
