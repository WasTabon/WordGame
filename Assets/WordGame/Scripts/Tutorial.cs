using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Tutorial : MonoBehaviour, IPointerClickHandler
{
    public CanvasGroup canvasGroup;
    public Image dimImage;
    public RectTransform calloutPanel;
    public TextMeshProUGUI calloutText;
    public TextMeshProUGUI hintText;

    public HexGrid grid;
    public RectTransform previewRT;
    public RectTransform timerHudRT;

    private const string KEY_EXPLORE = "WG_TutorialDone_Explore";
    private const string KEY_ESCAPE = "WG_TutorialDone_Escape";

    private struct Step
    {
        public string text;
        public float calloutY;
    }

    private List<Step> steps;
    private int currentStep;
    private bool active;

    public static bool IsTutorialDone(GameMode.Mode mode)
    {
        string key = mode == GameMode.Mode.Escape ? KEY_ESCAPE : KEY_EXPLORE;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(KEY_EXPLORE);
        PlayerPrefs.DeleteKey(KEY_ESCAPE);
        PlayerPrefs.Save();
    }

    private static void MarkDone(GameMode.Mode mode)
    {
        string key = mode == GameMode.Mode.Escape ? KEY_ESCAPE : KEY_EXPLORE;
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }

    public void TryShow()
    {
        if (IsTutorialDone(GameMode.Current))
        {
            gameObject.SetActive(false);
            return;
        }

        BuildSteps();
        currentStep = 0;
        active = true;
        gameObject.SetActive(true);
        ShowStep(0);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.3f);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
    }

    private void BuildSteps()
    {
        steps = new List<Step>();
        steps.Add(new Step { text = "Welcome!\n\nThis is your hex grid.\nFind words hidden inside.", calloutY = 0f });
        steps.Add(new Step { text = "Swipe across adjacent letters\nto build a word.\n\nStart on a cell next to\nan empty (transparent) one.", calloutY = -550f });
        steps.Add(new Step { text = "Words must be real English\nwords from the dictionary.\n\nValid words turn into empty\ncells - the board grows.", calloutY = 600f });

        bool hasNumberCell = false;
        if (grid != null)
        {
            foreach (var pair in grid.Cells)
            {
                if (pair.Value.MinWordLength > 0) { hasNumberCell = true; break; }
            }
        }
        if (hasNumberCell)
        {
            steps.Add(new Step { text = "Cells with numbers require\nwords of at least that length.\n\nStarting on a number cell\ngives you a x2 score bonus!", calloutY = -550f });
        }

        if (GameMode.Current == GameMode.Mode.Escape)
        {
            steps.Add(new Step { text = "ESCAPE MODE\n\nReach a glowing edge cell\nbefore the timer runs out.\n\nLong words give you\nmore time bonus.", calloutY = 600f });
        }
    }

    private void ShowStep(int idx)
    {
        if (steps == null || idx < 0 || idx >= steps.Count) return;
        var step = steps[idx];
        if (calloutText != null) calloutText.text = step.text;
        if (calloutPanel != null)
        {
            var pos = calloutPanel.anchoredPosition;
            pos.y = step.calloutY;
            calloutPanel.anchoredPosition = pos;
            calloutPanel.localScale = Vector3.one * 0.85f;
            calloutPanel.DOKill();
            calloutPanel.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        }
        if (hintText != null)
        {
            bool isLast = idx == steps.Count - 1;
            hintText.text = isLast ? "Tap to start" : "Tap to continue (" + (idx + 1) + "/" + steps.Count + ")";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!active) return;
        currentStep++;
        if (currentStep >= steps.Count)
        {
            Finish();
        }
        else
        {
            ShowStep(currentStep);
        }
    }

    private void Finish()
    {
        active = false;
        MarkDone(GameMode.Current);
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOFade(0f, 0.3f).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
