using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HexCell : MonoBehaviour, IPointerDownHandler
{
    public Image hexImage;
    public TextMeshProUGUI letterText;
    public TextMeshProUGUI numberText;

    public HexCoord Coord { get; private set; }
    public char Letter { get; private set; }
    public bool IsVacant { get; private set; }
    public int MinWordLength { get; private set; }
    public bool IsSelected { get; private set; }

    private static readonly Color ActiveColor = new Color(0.29f, 0.33f, 0.41f, 1f);
    private static readonly Color VacantColor = new Color(0.29f, 0.33f, 0.41f, 0.14f);
    private static readonly Color SelectedColor = new Color(0.91f, 0.65f, 0.27f, 1f);
    private static readonly Color LetterColorDefault = Color.white;
    private static readonly Color LetterColorOnSelected = new Color(0.1f, 0.14f, 0.2f, 1f);

    public void Setup(HexCoord coord, char letter)
    {
        Coord = coord;
        Letter = letter;
        IsVacant = false;
        MinWordLength = 0;
        IsSelected = false;
        Refresh();
    }

    public void SetVacant(bool vacant)
    {
        bool wasVacant = IsVacant;
        IsVacant = vacant;
        if (vacant) IsSelected = false;
        Refresh();
        if (vacant && !wasVacant) AnimateVacantTransition();
    }

    public void SetLetter(char letter)
    {
        Letter = letter;
        Refresh();
    }

    public void SetMinWordLength(int min)
    {
        MinWordLength = min;
        Refresh();
    }

    public void SetSelected(bool sel)
    {
        if (IsVacant) return;
        bool wasSelected = IsSelected;
        IsSelected = sel;
        Refresh();
        if (sel && !wasSelected) AnimateSelectPop();
    }

    private void AnimateSelectPop()
    {
        if (letterText == null) return;
        var t = letterText.transform;
        t.DOKill();
        t.localScale = Vector3.one;
        t.DOPunchScale(Vector3.one * 0.3f, 0.25f, 4, 0.6f);
    }

    private void AnimateVacantTransition()
    {
        if (hexImage == null) return;
        var t = hexImage.transform;
        t.DOKill();

        var startScale = t.localScale;
        var seq = DOTween.Sequence();
        seq.Append(t.DOScale(startScale * 1.2f, 0.15f).SetEase(Ease.OutQuad));
        seq.Append(t.DOScale(startScale, 0.25f).SetEase(Ease.OutBack));

        if (letterText != null)
        {
            letterText.DOKill();
            letterText.alpha = 0f;
            letterText.DOFade(1f, 0.15f);
        }

        if (SoundManager.Instance != null) SoundManager.Instance.PlayVacantPop();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsVacant) return;
        if (WordBuilder.Instance == null) return;
        WordBuilder.Instance.TryStartWord(this);
    }

    private void Refresh()
    {
        if (hexImage != null)
        {
            Color target;
            if (IsVacant) target = VacantColor;
            else if (IsSelected) target = SelectedColor;
            else target = ActiveColor;
            hexImage.color = target;
            hexImage.raycastTarget = !IsVacant;
        }
        if (letterText != null)
        {
            letterText.text = IsVacant ? "" : Letter.ToString();
            letterText.color = IsSelected ? LetterColorOnSelected : LetterColorDefault;
        }
        if (numberText != null)
        {
            bool show = !IsVacant && MinWordLength > 0;
            numberText.gameObject.SetActive(show);
            if (show) numberText.text = MinWordLength.ToString();
        }
    }
}
