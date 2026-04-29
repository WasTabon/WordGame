using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupNumberCellsScoreIteration5
{
    private const string SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");

    [MenuItem("WordGame/Setup Number Cells + Score (Iteration 5)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!File.Exists(SCENE_PATH))
        {
            Debug.LogError("Game.unity not found. Run iterations 2 and 3 setup first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null)
        {
            Debug.LogError("Canvas not found.");
            return;
        }

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null)
        {
            Debug.LogError("GameController not found.");
            return;
        }
        var controllerGO = controllerTR.gameObject;

        var hudTR = canvasGO.transform.Find("HUD");
        if (hudTR == null)
        {
            Debug.LogError("HUD not found.");
            return;
        }

        var scoreManager = GetOrAdd<ScoreManager>(controllerGO);

        var controller = controllerGO.GetComponent<GameController>();
        if (controller == null)
        {
            Debug.LogError("GameController component not found.");
            return;
        }
        controller.scoreManager = scoreManager;

        var builder = controllerGO.GetComponent<WordBuilder>();
        if (builder == null)
        {
            Debug.LogError("WordBuilder component not found. Run iteration 3 first.");
            return;
        }
        builder.scoreManager = scoreManager;

        var modeLabelTR = hudTR.Find("ModeLabel");
        if (modeLabelTR != null)
        {
            var modeRT = modeLabelTR.GetComponent<RectTransform>();
            modeRT.anchorMin = new Vector2(1, 1);
            modeRT.anchorMax = new Vector2(1, 1);
            modeRT.pivot = new Vector2(1, 1);
            modeRT.sizeDelta = new Vector2(500, 80);
            modeRT.anchoredPosition = new Vector2(-50, -15);

            var modeTMP = modeLabelTR.GetComponent<TextMeshProUGUI>();
            if (modeTMP != null)
            {
                modeTMP.fontSize = 50;
                modeTMP.alignment = TextAlignmentOptions.MidlineRight;
            }
        }

        var scoreLabelGO = FindOrCreateChild(hudTR, "ScoreLabel");
        var scoreRT = scoreLabelGO.GetComponent<RectTransform>();
        scoreRT.anchorMin = new Vector2(1, 0);
        scoreRT.anchorMax = new Vector2(1, 0);
        scoreRT.pivot = new Vector2(1, 0);
        scoreRT.sizeDelta = new Vector2(500, 100);
        scoreRT.anchoredPosition = new Vector2(-50, 15);

        var scoreTMP = GetOrAdd<TextMeshProUGUI>(scoreLabelGO);
        scoreTMP.text = "0";
        scoreTMP.fontSize = 90;
        scoreTMP.fontStyle = FontStyles.Bold;
        scoreTMP.color = PRIMARY_COLOR;
        scoreTMP.alignment = TextAlignmentOptions.MidlineRight;
        scoreTMP.raycastTarget = false;
        scoreTMP.enableAutoSizing = false;

        var hud = hudTR.GetComponent<GameHUD>();
        if (hud == null) hud = hudTR.gameObject.AddComponent<GameHUD>();
        hud.scoreLabel = scoreTMP;
        hud.scoreManager = scoreManager;

        EditorUtility.SetDirty(scoreManager);
        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(builder);
        EditorUtility.SetDirty(hud);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#E8A745><b>WordGame Iteration 5:</b> Number cells + Score setup complete.</color>");
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
