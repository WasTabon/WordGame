using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingScorePopup : MonoBehaviour
{
    public static void Spawn(Transform parent, Vector2 anchoredPos, string text, Color color, float fontSize = 90f)
    {
        var go = new GameObject("FloatingScore", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(400, 120);
        rt.anchoredPosition = anchoredPos;
        rt.localScale = Vector3.one * 0.5f;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = false;

        var seq = DOTween.Sequence();
        seq.Append(rt.DOScale(1.1f, 0.15f).SetEase(Ease.OutBack));
        seq.Append(rt.DOAnchorPosY(anchoredPos.y + 220f, 1.0f).SetEase(Ease.OutCubic));
        seq.Insert(0.5f, tmp.DOFade(0f, 0.6f));
        seq.OnComplete(() => Destroy(go));
    }
}
