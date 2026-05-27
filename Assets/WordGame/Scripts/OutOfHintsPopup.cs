using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OutOfHintsPopup : PopupBase
{
    public Button closeButton;
    public Button blockerButton;
    public Button buyButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI buyButtonLabel;

    private void Start()
    {
        Debug.Assert(closeButton != null, "OutOfHintsPopup: closeButton missing!");

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);

        if (blockerButton != null)
        {
            blockerButton.onClick.RemoveAllListeners();
            blockerButton.onClick.AddListener(Hide);
        }
    }
}
