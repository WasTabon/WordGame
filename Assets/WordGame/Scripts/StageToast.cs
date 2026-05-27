using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StageToast : MonoBehaviour
{
    public RectTransform root;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public CanvasGroup canvasGroup;

    public float scaleUpDuration = 0.35f;
    public float holdDuration = 1.0f;
    public float fadeOutDuration = 0.45f;

    private Sequence currentSeq;

    private void Start()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (root != null) root.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void ShowStage(int stage)
    {
        if (root == null || canvasGroup == null) return;

        gameObject.SetActive(true);

        if (titleText != null) titleText.text = "STAGE " + stage;
        if (subtitleText != null) subtitleText.text = "Board cleared! New stage starting...";

        if (currentSeq != null) currentSeq.Kill();
        root.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;

        currentSeq = DOTween.Sequence();
        currentSeq.Append(root.DOScale(1.1f, scaleUpDuration).SetEase(Ease.OutBack));
        currentSeq.Join(canvasGroup.DOFade(1f, scaleUpDuration * 0.6f));
        currentSeq.Append(root.DOScale(1f, 0.15f));
        currentSeq.AppendInterval(holdDuration);
        currentSeq.Append(canvasGroup.DOFade(0f, fadeOutDuration));
        currentSeq.Join(root.DOScale(1.15f, fadeOutDuration));
        currentSeq.OnComplete(() => gameObject.SetActive(false));
    }
}
