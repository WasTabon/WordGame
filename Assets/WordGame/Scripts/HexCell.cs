using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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
        IsVacant = vacant;
        if (vacant) IsSelected = false;
        Refresh();
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
        IsSelected = sel;
        Refresh();
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
