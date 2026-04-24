using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class SetupGameSceneIteration2
{
    private const string SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";
    private const string SPRITE_PATH = "Assets/WordGame/Sprites/Hex.png";

    private static readonly Color BG_COLOR = Hex("#1A2332");
    private static readonly Color HUD_BG_COLOR = Hex("#151E2B");
    private static readonly Color PRIMARY_COLOR = Hex("#E8A745");
    private static readonly Color SECONDARY_COLOR = Hex("#4A5568");
    private static readonly Color TEAL_COLOR = Hex("#4ECDC4");
    private static readonly Color TEXT_LIGHT = Hex("#FFFFFF");

    [MenuItem("WordGame/Setup Game Scene (Iteration 2)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        EnsureFolders();
        var hexSprite = EnsureHexSprite();

        var scene = OpenOrCreateScene();

        var canvasGO = BuildCanvas();
        BuildEventSystem();
        BuildBackground(canvasGO.transform);

        var hud = BuildHUD(canvasGO.transform);
        var gridContainer = BuildGridContainer(canvasGO.transform);

        var controllerGO = FindOrCreateChild(canvasGO.transform, "GameController");
        var hexGrid = GetOrAdd<HexGrid>(controllerGO);
        hexGrid.container = gridContainer;
        hexGrid.cellSprite = hexSprite;
        hexGrid.cellSize = 75f;
        hexGrid.gridRadius = 3;

        var controller = GetOrAdd<GameController>(controllerGO);
        controller.grid = hexGrid;

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(hexGrid);
        EditorUtility.SetDirty(hud);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EnsureSceneInBuildSettings(SCENE_PATH);

        Debug.Log("<color=#4ECDC4><b>WordGame Iteration 2:</b> Game scene setup complete.</color>");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/WordGame"))
            AssetDatabase.CreateFolder("Assets", "WordGame");
        if (!AssetDatabase.IsValidFolder("Assets/WordGame/Scenes"))
            AssetDatabase.CreateFolder("Assets/WordGame", "Scenes");
        if (!AssetDatabase.IsValidFolder("Assets/WordGame/Sprites"))
            AssetDatabase.CreateFolder("Assets/WordGame", "Sprites");
    }

    private static Sprite EnsureHexSprite()
    {
        var existing = AssetDatabase.LoadAssetAtPath<Sprite>(SPRITE_PATH);
        if (existing != null) return existing;

        int w = 174;
        int h = 200;
        float s = 100f;
        float cx = w * 0.5f;
        float cy = h * 0.5f;
        float sqrt3 = Mathf.Sqrt(3f);
        int samples = 3;

        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var pixels = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int inside = 0;
                for (int sy = 0; sy < samples; sy++)
                {
                    for (int sx = 0; sx < samples; sx++)
                    {
                        float px = x + (sx + 0.5f) / samples;
                        float py = y + (sy + 0.5f) / samples;
                        float dx = Mathf.Abs(px - cx);
                        float dy = Mathf.Abs(py - cy);
                        if (dx <= s * sqrt3 * 0.5f && dy <= s - dx / sqrt3) inside++;
                    }
                }
                float a = inside / (float)(samples * samples);
                pixels[y * w + x] = new Color(1f, 1f, 1f, a);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        byte[] png = tex.EncodeToPNG();
        Object.DestroyImmediate(tex);

        File.WriteAllBytes(SPRITE_PATH, png);
        AssetDatabase.ImportAsset(SPRITE_PATH);

        var importer = AssetImporter.GetAtPath(SPRITE_PATH) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(SPRITE_PATH);
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
        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes);
        list.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = list.ToArray();
    }

    private static GameObject BuildCanvas()
    {
        var go = FindOrCreateRoot("Canvas");
        var canvas = GetOrAdd<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        var scaler = GetOrAdd<CanvasScaler>(go);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        GetOrAdd<GraphicRaycaster>(go);
        return go;
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
        var rt = bg.GetComponent<RectTransform>();
        SetFullStretch(rt);
        var img = GetOrAdd<Image>(bg);
        img.color = BG_COLOR;
        img.raycastTarget = false;
        bg.transform.SetAsFirstSibling();
    }

    private static GameHUD BuildHUD(Transform parent)
    {
        var hud = FindOrCreateChild(parent, "HUD");
        var rt = hud.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(0, 200);
        rt.anchoredPosition = Vector2.zero;

        var hudBgImg = GetOrAdd<Image>(hud);
        hudBgImg.color = HUD_BG_COLOR;
        hudBgImg.raycastTarget = true;

        var backBtnGO = FindOrCreateChild(hud.transform, "BackButton");
        var backRT = backBtnGO.GetComponent<RectTransform>();
        backRT.anchorMin = new Vector2(0, 0.5f);
        backRT.anchorMax = new Vector2(0, 0.5f);
        backRT.pivot = new Vector2(0, 0.5f);
        backRT.sizeDelta = new Vector2(220, 130);
        backRT.anchoredPosition = new Vector2(30, 0);
        var backImg = GetOrAdd<Image>(backBtnGO);
        backImg.color = SECONDARY_COLOR;
        var backBtn = GetOrAdd<Button>(backBtnGO);
        backBtn.targetGraphic = backImg;
        GetOrAdd<UIButtonFeedback>(backBtnGO);

        var backLabel = CreateText(backBtnGO.transform, "Label", "← BACK", 55, TEXT_LIGHT, TextAlignmentOptions.Center);
        backLabel.fontStyle = FontStyles.Bold;
        SetFullStretch(backLabel.GetComponent<RectTransform>());

        var modeLabel = CreateText(hud.transform, "ModeLabel", "MODE", 70, PRIMARY_COLOR, TextAlignmentOptions.MidlineRight);
        modeLabel.fontStyle = FontStyles.Bold;
        var modeRT = modeLabel.GetComponent<RectTransform>();
        modeRT.anchorMin = new Vector2(1, 0.5f);
        modeRT.anchorMax = new Vector2(1, 0.5f);
        modeRT.pivot = new Vector2(1, 0.5f);
        modeRT.sizeDelta = new Vector2(500, 130);
        modeRT.anchoredPosition = new Vector2(-50, 0);

        var hudComp = GetOrAdd<GameHUD>(hud);
        hudComp.backButton = backBtn;
        hudComp.modeLabel = modeLabel;
        return hudComp;
    }

    private static RectTransform BuildGridContainer(Transform parent)
    {
        var go = FindOrCreateChild(parent, "GridContainer");
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(1080, 1200);
        rt.anchoredPosition = new Vector2(0, -50);
        return rt;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions align)
    {
        var go = FindOrCreateChild(parent, name);
        var tmp = GetOrAdd<TextMeshProUGUI>(go);
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = align;
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = false;
        return tmp;
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
