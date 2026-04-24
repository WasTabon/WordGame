using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexCell : MonoBehaviour
{
    public Image hexImage;
    public TextMeshProUGUI letterText;
    public TextMeshProUGUI numberText;

    public HexCoord Coord { get; private set; }
    public char Letter { get; private set; }
    public bool IsVacant { get; private set; }
    public int MinWordLength { get; private set; }

    private static readonly Color ActiveColor = new Color(0.29f, 0.33f, 0.41f, 1f);
    private static readonly Color VacantColor = new Color(0.29f, 0.33f, 0.41f, 0.14f);

    public void Setup(HexCoord coord, char letter)
    {
        Coord = coord;
        Letter = letter;
        IsVacant = false;
        MinWordLength = 0;
        Refresh();
    }

    public void SetVacant(bool vacant)
    {
        IsVacant = vacant;
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

    private void Refresh()
    {
        if (hexImage != null)
        {
            hexImage.color = IsVacant ? VacantColor : ActiveColor;
            hexImage.raycastTarget = !IsVacant;
        }
        if (letterText != null)
        {
            letterText.text = IsVacant ? "" : Letter.ToString();
        }
        if (numberText != null)
        {
            bool show = !IsVacant && MinWordLength > 0;
            numberText.gameObject.SetActive(show);
            if (show) numberText.text = MinWordLength.ToString();
        }
    }
}
