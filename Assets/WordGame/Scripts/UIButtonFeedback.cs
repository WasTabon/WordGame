using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class UIButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float pressedScale = 0.92f;
    public float pressDuration = 0.08f;
    public float releaseDuration = 0.12f;

    private Button button;
    private Vector3 originalScale;
    private bool pressed;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
        if (originalScale == Vector3.zero) originalScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        pressed = true;
        transform.DOKill();
        transform.DOScale(originalScale * pressedScale, pressDuration).SetEase(Ease.OutQuad);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!pressed) return;
        pressed = false;
        transform.DOKill();
        transform.DOScale(originalScale, releaseDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!pressed) return;
        pressed = false;
        transform.DOKill();
        transform.DOScale(originalScale, releaseDuration).SetEase(Ease.OutQuad);
    }
}
