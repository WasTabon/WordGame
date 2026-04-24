using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PopupBase : MonoBehaviour
{
    public RectTransform panel;
    public float fadeInDuration = 0.22f;
    public float fadeOutDuration = 0.18f;
    public float scaleStart = 0.85f;

    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Debug.Assert(panel != null, name + ": panel not assigned!");
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);

        panel.DOKill();
        panel.localScale = Vector3.one * scaleStart;
        panel.DOScale(Vector3.one, fadeInDuration + 0.08f).SetEase(Ease.OutBack);

        if (SoundManager.Instance != null) SoundManager.Instance.PlayPopupOpen();
    }

    public virtual void Hide()
    {
        canvasGroup.DOKill();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad);

        panel.DOKill();
        panel.DOScale(Vector3.one * scaleStart, fadeOutDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => gameObject.SetActive(false));

        if (SoundManager.Instance != null) SoundManager.Instance.PlayPopupClose();
    }
}
