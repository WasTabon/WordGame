using UnityEngine;
using TMPro;

public class WordPreviewUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void SetWord(string word)
    {
        if (text != null) text.text = word;
    }
}
