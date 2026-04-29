using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EdgeHighlighter : MonoBehaviour
{
    public HexGrid grid;
    public Sprite hexSprite;
    public Color highlightColor = new Color(0.31f, 0.80f, 0.51f, 0.5f);
    public float pulseAlphaMin = 0.2f;
    public float pulseAlphaMax = 0.55f;
    public float pulseDuration = 1.4f;
    public float scaleMultiplier = 1.18f;

    private readonly List<Image> highlights = new List<Image>();
    private bool active;

    public void Activate()
    {
        if (active) return;
        Debug.Assert(grid != null, "EdgeHighlighter: grid not assigned!");
        Debug.Assert(hexSprite != null, "EdgeHighlighter: hexSprite not assigned!");

        int r = grid.gridRadius;
        foreach (var pair in grid.Cells)
        {
            var coord = pair.Key;
            if (!IsEdge(coord, r)) continue;

            var cell = pair.Value;
            var cellRT = cell.GetComponent<RectTransform>();
            var go = new GameObject("EdgeGlow", typeof(RectTransform));
            go.transform.SetParent(cellRT, false);
            go.transform.SetAsFirstSibling();
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = cellRT.sizeDelta * scaleMultiplier;
            rt.anchoredPosition = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.sprite = hexSprite;
            img.color = highlightColor;
            img.raycastTarget = false;
            highlights.Add(img);

            img.DOFade(pulseAlphaMin, pulseDuration)
                .From(pulseAlphaMax)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        active = true;
    }

    private static bool IsEdge(HexCoord c, int radius)
    {
        int aq = Mathf.Abs(c.q);
        int ar = Mathf.Abs(c.r);
        int aqr = Mathf.Abs(c.q + c.r);
        int max = aq;
        if (ar > max) max = ar;
        if (aqr > max) max = aqr;
        return max == radius;
    }
}
