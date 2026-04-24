using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaceholderPopup : PopupBase
{
    public Button okButton;
    public TextMeshProUGUI modeText;

    private void Start()
    {
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(Hide);
    }

    private void OnEnable()
    {
        if (modeText != null)
        {
            modeText.text = "Selected mode: <b>" + GameMode.Current.ToString().ToUpper() + "</b>\n\nGame scene will be available\nin the next iteration.";
        }
    }
}
