using UnityEngine;
using UnityEngine.UI;

public class ModeSelectPopup : PopupBase
{
    public Button escapeButton;
    public Button exploreButton;
    public Button closeButton;
    public Button blockerButton;

    public PlaceholderPopup placeholderPopup;

    private void Start()
    {
        Debug.Assert(escapeButton != null, "ModeSelectPopup: escapeButton missing!");
        Debug.Assert(exploreButton != null, "ModeSelectPopup: exploreButton missing!");
        Debug.Assert(placeholderPopup != null, "ModeSelectPopup: placeholderPopup reference missing!");

        escapeButton.onClick.RemoveAllListeners();
        escapeButton.onClick.AddListener(OnEscapeSelected);

        exploreButton.onClick.RemoveAllListeners();
        exploreButton.onClick.AddListener(OnExploreSelected);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Hide);

        if (blockerButton != null)
        {
            blockerButton.onClick.RemoveAllListeners();
            blockerButton.onClick.AddListener(Hide);
        }
    }

    private void OnEscapeSelected()
    {
        GameMode.SetMode(GameMode.Mode.Escape);
        Hide();
        placeholderPopup.Show();
    }

    private void OnExploreSelected()
    {
        GameMode.SetMode(GameMode.Mode.Explore);
        Hide();
        placeholderPopup.Show();
    }
}
