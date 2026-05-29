using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SetupMinWordLengthPatch
{
    private const string GAME_SCENE_PATH = "Assets/WordGame/Scenes/Game.unity";

    [MenuItem("WordGame/Patch Min Word Length to 3")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit play mode before running this patch.");
            return;
        }

        if (!File.Exists(GAME_SCENE_PATH))
        {
            Debug.LogError("Game.unity not found.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(GAME_SCENE_PATH, OpenSceneMode.Single);

        var canvasGO = GameObject.Find("/Canvas");
        if (canvasGO == null) { Debug.LogError("Canvas not found."); return; }

        var controllerTR = canvasGO.transform.Find("GameController");
        if (controllerTR == null) { Debug.LogError("GameController not found."); return; }

        var validator = controllerTR.GetComponent<WordValidator>();
        if (validator == null) { Debug.LogError("WordValidator missing on GameController."); return; }

        int oldValue = validator.minWordLength;
        validator.minWordLength = 3;
        EditorUtility.SetDirty(validator);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("<color=#4FCC81><b>WordGame patch:</b> WordValidator.minWordLength " + oldValue + " → 3.</color>\n" +
                  "  Placement pool now also filters words >= 3 letters at load time.\n" +
                  "  Too-short releases on touch are now silently discarded (no error flash).");
    }
}
