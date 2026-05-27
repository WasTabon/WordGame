using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HintHighlighter : MonoBehaviour
{
    public Color glowColor = new Color(0.31f, 0.80f, 0.51f, 1f);
    public float perCellDelay = 0.18f;
    public float fadeInDuration = 0.22f;
    public float holdDuration = 3f;
    public float fadeOutDuration = 0.45f;
    public float glowMaxAlpha = 0.7f;
    public float glowPadding = 12f;

    private readonly List<Image> activeOverlays = new List<Image>();
    private Sequence currentSeq;

    public void Highlight(List<HexCell> path)
    {
        if (path == null || path.Count == 0) return;

        ClearOverlays();
        if (currentSeq != null) currentSeq.Kill();

        for (int i = 0; i < path.Count; i++)
        {
            var overlay = CreateOverlay(path[i]);
            activeOverlays.Add(overlay);
        }

        currentSeq = DOTween.Sequence();

        for (int i = 0; i < activeOverlays.Count; i++)
        {
            var img = activeOverlays[i];
            var rt = img.GetComponent<RectTransform>();
            float delay = i * perCellDelay;
            currentSeq.Insert(delay, img.DOFade(glowMaxAlpha, fadeInDuration));
            currentSeq.Insert(delay, rt.DOScale(1f, fadeInDuration + 0.05f).From(0.55f).SetEase(Ease.OutBack));
        }

        float lastFadeInEnd = (activeOverlays.Count - 1) * perCellDelay + fadeInDuration;
        currentSeq.AppendInterval(Mathf.Max(0f, holdDuration + lastFadeInEnd - currentSeq.Duration()));

        for (int i = 0; i < activeOverlays.Count; i++)
        {
            currentSeq.Join(activeOverlays[i].DOFade(0f, fadeOutDuration));
        }

        currentSeq.OnComplete(ClearOverlays);
    }

    private Image CreateOverlay(HexCell cell)
    {
        var go = new GameObject("HintGlow", typeof(RectTransform));
        go.transform.SetParent(cell.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(-glowPadding, -glowPadding);
        rt.offsetMax = new Vector2(glowPadding, glowPadding);
        rt.localScale = Vector3.one * 0.55f;

        var img = go.AddComponent<Image>();
        if (cell.hexImage != null) img.sprite = cell.hexImage.sprite;
        img.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
        img.raycastTarget = false;
        go.transform.SetAsFirstSibling();
        return img;
    }

    private void ClearOverlays()
    {
        for (int i = 0; i < activeOverlays.Count; i++)
        {
            if (activeOverlays[i] != null) Destroy(activeOverlays[i].gameObject);
        }
        activeOverlays.Clear();
    }
}
