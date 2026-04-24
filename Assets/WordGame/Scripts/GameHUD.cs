using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour
{
    public Button backButton;
    public TextMeshProUGUI modeLabel;

    private void Start()
    {
        Debug.Assert(backButton != null, "GameHUD: backButton missing!");
        Debug.Assert(modeLabel != null, "GameHUD: modeLabel missing!");

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBack);

        modeLabel.text = GameMode.Current == GameMode.Mode.Escape ? "ESCAPE" : "EXPLORE";
    }

    private void OnBack()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
