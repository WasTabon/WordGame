using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupLevelsEscapeIteration12
{
    private const string MAINMENU_SCENE_PATH = "Assets/WordGame/Scenes/MainMenu.unity";

    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color MUTED_COLOR = new Color(1f, 1f, 1f, 0.6f);

    [MenuItem("WordGame/Setup Levels Escape (Iteration 12)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(MAINMENU_SCENE_PATH))
        {
            Debug.LogError("MainMenu.unity not found. Run iterations 1-11 first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(MAINMENU_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var popupsTR = canvasGO.transform.Find("Popups");
        if (popupsTR == null) { Debug.LogError("Popups root not found."); return; }

        var statsTR = popupsTR.Find("StatsPopup");
        if (statsTR == null)
        {
            Debug.LogError("StatsPopup not found. Run iteration 10 setup first.");
            return;
        }

        var statsPopup = statsTR.GetComponent<StatsPopup>();
        if (statsPopup == null) { Debug.LogError("StatsPopup component missing."); return; }

        var rowsRoot = statsTR.Find("Panel/Rows");
        if (rowsRoot == null) { Debug.LogError("StatsPopup Rows root not found."); return; }

        var winrateRow = rowsRoot.Find("EscapeWinrate");
        if (winrateRow == null) { Debug.LogError("EscapeWinrate row not found."); return; }

        var winrateRT = winrateRow as RectTransform;
        float baseY = winrateRT.anchoredPosition.y;

        var levelRow = FindOrCreateChild(rowsRoot, "EscapeLevel");
        var rowRT = levelRow.GetComponent<RectTransform>();
        rowRT.anchorMin = new Vector2(0.5f, 0.5f);
        rowRT.anchorMax = new Vector2(0.5f, 0.5f);
        rowRT.pivot = new Vector2(0.5f, 0.5f);
        rowRT.sizeDelta = new Vector2(820, 70);
        rowRT.anchoredPosition = new Vector2(0, baseY - 95);

        var levelLabelGO = FindOrCreateChild(levelRow.transform, "Label");
        var labelRT = levelLabelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0, 0);
        labelRT.anchorMax = new Vector2(0.6f, 1);
        labelRT.offsetMin = Vector2.zero;
        labelRT.offsetMax = Vector2.zero;
        var labelTMP = GetOrAdd<TextMeshProUGUI>(levelLabelGO);
        labelTMP.text = "Escape level";
        labelTMP.fontSize = 42;
        labelTMP.color = MUTED_COLOR;
        labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
        labelTMP.raycastTarget = false;
        labelTMP.enableAutoSizing = false;

        var levelValueGO = FindOrCreateChild(levelRow.transform, "Value");
        var valueRT = levelValueGO.GetComponent<RectTransform>();
        valueRT.anchorMin = new Vector2(0.6f, 0);
        valueRT.anchorMax = new Vector2(1f, 1);
        valueRT.offsetMin = Vector2.zero;
        valueRT.offsetMax = Vector2.zero;
        var valueTMP = GetOrAdd<TextMeshProUGUI>(levelValueGO);
        valueTMP.text = "Lv 1";
        valueTMP.fontSize = 50;
        valueTMP.fontStyle = FontStyles.Bold;
        valueTMP.color = PRIMARY_COLOR;
        valueTMP.alignment = TextAlignmentOptions.MidlineRight;
        valueTMP.raycastTarget = false;
        valueTMP.enableAutoSizing = false;

        statsPopup.highestLevelText = valueTMP;

        var rowsToShift = new[] { "TimePlayed", "Divider", "ExploreBest", "EscapeBest" };
        for (int i = 0; i < rowsToShift.Length; i++)
        {
            var t = rowsRoot.Find(rowsToShift[i]);
            if (t == null) continue;
            var rt = t as RectTransform;
            var p = rt.anchoredPosition;
            rt.anchoredPosition = new Vector2(p.x, p.y - 95);
        }

        EditorUtility.SetDirty(statsPopup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#E8A745><b>WordGame Iteration 12:</b> Levels in Escape setup complete.</color>");
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

    private static Color Hex(string hex)
    {
        Color c;
        ColorUtility.TryParseHtmlString(hex, out c);
        return c;
    }
}
