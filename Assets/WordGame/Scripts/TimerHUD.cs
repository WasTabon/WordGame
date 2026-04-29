using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TimerHUD : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public Image background;
    public EscapeTimer timer;

    public Color normalColor = new Color(0.91f, 0.65f, 0.27f, 1f);
    public Color warningColor = new Color(0.92f, 0.36f, 0.36f, 1f);
    public float warningThreshold = 10f;

    private bool isWarningPulse;

    private void OnEnable()
    {
        if (timer != null)
        {
            timer.OnTick -= HandleTick;
            timer.OnTick += HandleTick;
        }
    }

    private void OnDisable()
    {
        if (timer != null) timer.OnTick -= HandleTick;
        if (timeText != null) timeText.transform.DOKill();
    }

    private void HandleTick(float timeLeft)
    {
        if (timeText == null) return;

        int total = Mathf.CeilToInt(timeLeft);
        int m = total / 60;
        int s = total % 60;
        timeText.text = string.Format("{0}:{1:D2}", m, s);

        bool warning = timeLeft <= warningThreshold && timeLeft > 0f;
        timeText.color = warning ? warningColor : normalColor;

        if (warning && !isWarningPulse)
        {
            isWarningPulse = true;
            timeText.transform.DOKill();
            timeText.transform.localScale = Vector3.one;
            timeText.transform.DOScale(1.15f, 0.4f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
        else if (!warning && isWarningPulse)
        {
            isWarningPulse = false;
            timeText.transform.DOKill();
            timeText.transform.localScale = Vector3.one;
        }
    }
}
