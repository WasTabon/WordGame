using System.IO;
using UnityEditor;
using UnityEngine;

public static class SetupBigDictionaryIteration11
{
    private const string RESOURCES_DIR = "Assets/WordGame/Resources";
    private const string WORDS_FILE = "Assets/WordGame/Resources/words.txt";

    [MenuItem("WordGame/Setup Big Dictionary (Iteration 11)")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this setup.");
            return;
        }

        if (!AssetDatabase.IsValidFolder("Assets/WordGame"))
        {
            Debug.LogError("Assets/WordGame folder not found. Run iterations 1-10 first.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(RESOURCES_DIR))
        {
            AssetDatabase.CreateFolder("Assets/WordGame", "Resources");
            Debug.Log("Created Resources folder.");
        }

        if (!File.Exists(WORDS_FILE))
        {
            Debug.LogError(
                "<color=#E84545><b>words.txt NOT FOUND</b></color> at " + WORDS_FILE + "\n" +
                "Please put your words.txt file (one word per line, e.g. 466k English words) into:\n" +
                "  " + RESOURCES_DIR + "/\n" +
                "Then re-run this setup. Without words.txt, validation will fall back to the small 401-word placement pool.\n\n" +
                "Recommended source: https://github.com/dwyl/english-words/blob/master/words_alpha.txt"
            );
            return;
        }

        string content = File.ReadAllText(WORDS_FILE);
        var lines = content.Split('\n');
        int validCount = 0;
        int withBadCharsCount = 0;
        int tooShortCount = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string w = lines[i].Trim();
            if (w.Length < 2)
            {
                if (w.Length > 0) tooShortCount++;
                continue;
            }

            bool ok = true;
            for (int c = 0; c < w.Length; c++)
            {
                char ch = char.ToUpperInvariant(w[c]);
                if (ch < 'A' || ch > 'Z') { ok = false; break; }
            }
            if (ok) validCount++;
            else withBadCharsCount++;
        }

        long sizeKb = new FileInfo(WORDS_FILE).Length / 1024;

        Debug.Log(
            "<color=#4FCC81><b>WordGame Iteration 11:</b> Big Dictionary check passed.</color>\n" +
            "  File: " + WORDS_FILE + " (" + sizeKb + " KB)\n" +
            "  Valid words: <b>" + validCount.ToString("N0") + "</b>\n" +
            (tooShortCount > 0 ? "  Skipped (too short): " + tooShortCount + "\n" : "") +
            (withBadCharsCount > 0 ? "  <color=#E8A745>Skipped (non A-Z chars): " + withBadCharsCount + "</color>\n" : "") +
            "  Estimated RAM: ~" + ((validCount * 60) / (1024 * 1024)) + " MB\n" +
            "  Placement pool (for board generation): unchanged, 401 small words.\n" +
            "  Run the game - on MainMenu.Start dictionary preloads on background coroutine."
        );

        AssetDatabase.ImportAsset(WORDS_FILE);
    }
}
