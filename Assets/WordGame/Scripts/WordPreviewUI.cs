using UnityEngine;
using TMPro;
using DG.Tweening;

public class WordPreviewUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public Color normalColor = new Color(0.91f, 0.65f, 0.27f, 1f);
    public Color successColor = new Color(0.31f, 0.80f, 0.51f, 1f);
    public Color errorColor = new Color(0.92f, 0.36f, 0.36f, 1f);

    public float flashDuration = 0.7f;

    private Sequence flashSeq;

    public void SetWord(string word)
    {
        if (text == null) return;
        if (flashSeq != null && flashSeq.IsActive()) flashSeq.Kill();
        text.color = normalColor;
        text.alpha = 1f;
        text.text = word;
    }

    public void FlashSuccess(string message)
    {
        Flash(message, successColor);
    }

    public void FlashError(string message)
    {
        Flash(message, errorColor);
    }

    private void Flash(string message, Color color)
    {
        if (text == null) return;
        if (flashSeq != null && flashSeq.IsActive()) flashSeq.Kill();

        text.text = message;
        text.color = color;
        text.alpha = 1f;
        text.transform.localScale = Vector3.one;

        flashSeq = DOTween.Sequence();
        flashSeq.Append(text.transform.DOScale(1.15f, 0.12f).SetEase(Ease.OutQuad));
        flashSeq.Append(text.transform.DOScale(1f, 0.12f).SetEase(Ease.InQuad));
        flashSeq.AppendInterval(flashDuration - 0.24f);
        flashSeq.Append(text.DOFade(0f, 0.2f));
        flashSeq.OnComplete(() =>
        {
            text.text = "";
            text.alpha = 1f;
            text.color = normalColor;
        });
    }
}
