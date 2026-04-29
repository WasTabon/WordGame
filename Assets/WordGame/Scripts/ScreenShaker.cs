using UnityEngine;
using DG.Tweening;

public class ScreenShaker : MonoBehaviour
{
    public static ScreenShaker Instance { get; private set; }

    public RectTransform target;

    private Vector2 originalPos;
    private bool initialized;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Shake(float strength = 30f, float duration = 0.3f)
    {
        if (target == null) return;
        if (!initialized)
        {
            originalPos = target.anchoredPosition;
            initialized = true;
        }
        target.DOKill();
        target.anchoredPosition = originalPos;
        target.DOShakeAnchorPos(duration, strength, 18, 90f, false, true)
            .OnComplete(() => target.anchoredPosition = originalPos);
    }
}
