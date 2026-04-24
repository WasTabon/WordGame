using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public Image fadeImage;
    public float fadeDuration = 0.35f;

    private bool isLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Assert(fadeImage != null, "SceneLoader: fadeImage not assigned!");

        var c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        fadeImage.raycastTarget = false;
    }

    public void LoadScene(string sceneName)
    {
        if (isLoading)
        {
            Debug.LogWarning("SceneLoader: already loading a scene.");
            return;
        }
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;
        fadeImage.raycastTarget = true;
        yield return fadeImage.DOFade(1f, fadeDuration).WaitForCompletion();
        SceneManager.LoadScene(sceneName);
        yield return null;
        yield return fadeImage.DOFade(0f, fadeDuration).WaitForCompletion();
        fadeImage.raycastTarget = false;
        isLoading = false;
    }
}
