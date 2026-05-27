using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public static class GenerateButtonIconsEditor
{
    private const int FINAL_SIZE = 128;
    private const int SUPERSAMPLE = 3;
    private const string SPRITES_DIR = "Assets/WordGame/Sprites";
    private const string PAN_PATH = "Assets/WordGame/Sprites/Icon_Pan.png";
    private const string HINT_PATH = "Assets/WordGame/Sprites/Icon_Hint.png";
    private const string BUY_PATH = "Assets/WordGame/Sprites/Icon_Buy.png";
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    [MenuItem("WordGame/Generate Button Icons")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before generating icons.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(SPRITES_DIR))
        {
            AssetDatabase.CreateFolder("Assets/WordGame", "Sprites");
        }

        SaveSprite(PAN_PATH, BuildPanIcon());
        SaveSprite(HINT_PATH, BuildHintIcon());
        SaveSprite(BUY_PATH, BuildBuyIcon());

        AssetDatabase.Refresh();

        var panSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PAN_PATH);
        var hintSprite = AssetDatabase.LoadAssetAtPath<Sprite>(HINT_PATH);
        var buySprite = AssetDatabase.LoadAssetAtPath<Sprite>(BUY_PATH);

        if (panSprite == null || hintSprite == null || buySprite == null)
        {
            Debug.LogError("Failed to load generated sprites. Check console for errors.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH))
        {
            Debug.LogWarning("Game.unity not found — sprites generated but not wired into scene.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        WirePanIcon(canvasGO.transform, panSprite);
        WireHintIcon(canvasGO.transform, hintSprite);
        WireBuyIcon(canvasGO.transform, buySprite);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#4FCC81><b>Button icons generated and wired:</b></color>\n" +
                  "  " + PAN_PATH + "\n" +
                  "  " + HINT_PATH + "\n" +
                  "  " + BUY_PATH);
    }

    private static void WirePanIcon(Transform canvasTR, Sprite sprite)
    {
        var btnTR = canvasTR.Find("HUD/PanButton");
        if (btnTR == null) { Debug.LogWarning("PanButton not found."); return; }

        ReplaceTextChildWithImage(btnTR, "Label", "Icon", sprite, new Vector2(70, 70), Vector2.zero);
        ReplaceTextChildWithImage(btnTR, "Icon", "Icon", sprite, new Vector2(70, 70), Vector2.zero);
    }

    private static void WireHintIcon(Transform canvasTR, Sprite sprite)
    {
        var btnTR = canvasTR.Find("HUD/HintButton");
        if (btnTR == null) { Debug.LogWarning("HintButton not found."); return; }

        var iconTR = btnTR.Find("Icon");
        if (iconTR != null) Object.DestroyImmediate(iconTR.gameObject);

        var go = new GameObject("Icon", typeof(RectTransform));
        go.transform.SetParent(btnTR, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(64, 64);
        rt.anchoredPosition = new Vector2(16, 0);
        var img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.color = new Color(0.31f, 0.80f, 0.51f, 1f);
        img.raycastTarget = false;
        img.preserveAspect = true;
    }

    private static void WireBuyIcon(Transform canvasTR, Sprite sprite)
    {
        var btnTR = canvasTR.Find("Popups/OutOfHintsPopup/Panel/BuyButton");
        if (btnTR == null) { Debug.LogWarning("BuyButton not found."); return; }

        var labelTR = btnTR.Find("Label");
        if (labelTR != null)
        {
            var labelRT = labelTR.GetComponent<RectTransform>();
            labelRT.anchorMin = new Vector2(0, 0);
            labelRT.anchorMax = new Vector2(1, 1);
            labelRT.pivot = new Vector2(0.5f, 0.5f);
            labelRT.offsetMin = new Vector2(140, 0);
            labelRT.offsetMax = Vector2.zero;
        }

        var iconTR = btnTR.Find("Icon");
        GameObject iconGO;
        if (iconTR != null) iconGO = iconTR.gameObject;
        else
        {
            iconGO = new GameObject("Icon", typeof(RectTransform));
            iconGO.transform.SetParent(btnTR, false);
        }

        var rt = iconGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(110, 110);
        rt.anchoredPosition = new Vector2(20, 0);
        var img = iconGO.GetComponent<Image>();
        if (img == null) img = iconGO.AddComponent<Image>();
        img.sprite = sprite;
        img.color = new Color(0.10f, 0.14f, 0.20f, 1f);
        img.raycastTarget = false;
        img.preserveAspect = true;
    }

    private static void ReplaceTextChildWithImage(Transform parent, string searchName, string newName, Sprite sprite, Vector2 size, Vector2 anchoredPos)
    {
        var existing = parent.Find(searchName);
        if (existing == null) return;

        var tmp = existing.GetComponent<TextMeshProUGUI>();
        if (tmp == null) return;

        Object.DestroyImmediate(existing.gameObject);

        var go = new GameObject(newName, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        var img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.color = Color.white;
        img.raycastTarget = false;
        img.preserveAspect = true;
    }

    private static void SaveSprite(string path, Color32[] pixels)
    {
        var tex = new Texture2D(FINAL_SIZE, FINAL_SIZE, TextureFormat.RGBA32, false);
        tex.SetPixels32(pixels);
        tex.Apply();

        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    private static Color32[] BuildPanIcon()
    {
        int s = FINAL_SIZE * SUPERSAMPLE;
        var px = NewBuffer(s);
        Color32 white = new Color32(255, 255, 255, 255);

        float cx = s * 0.5f;
        float cy = s * 0.5f;
        float strokeHalf = s * 0.04f;
        float armLen = s * 0.30f;

        DrawRectFilled(px, s, cx - armLen, cy - strokeHalf, armLen * 2, strokeHalf * 2, white);
        DrawRectFilled(px, s, cx - strokeHalf, cy - armLen, strokeHalf * 2, armLen * 2, white);

        float headOut = s * 0.10f;
        float headHalf = s * 0.085f;

        DrawTriangleFilled(px, s,
            new Vector2(cx + armLen + headOut, cy),
            new Vector2(cx + armLen, cy - headHalf),
            new Vector2(cx + armLen, cy + headHalf), white);
        DrawTriangleFilled(px, s,
            new Vector2(cx - armLen - headOut, cy),
            new Vector2(cx - armLen, cy - headHalf),
            new Vector2(cx - armLen, cy + headHalf), white);
        DrawTriangleFilled(px, s,
            new Vector2(cx, cy + armLen + headOut),
            new Vector2(cx - headHalf, cy + armLen),
            new Vector2(cx + headHalf, cy + armLen), white);
        DrawTriangleFilled(px, s,
            new Vector2(cx, cy - armLen - headOut),
            new Vector2(cx - headHalf, cy - armLen),
            new Vector2(cx + headHalf, cy - armLen), white);

        return Downsample(px, s);
    }

    private static Color32[] BuildHintIcon()
    {
        int s = FINAL_SIZE * SUPERSAMPLE;
        var px = NewBuffer(s);
        Color32 white = new Color32(255, 255, 255, 255);

        float cx = s * 0.5f;
        float bulbCy = s * 0.56f;
        float bulbR = s * 0.22f;

        DrawCircleFilled(px, s, cx, bulbCy, bulbR, white);

        float baseTopY = bulbCy - bulbR * 0.85f;
        float baseHalfW = s * 0.12f;
        float baseHeight = s * 0.08f;
        DrawRectFilled(px, s, cx - baseHalfW, baseTopY - baseHeight, baseHalfW * 2, baseHeight, white);

        float capHalfW = s * 0.09f;
        float capHeight = s * 0.05f;
        DrawRectFilled(px, s, cx - capHalfW, baseTopY - baseHeight - capHeight, capHalfW * 2, capHeight, white);

        float rayStart = bulbR + s * 0.04f;
        float rayLen = s * 0.10f;
        float rayThick = s * 0.025f;
        float[] angles = new float[] { 30f, 60f, 90f, 120f, 150f };
        for (int i = 0; i < angles.Length; i++)
        {
            float rad = angles[i] * Mathf.Deg2Rad;
            float x0 = cx + Mathf.Cos(rad) * rayStart;
            float y0 = bulbCy + Mathf.Sin(rad) * rayStart;
            float x1 = cx + Mathf.Cos(rad) * (rayStart + rayLen);
            float y1 = bulbCy + Mathf.Sin(rad) * (rayStart + rayLen);
            DrawLine(px, s, x0, y0, x1, y1, rayThick, white);
        }

        return Downsample(px, s);
    }

    private static Color32[] BuildBuyIcon()
    {
        int s = FINAL_SIZE * SUPERSAMPLE;
        var px = NewBuffer(s);
        Color32 white = new Color32(255, 255, 255, 255);

        Vector2[] verts = new Vector2[]
        {
            new Vector2(s * 0.12f, s * 0.70f),
            new Vector2(s * 0.62f, s * 0.70f),
            new Vector2(s * 0.86f, s * 0.50f),
            new Vector2(s * 0.62f, s * 0.30f),
            new Vector2(s * 0.12f, s * 0.30f)
        };

        float strokeThick = s * 0.05f;
        for (int i = 0; i < verts.Length; i++)
        {
            var p1 = verts[i];
            var p2 = verts[(i + 1) % verts.Length];
            DrawLine(px, s, p1.x, p1.y, p2.x, p2.y, strokeThick, white);
        }

        float holeX = s * 0.70f;
        float holeY = s * 0.50f;
        float holeR = s * 0.04f;
        DrawCircleFilled(px, s, holeX, holeY, holeR, white);

        float dotCx = s * 0.30f;
        float dotCy = s * 0.50f;
        float dotR = s * 0.055f;
        DrawCircleFilled(px, s, dotCx, dotCy, dotR, white);
        DrawCircleFilled(px, s, dotCx + s * 0.16f, dotCy, dotR, white);
        DrawCircleFilled(px, s, dotCx + s * 0.32f, dotCy, dotR * 0.9f, white);

        return Downsample(px, s);
    }

    private static Color32[] NewBuffer(int s)
    {
        var px = new Color32[s * s];
        for (int i = 0; i < px.Length; i++) px[i] = new Color32(0, 0, 0, 0);
        return px;
    }

    private static void DrawRectFilled(Color32[] px, int w, float x, float y, float rw, float rh, Color32 color)
    {
        int xi0 = Mathf.Max(0, Mathf.FloorToInt(x));
        int yi0 = Mathf.Max(0, Mathf.FloorToInt(y));
        int xi1 = Mathf.Min(w - 1, Mathf.CeilToInt(x + rw));
        int yi1 = Mathf.Min(w - 1, Mathf.CeilToInt(y + rh));
        for (int j = yi0; j <= yi1; j++)
            for (int i = xi0; i <= xi1; i++)
                px[j * w + i] = color;
    }

    private static void DrawCircleFilled(Color32[] px, int w, float cx, float cy, float r, Color32 color)
    {
        int xi0 = Mathf.Max(0, Mathf.FloorToInt(cx - r));
        int yi0 = Mathf.Max(0, Mathf.FloorToInt(cy - r));
        int xi1 = Mathf.Min(w - 1, Mathf.CeilToInt(cx + r));
        int yi1 = Mathf.Min(w - 1, Mathf.CeilToInt(cy + r));
        float r2 = r * r;
        for (int j = yi0; j <= yi1; j++)
        {
            for (int i = xi0; i <= xi1; i++)
            {
                float dx = i + 0.5f - cx;
                float dy = j + 0.5f - cy;
                if (dx * dx + dy * dy <= r2) px[j * w + i] = color;
            }
        }
    }

    private static void DrawLine(Color32[] px, int w, float x0, float y0, float x1, float y1, float thickness, Color32 color)
    {
        float half = thickness * 0.5f + 1f;
        int xi0 = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(x0, x1) - half));
        int xi1 = Mathf.Min(w - 1, Mathf.CeilToInt(Mathf.Max(x0, x1) + half));
        int yi0 = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(y0, y1) - half));
        int yi1 = Mathf.Min(w - 1, Mathf.CeilToInt(Mathf.Max(y0, y1) + half));

        Vector2 a = new Vector2(x0, y0);
        Vector2 b = new Vector2(x1, y1);
        Vector2 ab = b - a;
        float abLen2 = Mathf.Max(0.0001f, ab.sqrMagnitude);
        float halfThick = thickness * 0.5f;

        for (int j = yi0; j <= yi1; j++)
        {
            for (int i = xi0; i <= xi1; i++)
            {
                Vector2 p = new Vector2(i + 0.5f, j + 0.5f);
                float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / abLen2);
                Vector2 closest = a + ab * t;
                float d = Vector2.Distance(p, closest);
                if (d <= halfThick) px[j * w + i] = color;
            }
        }
    }

    private static void DrawTriangleFilled(Color32[] px, int w, Vector2 a, Vector2 b, Vector2 c, Color32 color)
    {
        float minX = Mathf.Min(a.x, Mathf.Min(b.x, c.x));
        float maxX = Mathf.Max(a.x, Mathf.Max(b.x, c.x));
        float minY = Mathf.Min(a.y, Mathf.Min(b.y, c.y));
        float maxY = Mathf.Max(a.y, Mathf.Max(b.y, c.y));
        int xi0 = Mathf.Max(0, Mathf.FloorToInt(minX));
        int yi0 = Mathf.Max(0, Mathf.FloorToInt(minY));
        int xi1 = Mathf.Min(w - 1, Mathf.CeilToInt(maxX));
        int yi1 = Mathf.Min(w - 1, Mathf.CeilToInt(maxY));

        for (int j = yi0; j <= yi1; j++)
        {
            for (int i = xi0; i <= xi1; i++)
            {
                Vector2 p = new Vector2(i + 0.5f, j + 0.5f);
                if (PointInTriangle(p, a, b, c)) px[j * w + i] = color;
            }
        }
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = Sign(p, a, b);
        float d2 = Sign(p, b, c);
        float d3 = Sign(p, c, a);
        bool hasNeg = d1 < 0f || d2 < 0f || d3 < 0f;
        bool hasPos = d1 > 0f || d2 > 0f || d3 > 0f;
        return !(hasNeg && hasPos);
    }

    private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    private static Color32[] Downsample(Color32[] src, int srcSize)
    {
        int dstSize = srcSize / SUPERSAMPLE;
        var dst = new Color32[dstSize * dstSize];
        int k2 = SUPERSAMPLE * SUPERSAMPLE;
        for (int dy = 0; dy < dstSize; dy++)
        {
            for (int dx = 0; dx < dstSize; dx++)
            {
                int sx = dx * SUPERSAMPLE;
                int sy = dy * SUPERSAMPLE;
                int r = 0, g = 0, b = 0, a = 0;
                for (int j = 0; j < SUPERSAMPLE; j++)
                {
                    for (int i = 0; i < SUPERSAMPLE; i++)
                    {
                        var c = src[(sy + j) * srcSize + (sx + i)];
                        r += c.r; g += c.g; b += c.b; a += c.a;
                    }
                }
                dst[dy * dstSize + dx] = new Color32((byte)(r / k2), (byte)(g / k2), (byte)(b / k2), (byte)(a / k2));
            }
        }
        return dst;
    }
}
