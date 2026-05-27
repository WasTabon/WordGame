using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupHintsIteration15
{
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    private static readonly Color HINT_BG = Hex("#4A5568");
    private static readonly Color HINT_ACCENT = Hex("#4FCC81");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");
    private static readonly Color TEXT_MUTED = new Color(1f, 1f, 1f, 0.7f);
    private static readonly Color PANEL_COLOR = Hex("#151E2B");
    private static readonly Color BLOCKER_COLOR = new Color(0f, 0f, 0f, 0.55f);
    private static readonly Color BUY_COLOR = Hex("#4FCC81");
    private static readonly Color CLOSE_COLOR = Hex("#4A5568");

    [MenuItem("WordGame/Setup Hints (Iteration 15)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH))
        {
            Debug.LogError("Game.unity not found. Run iterations 1-14 first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var hudTR = canvasGO.transform.Find("HUD");
        if (hudTR == null) { Debug.LogError("HUD not found."); return; }
        var hud = hudTR.GetComponent<GameHUD>();
        if (hud == null) { Debug.LogError("GameHUD component missing."); return; }

        var hintBtnInfo = BuildHintButton(hudTR);

        var highlighterGO = FindOrCreateChild(canvasGO.transform, "HintHighlighter");
        var highlighter = GetOrAdd<HintHighlighter>(highlighterGO);

        var popupsTR = canvasGO.transform.Find("Popups");
        if (popupsTR == null) { Debug.LogError("Popups root not found."); return; }

        var popup = BuildOutOfHintsPopup(popupsTR);

        hud.hintButton = hintBtnInfo.button;
        hud.hintCountLabel = hintBtnInfo.countLabel;
        hud.hintHighlighter = highlighter;
        hud.outOfHintsPopup = popup;

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR != null)
        {
            var ctrl = controllerTR.GetComponent<GameController>();
            if (ctrl != null && hud.gameController == null) hud.gameController = ctrl;
        }

        EditorUtility.SetDirty(hud);
        EditorUtility.SetDirty(highlighter);
        EditorUtility.SetDirty(popup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#4FCC81><b>WordGame Iteration 15:</b> Hints setup complete.</color>\n" +
                  "  Hint button created in HUD (right-bottom).\n" +
                  "  OutOfHintsPopup created with placeholder BuyButton.\n" +
                  "  HintIAPBridge created on popup — attach IAPButton component to BuyButton manually, see README.");
    }

    private struct HintBtnInfo
    {
        public Button button;
        public TextMeshProUGUI countLabel;
    }

    private static HintBtnInfo BuildHintButton(Transform hudTR)
    {
        var btnGO = FindOrCreateChild(hudTR, "HintButton");
        var rt = btnGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);
        rt.sizeDelta = new Vector2(170, 90);
        rt.anchoredPosition = new Vector2(-30, 15);

        var img = GetOrAdd<Image>(btnGO);
        img.color = HINT_BG;
        var btn = GetOrAdd<Button>(btnGO);
        btn.targetGraphic = img;
        GetOrAdd<UIButtonFeedback>(btnGO);

        var iconGO = FindOrCreateChild(btnGO.transform, "Icon");
        var iconRT = iconGO.GetComponent<RectTransform>();
        iconRT.anchorMin = new Vector2(0, 0.5f);
        iconRT.anchorMax = new Vector2(0, 0.5f);
        iconRT.pivot = new Vector2(0, 0.5f);
        iconRT.sizeDelta = new Vector2(70, 70);
        iconRT.anchoredPosition = new Vector2(14, 0);
        var iconTMP = GetOrAdd<TextMeshProUGUI>(iconGO);
        iconTMP.text = "💡";
        iconTMP.fontSize = 50;
        iconTMP.alignment = TextAlignmentOptions.Center;
        iconTMP.color = HINT_ACCENT;
        iconTMP.raycastTarget = false;
        iconTMP.enableAutoSizing = false;

        var countGO = FindOrCreateChild(btnGO.transform, "Count");
        var countRT = countGO.GetComponent<RectTransform>();
        countRT.anchorMin = new Vector2(1, 0.5f);
        countRT.anchorMax = new Vector2(1, 0.5f);
        countRT.pivot = new Vector2(1, 0.5f);
        countRT.sizeDelta = new Vector2(80, 70);
        countRT.anchoredPosition = new Vector2(-18, 0);
        var countTMP = GetOrAdd<TextMeshProUGUI>(countGO);
        countTMP.text = "5";
        countTMP.fontSize = 56;
        countTMP.fontStyle = FontStyles.Bold;
        countTMP.alignment = TextAlignmentOptions.MidlineRight;
        countTMP.color = TEXT_LIGHT;
        countTMP.raycastTarget = false;
        countTMP.enableAutoSizing = false;

        return new HintBtnInfo { button = btn, countLabel = countTMP };
    }

    private static OutOfHintsPopup BuildOutOfHintsPopup(Transform popupsTR)
    {
        var popupGO = FindOrCreateChild(popupsTR, "OutOfHintsPopup");
        var popupRT = popupGO.GetComponent<RectTransform>();
        SetFullStretch(popupRT);

        var canvasGroup = GetOrAdd<CanvasGroup>(popupGO);
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        var popup = GetOrAdd<OutOfHintsPopup>(popupGO);
        GetOrAdd<HintIAPBridge>(popupGO);

        var blockerGO = FindOrCreateChild(popupGO.transform, "Blocker");
        var blockerRT = blockerGO.GetComponent<RectTransform>();
        SetFullStretch(blockerRT);
        var blockerImg = GetOrAdd<Image>(blockerGO);
        blockerImg.color = BLOCKER_COLOR;
        var blockerBtn = GetOrAdd<Button>(blockerGO);
        blockerBtn.targetGraphic = blockerImg;
        var blockerColors = blockerBtn.colors;
        blockerColors.normalColor = BLOCKER_COLOR;
        blockerColors.highlightedColor = BLOCKER_COLOR;
        blockerColors.pressedColor = BLOCKER_COLOR;
        blockerColors.selectedColor = BLOCKER_COLOR;
        blockerBtn.colors = blockerColors;

        var panelGO = FindOrCreateChild(popupGO.transform, "Panel");
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(850, 750);
        panelRT.anchoredPosition = Vector2.zero;
        var panelImg = GetOrAdd<Image>(panelGO);
        panelImg.color = PANEL_COLOR;

        var titleGO = FindOrCreateChild(panelGO.transform, "Title");
        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 1f);
        titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.sizeDelta = new Vector2(800, 130);
        titleRT.anchoredPosition = new Vector2(0, -70);
        var titleTMP = GetOrAdd<TextMeshProUGUI>(titleGO);
        titleTMP.text = "OUT OF HINTS";
        titleTMP.fontSize = 90;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.color = HINT_ACCENT;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.raycastTarget = false;
        titleTMP.enableAutoSizing = false;

        var subGO = FindOrCreateChild(panelGO.transform, "Subtitle");
        var subRT = subGO.GetComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0.5f, 1f);
        subRT.anchorMax = new Vector2(0.5f, 1f);
        subRT.pivot = new Vector2(0.5f, 1f);
        subRT.sizeDelta = new Vector2(740, 160);
        subRT.anchoredPosition = new Vector2(0, -210);
        var subTMP = GetOrAdd<TextMeshProUGUI>(subGO);
        subTMP.text = "Get 10 hints to keep solving boards.\nTip: find 6+ letter words to earn hints for free.";
        subTMP.fontSize = 36;
        subTMP.color = TEXT_MUTED;
        subTMP.alignment = TextAlignmentOptions.Center;
        subTMP.raycastTarget = false;
        subTMP.enableAutoSizing = false;

        var buyBtnGO = FindOrCreateChild(panelGO.transform, "BuyButton");
        var buyRT = buyBtnGO.GetComponent<RectTransform>();
        buyRT.anchorMin = new Vector2(0.5f, 0f);
        buyRT.anchorMax = new Vector2(0.5f, 0f);
        buyRT.pivot = new Vector2(0.5f, 0f);
        buyRT.sizeDelta = new Vector2(620, 150);
        buyRT.anchoredPosition = new Vector2(0, 230);
        var buyImg = GetOrAdd<Image>(buyBtnGO);
        buyImg.color = BUY_COLOR;
        var buyBtn = GetOrAdd<Button>(buyBtnGO);
        buyBtn.targetGraphic = buyImg;
        GetOrAdd<UIButtonFeedback>(buyBtnGO);

        var buyLabelGO = FindOrCreateChild(buyBtnGO.transform, "Label");
        var buyLabelRT = buyLabelGO.GetComponent<RectTransform>();
        SetFullStretch(buyLabelRT);
        var buyLabelTMP = GetOrAdd<TextMeshProUGUI>(buyLabelGO);
        buyLabelTMP.text = "BUY 10 HINTS";
        buyLabelTMP.fontSize = 56;
        buyLabelTMP.fontStyle = FontStyles.Bold;
        buyLabelTMP.color = new Color(0.10f, 0.14f, 0.20f, 1f);
        buyLabelTMP.alignment = TextAlignmentOptions.Center;
        buyLabelTMP.raycastTarget = false;
        buyLabelTMP.enableAutoSizing = false;

        var closeBtnGO = FindOrCreateChild(panelGO.transform, "CloseButton");
        var closeRT = closeBtnGO.GetComponent<RectTransform>();
        closeRT.anchorMin = new Vector2(0.5f, 0f);
        closeRT.anchorMax = new Vector2(0.5f, 0f);
        closeRT.pivot = new Vector2(0.5f, 0f);
        closeRT.sizeDelta = new Vector2(440, 100);
        closeRT.anchoredPosition = new Vector2(0, 70);
        var closeImg = GetOrAdd<Image>(closeBtnGO);
        closeImg.color = CLOSE_COLOR;
        var closeBtn = GetOrAdd<Button>(closeBtnGO);
        closeBtn.targetGraphic = closeImg;
        GetOrAdd<UIButtonFeedback>(closeBtnGO);

        var closeLabelGO = FindOrCreateChild(closeBtnGO.transform, "Label");
        var closeLabelRT = closeLabelGO.GetComponent<RectTransform>();
        SetFullStretch(closeLabelRT);
        var closeLabelTMP = GetOrAdd<TextMeshProUGUI>(closeLabelGO);
        closeLabelTMP.text = "CLOSE";
        closeLabelTMP.fontSize = 44;
        closeLabelTMP.fontStyle = FontStyles.Bold;
        closeLabelTMP.color = TEXT_LIGHT;
        closeLabelTMP.alignment = TextAlignmentOptions.Center;
        closeLabelTMP.raycastTarget = false;
        closeLabelTMP.enableAutoSizing = false;

        popup.closeButton = closeBtn;
        popup.blockerButton = blockerBtn;
        popup.buyButton = buyBtn;
        popup.titleText = titleTMP;
        popup.subtitleText = subTMP;
        popup.buyButtonLabel = buyLabelTMP;

        popupGO.SetActive(false);

        return popup;
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
