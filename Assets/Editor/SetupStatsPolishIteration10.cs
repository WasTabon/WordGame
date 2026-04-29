using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupStatsPolishIteration10
{
    private const string MAINMENU_SCENE_PATH = "Assets/WordGame/Scenes/MainMenu.unity";
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    private static readonly Color POPUP_BG_COLOR = Hex("#151E2B");
    private static readonly Color OVERLAY_COLOR = new Color(0f, 0f, 0f, 0.65f);
    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color SECONDARY_COLOR = Hex("#4A5568");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");
    private static readonly Color TEXT_DARK = Hex("#1A2332");
    private static readonly Color SUCCESS_COLOR = Hex("#4FCC81");
    private static readonly Color MUTED_COLOR = new Color(1f, 1f, 1f, 0.6f);

    [MenuItem("WordGame/Setup Stats + Polish (Iteration 10)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(MAINMENU_SCENE_PATH) || !File.Exists(GAME_SCENE_PATH))
        {
            Debug.LogError("Required scenes not found.");
            return;
        }

        SetupMainMenu();
        SetupGameScene();

        Debug.Log("<color=#4FCC81><b>WordGame Iteration 10:</b> Stats + Polish setup complete. Game is feature-complete!</color>");
    }

    private static void SetupMainMenu()
    {
        var scene = EditorSceneManager.OpenScene(MAINMENU_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var bottomRow = canvasGO.transform.Find("BottomRow");
        if (bottomRow == null) { Debug.LogError("BottomRow not found."); return; }

        var settingsBtnTR = bottomRow.Find("SettingsButton");
        var howToBtnTR = bottomRow.Find("HowToPlayButton");

        if (settingsBtnTR != null)
        {
            var rt = settingsBtnTR as RectTransform;
            rt.sizeDelta = new Vector2(290, 130);
            rt.anchoredPosition = new Vector2(-330, 0);
        }
        if (howToBtnTR != null)
        {
            var rt = howToBtnTR as RectTransform;
            rt.sizeDelta = new Vector2(290, 130);
            rt.anchoredPosition = new Vector2(0, 0);
        }

        var statsBtnGO = FindOrCreateChild(bottomRow, "StatsButton");
        var statsRT = statsBtnGO.GetComponent<RectTransform>();
        statsRT.anchorMin = new Vector2(0.5f, 0.5f);
        statsRT.anchorMax = new Vector2(0.5f, 0.5f);
        statsRT.pivot = new Vector2(0.5f, 0.5f);
        statsRT.sizeDelta = new Vector2(290, 130);
        statsRT.anchoredPosition = new Vector2(330, 0);

        var statsImg = GetOrAdd<Image>(statsBtnGO);
        statsImg.color = SECONDARY_COLOR;
        var statsBtn = GetOrAdd<Button>(statsBtnGO);
        statsBtn.targetGraphic = statsImg;
        GetOrAdd<UIButtonFeedback>(statsBtnGO);

        var statsLabelGO = FindOrCreateChild(statsBtnGO.transform, "Label");
        var statsLabelRT = statsLabelGO.GetComponent<RectTransform>();
        SetFullStretch(statsLabelRT);
        var statsLabelTMP = GetOrAdd<TextMeshProUGUI>(statsLabelGO);
        statsLabelTMP.text = "STATS";
        statsLabelTMP.fontSize = 50;
        statsLabelTMP.fontStyle = FontStyles.Bold;
        statsLabelTMP.color = TEXT_LIGHT;
        statsLabelTMP.alignment = TextAlignmentOptions.Center;
        statsLabelTMP.raycastTarget = false;
        statsLabelTMP.enableAutoSizing = false;

        var popupsTR = canvasGO.transform.Find("Popups");
        if (popupsTR == null) { Debug.LogError("Popups root not found."); return; }

        var statsPopup = BuildStatsPopup(popupsTR);
        statsPopup.gameObject.SetActive(false);

        var menuTR = canvasGO.GetComponent<MainMenuUI>();
        if (menuTR == null) menuTR = canvasGO.AddComponent<MainMenuUI>();
        menuTR.statsButton = statsBtn;
        menuTR.statsPopup = statsPopup;

        EditorUtility.SetDirty(menuTR);
        EditorUtility.SetDirty(statsPopup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static StatsPopup BuildStatsPopup(Transform parent)
    {
        var root = FindOrCreateChild(parent, "StatsPopup");
        var rootRT = root.GetComponent<RectTransform>();
        SetFullStretch(rootRT);
        GetOrAdd<CanvasGroup>(root);

        var blocker = FindOrCreateChild(root.transform, "Blocker");
        var blockerRT = blocker.GetComponent<RectTransform>();
        SetFullStretch(blockerRT);
        var blockerImg = GetOrAdd<Image>(blocker);
        blockerImg.color = OVERLAY_COLOR;
        blockerImg.raycastTarget = true;
        var blockerBtn = GetOrAdd<Button>(blocker);
        blockerBtn.transition = Selectable.Transition.None;

        var panel = FindOrCreateChild(root.transform, "Panel");
        var panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(940, 1500);
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
        titleRT.anchoredPosition = new Vector2(0, -60);
        var titleTMP = GetOrAdd<TextMeshProUGUI>(titleGO);
        titleTMP.text = "STATS";
        titleTMP.fontSize = 80;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = TEXT_LIGHT;
        titleTMP.raycastTarget = false;
        titleTMP.enableAutoSizing = false;

        var accent = FindOrCreateChild(panel.transform, "TitleAccent");
        var accentRT = accent.GetComponent<RectTransform>();
        accentRT.anchorMin = new Vector2(0.5f, 1f);
        accentRT.anchorMax = new Vector2(0.5f, 1f);
        accentRT.pivot = new Vector2(0.5f, 1f);
        accentRT.sizeDelta = new Vector2(160, 8);
        accentRT.anchoredPosition = new Vector2(0, -195);
        var accentImg = GetOrAdd<Image>(accent);
        accentImg.color = PRIMARY_COLOR;
        accentImg.raycastTarget = false;

        var closeX = FindOrCreateChild(panel.transform, "CloseXButton");
        var closeXRT = closeX.GetComponent<RectTransform>();
        closeXRT.anchorMin = new Vector2(1f, 1f);
        closeXRT.anchorMax = new Vector2(1f, 1f);
        closeXRT.pivot = new Vector2(1f, 1f);
        closeXRT.sizeDelta = new Vector2(90, 90);
        closeXRT.anchoredPosition = new Vector2(-25, -25);
        var closeXImg = GetOrAdd<Image>(closeX);
        closeXImg.color = SECONDARY_COLOR;
        var closeXBtn = GetOrAdd<Button>(closeX);
        closeXBtn.targetGraphic = closeXImg;
        GetOrAdd<UIButtonFeedback>(closeX);

        var closeXLabelGO = FindOrCreateChild(closeX.transform, "Label");
        var closeXLabelRT = closeXLabelGO.GetComponent<RectTransform>();
        SetFullStretch(closeXLabelRT);
        var closeXLabel = GetOrAdd<TextMeshProUGUI>(closeXLabelGO);
        closeXLabel.text = "×";
        closeXLabel.fontSize = 80;
        closeXLabel.fontStyle = FontStyles.Bold;
        closeXLabel.color = TEXT_LIGHT;
        closeXLabel.alignment = TextAlignmentOptions.Center;
        closeXLabel.raycastTarget = false;

        var rowsRoot = FindOrCreateChild(panel.transform, "Rows");
        var rowsRT = rowsRoot.GetComponent<RectTransform>();
        rowsRT.anchorMin = new Vector2(0.5f, 0.5f);
        rowsRT.anchorMax = new Vector2(0.5f, 0.5f);
        rowsRT.pivot = new Vector2(0.5f, 0.5f);
        rowsRT.sizeDelta = new Vector2(820, 1000);
        rowsRT.anchoredPosition = new Vector2(0, 50);

        float yPos = 420;
        var gamesText = AddStatRow(rowsRoot.transform, "GamesPlayed", "Games played", "0", ref yPos);
        var wordsText = AddStatRow(rowsRoot.transform, "WordsTotal", "Words found", "0", ref yPos);
        var longestText = AddStatRow(rowsRoot.transform, "LongestWord", "Longest word", "-", ref yPos);
        var bestWordText = AddStatRow(rowsRoot.transform, "BestWordScore", "Best word score", "0", ref yPos);
        var winrateText = AddStatRow(rowsRoot.transform, "EscapeWinrate", "Escape wins", "-", ref yPos);
        var timeText = AddStatRow(rowsRoot.transform, "TimePlayed", "Time played", "0s", ref yPos);
        yPos -= 40;
        var divider = FindOrCreateChild(rowsRoot.transform, "Divider");
        var dividerRT = divider.GetComponent<RectTransform>();
        dividerRT.anchorMin = new Vector2(0.5f, 0.5f);
        dividerRT.anchorMax = new Vector2(0.5f, 0.5f);
        dividerRT.pivot = new Vector2(0.5f, 0.5f);
        dividerRT.sizeDelta = new Vector2(700, 3);
        dividerRT.anchoredPosition = new Vector2(0, yPos);
        var divImg = GetOrAdd<Image>(divider);
        divImg.color = MUTED_COLOR;
        divImg.raycastTarget = false;
        yPos -= 60;
        var exploreBestText = AddStatRow(rowsRoot.transform, "ExploreBest", "Explore best", "0", ref yPos);
        var escapeBestText = AddStatRow(rowsRoot.transform, "EscapeBest", "Escape best", "0", ref yPos);

        var resetBtnGO = FindOrCreateChild(panel.transform, "ResetButton");
        var resetRT = resetBtnGO.GetComponent<RectTransform>();
        resetRT.anchorMin = new Vector2(0.5f, 0f);
        resetRT.anchorMax = new Vector2(0.5f, 0f);
        resetRT.pivot = new Vector2(0.5f, 0f);
        resetRT.sizeDelta = new Vector2(600, 110);
        resetRT.anchoredPosition = new Vector2(0, 60);
        var resetImg = GetOrAdd<Image>(resetBtnGO);
        resetImg.color = SECONDARY_COLOR;
        var resetBtn = GetOrAdd<Button>(resetBtnGO);
        resetBtn.targetGraphic = resetImg;
        GetOrAdd<UIButtonFeedback>(resetBtnGO);
        var resetLabelGO = FindOrCreateChild(resetBtnGO.transform, "Label");
        var resetLabelRT = resetLabelGO.GetComponent<RectTransform>();
        SetFullStretch(resetLabelRT);
        var resetLabel = GetOrAdd<TextMeshProUGUI>(resetLabelGO);
        resetLabel.text = "RESET STATS";
        resetLabel.fontSize = 45;
        resetLabel.fontStyle = FontStyles.Bold;
        resetLabel.color = TEXT_LIGHT;
        resetLabel.alignment = TextAlignmentOptions.Center;
        resetLabel.raycastTarget = false;
        resetLabel.enableAutoSizing = false;

        var feedbackGO = FindOrCreateChild(panel.transform, "ResetFeedback");
        var fbRT = feedbackGO.GetComponent<RectTransform>();
        fbRT.anchorMin = new Vector2(0.5f, 0f);
        fbRT.anchorMax = new Vector2(0.5f, 0f);
        fbRT.pivot = new Vector2(0.5f, 0f);
        fbRT.sizeDelta = new Vector2(600, 50);
        fbRT.anchoredPosition = new Vector2(0, 10);
        var fbTMP = GetOrAdd<TextMeshProUGUI>(feedbackGO);
        fbTMP.text = "Stats reset!";
        fbTMP.fontSize = 35;
        fbTMP.color = SUCCESS_COLOR;
        fbTMP.alignment = TextAlignmentOptions.Center;
        fbTMP.raycastTarget = false;
        fbTMP.enableAutoSizing = false;
        feedbackGO.SetActive(false);

        var popup = GetOrAdd<StatsPopup>(root);
        popup.panel = panelRT;
        popup.gamesPlayedText = gamesText;
        popup.wordsTotalText = wordsText;
        popup.longestWordText = longestText;
        popup.bestWordScoreText = bestWordText;
        popup.escapeWinrateText = winrateText;
        popup.timePlayedText = timeText;
        popup.exploreBestText = exploreBestText;
        popup.escapeBestText = escapeBestText;
        popup.closeButton = closeXBtn;
        popup.blockerButton = blockerBtn;
        popup.resetButton = resetBtn;
        popup.resetFeedback = fbTMP;
        return popup;
    }

    private static TextMeshProUGUI AddStatRow(Transform parent, string name, string label, string defaultValue, ref float yPos)
    {
        var row = FindOrCreateChild(parent, name);
        var rowRT = row.GetComponent<RectTransform>();
        rowRT.anchorMin = new Vector2(0.5f, 0.5f);
        rowRT.anchorMax = new Vector2(0.5f, 0.5f);
        rowRT.pivot = new Vector2(0.5f, 0.5f);
        rowRT.sizeDelta = new Vector2(820, 70);
        rowRT.anchoredPosition = new Vector2(0, yPos);

        var labelGO = FindOrCreateChild(row.transform, "Label");
        var labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0, 0);
        labelRT.anchorMax = new Vector2(0.6f, 1);
        labelRT.offsetMin = Vector2.zero;
        labelRT.offsetMax = Vector2.zero;
        var labelTMP = GetOrAdd<TextMeshProUGUI>(labelGO);
        labelTMP.text = label;
        labelTMP.fontSize = 42;
        labelTMP.color = MUTED_COLOR;
        labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
        labelTMP.raycastTarget = false;
        labelTMP.enableAutoSizing = false;

        var valueGO = FindOrCreateChild(row.transform, "Value");
        var valueRT = valueGO.GetComponent<RectTransform>();
        valueRT.anchorMin = new Vector2(0.6f, 0);
        valueRT.anchorMax = new Vector2(1f, 1);
        valueRT.offsetMin = Vector2.zero;
        valueRT.offsetMax = Vector2.zero;
        var valueTMP = GetOrAdd<TextMeshProUGUI>(valueGO);
        valueTMP.text = defaultValue;
        valueTMP.fontSize = 50;
        valueTMP.fontStyle = FontStyles.Bold;
        valueTMP.color = PRIMARY_COLOR;
        valueTMP.alignment = TextAlignmentOptions.MidlineRight;
        valueTMP.raycastTarget = false;
        valueTMP.enableAutoSizing = false;

        yPos -= 95;
        return valueTMP;
    }

    private static void SetupGameScene()
    {
        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found in Game.unity."); return; }

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null) { Debug.LogError("GameController not found."); return; }

        var controller = controllerTR.GetComponent<GameController>();
        var builder = controllerTR.GetComponent<WordBuilder>();
        if (builder == null || controller == null) { Debug.LogError("WordBuilder/GameController missing."); return; }

        builder.gameController = controller;

        EditorUtility.SetDirty(builder);
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
