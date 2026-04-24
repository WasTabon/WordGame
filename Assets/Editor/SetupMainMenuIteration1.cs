using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class SetupMainMenuIteration1
{
    private const string SCENE_PATH = "Assets/WordGame/Scenes/MainMenu.unity";

    private static readonly Color BG_COLOR = Hex("#1A2332");
    private static readonly Color POPUP_BG_COLOR = Hex("#151E2B");
    private static readonly Color PANEL_COLOR = Hex("#2A3442");
    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color SECONDARY_COLOR = Hex("#4A5568");
    private static readonly Color ACCENT_COLOR = Hex("#5DADE2");
    private static readonly Color TEAL_COLOR = Hex("#4ECDC4");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");
    private static readonly Color TEXT_DARK = Hex("#1A2332");
    private static readonly Color OVERLAY_COLOR = new Color(0f, 0f, 0f, 0.65f);
    private static readonly Color SLIDER_BG_COLOR = Hex("#0F1722");

    [MenuItem("WordGame/Setup Main Menu (Iteration 1)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        EnsureFolders();
        var scene = OpenOrCreateScene();

        BuildSoundManager();
        BuildSceneLoader();

        var canvasGO = BuildMainCanvas();
        BuildEventSystem();

        BuildBackground(canvasGO.transform);
        var title = BuildTitle(canvasGO.transform);
        var playButton = BuildPlayButton(canvasGO.transform);
        var bottomRow = BuildBottomRow(canvasGO.transform);
        var settingsButton = BuildSecondaryButton(bottomRow, "SettingsButton", "SETTINGS", new Vector2(-220, 0));
        var howToPlayButton = BuildSecondaryButton(bottomRow, "HowToPlayButton", "HOW TO PLAY", new Vector2(220, 0));

        var popupsRoot = FindOrCreateChild(canvasGO.transform, "Popups");
        SetFullStretch(popupsRoot.GetComponent<RectTransform>());

        var settingsPopup = BuildSettingsPopup(popupsRoot.transform);
        var howToPlayPopup = BuildHowToPlayPopup(popupsRoot.transform);
        var modeSelectPopup = BuildModeSelectPopup(popupsRoot.transform);
        var placeholderPopup = BuildPlaceholderPopup(popupsRoot.transform);

        var mainMenuUI = GetOrAdd<MainMenuUI>(canvasGO);
        mainMenuUI.titleText = title.GetComponent<RectTransform>();
        mainMenuUI.playButton = playButton;
        mainMenuUI.settingsButton = settingsButton;
        mainMenuUI.howToPlayButton = howToPlayButton;
        mainMenuUI.settingsPopup = settingsPopup;
        mainMenuUI.howToPlayPopup = howToPlayPopup;
        mainMenuUI.modeSelectPopup = modeSelectPopup;

        modeSelectPopup.placeholderPopup = placeholderPopup;

        settingsPopup.gameObject.SetActive(false);
        howToPlayPopup.gameObject.SetActive(false);
        modeSelectPopup.gameObject.SetActive(false);
        placeholderPopup.gameObject.SetActive(false);

        EditorUtility.SetDirty(mainMenuUI);
        EditorUtility.SetDirty(modeSelectPopup);
        EditorUtility.SetDirty(settingsPopup);
        EditorUtility.SetDirty(howToPlayPopup);
        EditorUtility.SetDirty(placeholderPopup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EnsureSceneInBuildSettings(SCENE_PATH);

        Debug.Log("<color=#E8A745><b>WordGame Iteration 1:</b> Main Menu setup complete.</color>");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/WordGame"))
            AssetDatabase.CreateFolder("Assets", "WordGame");
        if (!AssetDatabase.IsValidFolder("Assets/WordGame/Scenes"))
            AssetDatabase.CreateFolder("Assets/WordGame", "Scenes");
        if (!AssetDatabase.IsValidFolder("Assets/WordGame/Scripts"))
            AssetDatabase.CreateFolder("Assets/WordGame", "Scripts");
        if (!AssetDatabase.IsValidFolder("Assets/WordGame/Editor"))
            AssetDatabase.CreateFolder("Assets/WordGame", "Editor");
    }

    private static Scene OpenOrCreateScene()
    {
        if (File.Exists(SCENE_PATH))
        {
            return EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);
        }
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, SCENE_PATH);
        return scene;
    }

    private static void EnsureSceneInBuildSettings(string path)
    {
        var scenes = EditorBuildSettings.scenes;
        foreach (var s in scenes)
        {
            if (s.path == path) return;
        }
        var newList = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes);
        newList.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = newList.ToArray();
    }

    private static void BuildSoundManager()
    {
        var go = FindOrCreateRoot("SoundManager");
        var music = GetOrAdd<AudioSource>(go);
        music.playOnAwake = false;
        music.loop = true;
        music.spatialBlend = 0f;

        var sfxChild = FindOrCreateChild(go.transform, "SfxSource");
        var sfx = GetOrAdd<AudioSource>(sfxChild);
        sfx.playOnAwake = false;
        sfx.spatialBlend = 0f;

        var sm = GetOrAdd<SoundManager>(go);
        sm.musicSource = music;
        sm.sfxSource = sfx;
        EditorUtility.SetDirty(sm);
    }

    private static void BuildSceneLoader()
    {
        var root = FindOrCreateRoot("SceneLoader");
        var canvas = GetOrAdd<Canvas>(root);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        GetOrAdd<CanvasScaler>(root).uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        GetOrAdd<GraphicRaycaster>(root);

        var fade = FindOrCreateChild(root.transform, "FadeImage");
        var fadeRT = GetOrAdd<RectTransform>(fade);
        SetFullStretch(fadeRT);
        var fadeImg = GetOrAdd<Image>(fade);
        fadeImg.color = new Color(0f, 0f, 0f, 0f);
        fadeImg.raycastTarget = false;

        var sl = GetOrAdd<SceneLoader>(root);
        sl.fadeImage = fadeImg;
        EditorUtility.SetDirty(sl);
    }

    private static GameObject BuildMainCanvas()
    {
        var canvasGO = FindOrCreateRoot("Canvas");
        var canvas = GetOrAdd<Canvas>(canvasGO);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        var scaler = GetOrAdd<CanvasScaler>(canvasGO);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        GetOrAdd<GraphicRaycaster>(canvasGO);
        return canvasGO;
    }

    private static void BuildEventSystem()
    {
        var existing = Object.FindObjectOfType<EventSystem>();
        if (existing != null) return;
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    private static void BuildBackground(Transform parent)
    {
        var bg = FindOrCreateChild(parent, "Background");
        var rt = GetOrAdd<RectTransform>(bg);
        SetFullStretch(rt);
        var img = GetOrAdd<Image>(bg);
        img.color = BG_COLOR;
        img.raycastTarget = false;
        bg.transform.SetAsFirstSibling();
    }

    private static RectTransform BuildTitle(Transform parent)
    {
        var go = FindOrCreateChild(parent, "TitleText");
        var rt = GetOrAdd<RectTransform>(go);
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(1000, 260);
        rt.anchoredPosition = new Vector2(0, -280);

        var tmp = GetOrAdd<TextMeshProUGUI>(go);
        tmp.text = "WORD GAME";
        tmp.fontSize = 140;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = TEXT_LIGHT;
        tmp.enableAutoSizing = false;
        tmp.raycastTarget = false;
        return rt;
    }

    private static Button BuildPlayButton(Transform parent)
    {
        var btn = CreateButton(parent, "PlayButton", "PLAY", PRIMARY_COLOR, TEXT_DARK, new Vector2(720, 200), 100);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, -40);
        return btn;
    }

    private static Transform BuildBottomRow(Transform parent)
    {
        var row = FindOrCreateChild(parent, "BottomRow");
        var rt = GetOrAdd<RectTransform>(row);
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(950, 160);
        rt.anchoredPosition = new Vector2(0, 260);
        return row.transform;
    }

    private static Button BuildSecondaryButton(Transform parent, string name, string label, Vector2 anchoredPos)
    {
        var btn = CreateButton(parent, name, label, SECONDARY_COLOR, TEXT_LIGHT, new Vector2(430, 140), 55);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        return btn;
    }

    private static SettingsPopup BuildSettingsPopup(Transform parent)
    {
        var frame = CreatePopupFrame(parent, "SettingsPopup", new Vector2(920, 1200), "SETTINGS");
        var panel = frame.panel;

        var musicLabel = CreateText(panel, "MusicLabel", "Music", 60, TEXT_LIGHT, TextAlignmentOptions.MidlineLeft);
        PositionElement(musicLabel.GetComponent<RectTransform>(), new Vector2(-200, 280), new Vector2(320, 80));

        var musicValue = CreateText(panel, "MusicValue", "100%", 55, ACCENT_COLOR, TextAlignmentOptions.MidlineRight);
        PositionElement(musicValue.GetComponent<RectTransform>(), new Vector2(280, 280), new Vector2(240, 80));

        var musicSlider = CreateSlider(panel, "MusicSlider", new Vector2(760, 50));
        PositionElement(musicSlider.GetComponent<RectTransform>(), new Vector2(0, 180), new Vector2(760, 50));

        var sfxLabel = CreateText(panel, "SfxLabel", "SFX", 60, TEXT_LIGHT, TextAlignmentOptions.MidlineLeft);
        PositionElement(sfxLabel.GetComponent<RectTransform>(), new Vector2(-200, 20), new Vector2(320, 80));

        var sfxValue = CreateText(panel, "SfxValue", "100%", 55, ACCENT_COLOR, TextAlignmentOptions.MidlineRight);
        PositionElement(sfxValue.GetComponent<RectTransform>(), new Vector2(280, 20), new Vector2(240, 80));

        var sfxSlider = CreateSlider(panel, "SfxSlider", new Vector2(760, 50));
        PositionElement(sfxSlider.GetComponent<RectTransform>(), new Vector2(0, -80), new Vector2(760, 50));

        var closeBtn = CreateButton(panel, "CloseButton", "CLOSE", PRIMARY_COLOR, TEXT_DARK, new Vector2(600, 140), 65);
        PositionElement(closeBtn.GetComponent<RectTransform>(), new Vector2(0, -480), new Vector2(600, 140));

        var popup = GetOrAdd<SettingsPopup>(frame.root);
        popup.panel = panel;
        popup.musicSlider = musicSlider;
        popup.musicValueText = musicValue;
        popup.sfxSlider = sfxSlider;
        popup.sfxValueText = sfxValue;
        popup.closeButton = closeBtn;
        popup.blockerButton = frame.blockerBtn;
        musicSlider.value = 0.7f;
        sfxSlider.value = 1f;
        return popup;
    }

    private static HowToPlayPopup BuildHowToPlayPopup(Transform parent)
    {
        var frame = CreatePopupFrame(parent, "HowToPlayPopup", new Vector2(940, 1400), "HOW TO PLAY");
        var panel = frame.panel;

        var rulesText = CreateText(panel, "RulesText",
            "• Swipe across adjacent hexagon letters to form words.\n\n" +
            "• Every word must <b>begin on a cell next to an empty space</b>.\n\n" +
            "• Valid words must exist in the dictionary.\n\n" +
            "• After a valid word, used letters turn into new empty spaces — the board grows outward.\n\n" +
            "• Cells showing a <b>number</b> require a word of at least that length.\n\n" +
            "• <b>EXPLORE</b> mode: play freely, aim for the highest score.\n\n" +
            "• <b>ESCAPE</b> mode: reach the edge before time runs out.",
            38, TEXT_LIGHT, TextAlignmentOptions.TopLeft);
        rulesText.enableWordWrapping = true;
        PositionElement(rulesText.GetComponent<RectTransform>(), new Vector2(0, -10), new Vector2(820, 950));

        var closeBtn = CreateButton(panel, "CloseButton", "GOT IT", PRIMARY_COLOR, TEXT_DARK, new Vector2(600, 140), 65);
        PositionElement(closeBtn.GetComponent<RectTransform>(), new Vector2(0, -580), new Vector2(600, 140));

        var popup = GetOrAdd<HowToPlayPopup>(frame.root);
        popup.panel = panel;
        popup.closeButton = closeBtn;
        popup.blockerButton = frame.blockerBtn;
        return popup;
    }

    private static ModeSelectPopup BuildModeSelectPopup(Transform parent)
    {
        var frame = CreatePopupFrame(parent, "ModeSelectPopup", new Vector2(920, 1350), "SELECT MODE");
        var panel = frame.panel;

        var escapeBtn = CreateRichButton(panel, "EscapeButton",
            "<b><size=85>ESCAPE</size></b>\n<size=40>Reach the edge before time runs out</size>",
            PRIMARY_COLOR, TEXT_DARK, new Vector2(780, 300));
        PositionElement(escapeBtn.GetComponent<RectTransform>(), new Vector2(0, 240), new Vector2(780, 300));

        var exploreBtn = CreateRichButton(panel, "ExploreButton",
            "<b><size=85>EXPLORE</size></b>\n<size=40>Play freely for the highest score</size>",
            TEAL_COLOR, TEXT_DARK, new Vector2(780, 300));
        PositionElement(exploreBtn.GetComponent<RectTransform>(), new Vector2(0, -140), new Vector2(780, 300));

        var closeBtn = CreateButton(panel, "CloseButton", "BACK", SECONDARY_COLOR, TEXT_LIGHT, new Vector2(600, 140), 60);
        PositionElement(closeBtn.GetComponent<RectTransform>(), new Vector2(0, -555), new Vector2(600, 140));

        var popup = GetOrAdd<ModeSelectPopup>(frame.root);
        popup.panel = panel;
        popup.escapeButton = escapeBtn;
        popup.exploreButton = exploreBtn;
        popup.closeButton = closeBtn;
        popup.blockerButton = frame.blockerBtn;
        return popup;
    }

    private static PlaceholderPopup BuildPlaceholderPopup(Transform parent)
    {
        var frame = CreatePopupFrame(parent, "PlaceholderPopup", new Vector2(820, 720), "COMING SOON");
        var panel = frame.panel;

        var modeText = CreateText(panel, "ModeText",
            "Selected mode: <b>EXPLORE</b>\n\nGame scene will be available\nin the next iteration.",
            42, TEXT_LIGHT, TextAlignmentOptions.Center);
        modeText.enableWordWrapping = true;
        PositionElement(modeText.GetComponent<RectTransform>(), new Vector2(0, 30), new Vector2(720, 320));

        var okBtn = CreateButton(panel, "OKButton", "OK", PRIMARY_COLOR, TEXT_DARK, new Vector2(500, 130), 60);
        PositionElement(okBtn.GetComponent<RectTransform>(), new Vector2(0, -240), new Vector2(500, 130));

        var popup = GetOrAdd<PlaceholderPopup>(frame.root);
        popup.panel = panel;
        popup.okButton = okBtn;
        popup.modeText = modeText;
        return popup;
    }

    private struct PopupFrame
    {
        public GameObject root;
        public RectTransform panel;
        public Button closeBtn;
        public Button blockerBtn;
    }

    private static PopupFrame CreatePopupFrame(Transform parent, string name, Vector2 panelSize, string title)
    {
        var root = FindOrCreateChild(parent, name);
        var rootRT = GetOrAdd<RectTransform>(root);
        SetFullStretch(rootRT);
        GetOrAdd<CanvasGroup>(root);

        var blocker = FindOrCreateChild(root.transform, "Blocker");
        var blockerRT = GetOrAdd<RectTransform>(blocker);
        SetFullStretch(blockerRT);
        var blockerImg = GetOrAdd<Image>(blocker);
        blockerImg.color = OVERLAY_COLOR;
        blockerImg.raycastTarget = true;
        var blockerBtn = GetOrAdd<Button>(blocker);
        blockerBtn.transition = Selectable.Transition.None;

        var panel = FindOrCreateChild(root.transform, "Panel");
        var panelRT = GetOrAdd<RectTransform>(panel);
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = panelSize;
        panelRT.anchoredPosition = Vector2.zero;
        var panelImg = GetOrAdd<Image>(panel);
        panelImg.color = POPUP_BG_COLOR;
        panelImg.raycastTarget = true;

        var titleGO = CreateText(panelRT, "Title", title, 80, TEXT_LIGHT, TextAlignmentOptions.Center);
        titleGO.fontStyle = FontStyles.Bold;
        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 1f);
        titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.sizeDelta = new Vector2(800, 120);
        titleRT.anchoredPosition = new Vector2(0, -60);

        var accent = FindOrCreateChild(panelRT, "TitleAccent");
        var accentRT = GetOrAdd<RectTransform>(accent);
        accentRT.anchorMin = new Vector2(0.5f, 1f);
        accentRT.anchorMax = new Vector2(0.5f, 1f);
        accentRT.pivot = new Vector2(0.5f, 1f);
        accentRT.sizeDelta = new Vector2(160, 8);
        accentRT.anchoredPosition = new Vector2(0, -195);
        var accentImg = GetOrAdd<Image>(accent);
        accentImg.color = PRIMARY_COLOR;
        accentImg.raycastTarget = false;

        var closeX = FindOrCreateChild(panelRT, "CloseXButton");
        var closeXRT = GetOrAdd<RectTransform>(closeX);
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

        var closeXText = CreateText(closeX.transform, "Label", "×", 80, TEXT_LIGHT, TextAlignmentOptions.Center);
        closeXText.fontStyle = FontStyles.Bold;
        var closeXTextRT = closeXText.GetComponent<RectTransform>();
        SetFullStretch(closeXTextRT);

        return new PopupFrame
        {
            root = root,
            panel = panelRT,
            closeBtn = closeXBtn,
            blockerBtn = blockerBtn
        };
    }

    private static Image CreateImage(Transform parent, string name, Color color)
    {
        var go = FindOrCreateChild(parent, name);
        GetOrAdd<RectTransform>(go);
        var img = GetOrAdd<Image>(go);
        img.color = color;
        return img;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions align)
    {
        var go = FindOrCreateChild(parent, name);
        GetOrAdd<RectTransform>(go);
        var tmp = GetOrAdd<TextMeshProUGUI>(go);
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = align;
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = false;
        return tmp;
    }

    private static Button CreateButton(Transform parent, string name, string label, Color bgColor, Color textColor, Vector2 size, float labelFontSize)
    {
        var go = FindOrCreateChild(parent, name);
        var rt = GetOrAdd<RectTransform>(go);
        rt.sizeDelta = size;
        var img = GetOrAdd<Image>(go);
        img.color = bgColor;
        var btn = GetOrAdd<Button>(go);
        btn.targetGraphic = img;
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.05f, 1.05f, 1.05f, 1f);
        colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.selectedColor = Color.white;
        btn.colors = colors;
        GetOrAdd<UIButtonFeedback>(go);

        var labelTMP = CreateText(go.transform, "Label", label, labelFontSize, textColor, TextAlignmentOptions.Center);
        labelTMP.fontStyle = FontStyles.Bold;
        var labelRT = labelTMP.GetComponent<RectTransform>();
        SetFullStretch(labelRT);
        return btn;
    }

    private static Button CreateRichButton(Transform parent, string name, string richLabel, Color bgColor, Color textColor, Vector2 size)
    {
        var go = FindOrCreateChild(parent, name);
        var rt = GetOrAdd<RectTransform>(go);
        rt.sizeDelta = size;
        var img = GetOrAdd<Image>(go);
        img.color = bgColor;
        var btn = GetOrAdd<Button>(go);
        btn.targetGraphic = img;
        GetOrAdd<UIButtonFeedback>(go);

        var labelTMP = CreateText(go.transform, "Label", richLabel, 50, textColor, TextAlignmentOptions.Center);
        labelTMP.richText = true;
        var labelRT = labelTMP.GetComponent<RectTransform>();
        SetFullStretch(labelRT);
        return btn;
    }

    private static Slider CreateSlider(Transform parent, string name, Vector2 size)
    {
        var go = FindOrCreateChild(parent, name);
        var rt = GetOrAdd<RectTransform>(go);
        rt.sizeDelta = size;

        var bg = FindOrCreateChild(go.transform, "Background");
        var bgRT = GetOrAdd<RectTransform>(bg);
        bgRT.anchorMin = new Vector2(0, 0.3f);
        bgRT.anchorMax = new Vector2(1, 0.7f);
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        var bgImg = GetOrAdd<Image>(bg);
        bgImg.color = SLIDER_BG_COLOR;

        var fillArea = FindOrCreateChild(go.transform, "Fill Area");
        var fillAreaRT = GetOrAdd<RectTransform>(fillArea);
        fillAreaRT.anchorMin = new Vector2(0, 0.3f);
        fillAreaRT.anchorMax = new Vector2(1, 0.7f);
        fillAreaRT.offsetMin = new Vector2(10, 0);
        fillAreaRT.offsetMax = new Vector2(-10, 0);

        var fill = FindOrCreateChild(fillArea.transform, "Fill");
        var fillRT = GetOrAdd<RectTransform>(fill);
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        var fillImg = GetOrAdd<Image>(fill);
        fillImg.color = ACCENT_COLOR;

        var handleArea = FindOrCreateChild(go.transform, "Handle Slide Area");
        var handleAreaRT = GetOrAdd<RectTransform>(handleArea);
        handleAreaRT.anchorMin = Vector2.zero;
        handleAreaRT.anchorMax = Vector2.one;
        handleAreaRT.offsetMin = new Vector2(20, 0);
        handleAreaRT.offsetMax = new Vector2(-20, 0);

        var handle = FindOrCreateChild(handleArea.transform, "Handle");
        var handleRT = GetOrAdd<RectTransform>(handle);
        handleRT.anchorMin = new Vector2(0, 0.5f);
        handleRT.anchorMax = new Vector2(0, 0.5f);
        handleRT.pivot = new Vector2(0.5f, 0.5f);
        handleRT.sizeDelta = new Vector2(50, 50);
        var handleImg = GetOrAdd<Image>(handle);
        handleImg.color = TEXT_LIGHT;

        var slider = GetOrAdd<Slider>(go);
        slider.fillRect = fillRT;
        slider.handleRect = handleRT;
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;

        return slider;
    }

    private static void PositionElement(RectTransform rt, Vector2 anchoredPos, Vector2 size)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
    }

    private static GameObject FindOrCreateRoot(string name)
    {
        var go = GameObject.Find("/" + name);
        if (go != null) return go;
        return new GameObject(name);
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
