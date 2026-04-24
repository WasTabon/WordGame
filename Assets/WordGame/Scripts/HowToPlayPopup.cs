using UnityEngine;
using UnityEngine.UI;

public class HowToPlayPopup : PopupBase
{
    public Button closeButton;
    public Button blockerButton;

    private void Start()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);

        if (blockerButton != null)
        {
            blockerButton.onClick.RemoveAllListeners();
            blockerButton.onClick.AddListener(Hide);
        }
    }
}
