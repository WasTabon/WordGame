using UnityEngine;

public static class ProceduralSounds
{
    private const int SampleRate = 44100;

    public static AudioClip Click()
    {
        return GenerateTone("click", 0.05f, t => Mathf.Sin(2f * Mathf.PI * 800f * t) * Envelope(t, 0.05f, 0.005f, 0.02f));
    }

    public static AudioClip SelectStart()
    {
        return GenerateTone("select_start", 0.08f, t =>
        {
            float freq = 440f + 200f * t / 0.08f;
            return Mathf.Sin(2f * Mathf.PI * freq * t) * Envelope(t, 0.08f, 0.005f, 0.04f);
        });
    }

    public static AudioClip SelectAdd(int chainIndex)
    {
        float baseFreq = 440f;
        float step = 1.06f;
        float freq = baseFreq * Mathf.Pow(step, Mathf.Clamp(chainIndex, 0, 12));
        return GenerateTone("select_add_" + chainIndex, 0.06f, t =>
            Mathf.Sin(2f * Mathf.PI * freq * t) * Envelope(t, 0.06f, 0.003f, 0.03f));
    }

    public static AudioClip Success()
    {
        return GenerateTone("success", 0.4f, t =>
        {
            float f1 = 523f;
            float f2 = 659f;
            float f3 = 784f;
            float v = 0f;
            if (t < 0.1f) v = Mathf.Sin(2f * Mathf.PI * f1 * t);
            else if (t < 0.2f) v = Mathf.Sin(2f * Mathf.PI * f2 * t);
            else v = Mathf.Sin(2f * Mathf.PI * f3 * t);
            return v * Envelope(t, 0.4f, 0.01f, 0.2f);
        });
    }

    public static AudioClip Error()
    {
        return GenerateTone("error", 0.25f, t =>
        {
            float freq = 220f - 80f * t / 0.25f;
            float saw = (t * freq) % 1f;
            saw = saw * 2f - 1f;
            return saw * 0.5f * Envelope(t, 0.25f, 0.01f, 0.15f);
        });
    }

    public static AudioClip VacantPop()
    {
        return GenerateTone("vacant_pop", 0.18f, t =>
        {
            float freq = 880f - 400f * t / 0.18f;
            float noise = (Random.value * 2f - 1f) * 0.2f;
            return (Mathf.Sin(2f * Mathf.PI * freq * t) + noise) * Envelope(t, 0.18f, 0.005f, 0.1f);
        });
    }

    public static AudioClip Tick()
    {
        return GenerateTone("tick", 0.04f, t =>
            Mathf.Sin(2f * Mathf.PI * 1200f * t) * Envelope(t, 0.04f, 0.002f, 0.02f));
    }

    public static AudioClip Win()
    {
        return GenerateTone("win", 0.7f, t =>
        {
            float v = 0f;
            if (t < 0.15f) v = Mathf.Sin(2f * Mathf.PI * 523f * t);
            else if (t < 0.30f) v = Mathf.Sin(2f * Mathf.PI * 659f * t);
            else if (t < 0.45f) v = Mathf.Sin(2f * Mathf.PI * 784f * t);
            else v = Mathf.Sin(2f * Mathf.PI * 1047f * t);
            return v * Envelope(t, 0.7f, 0.01f, 0.3f);
        });
    }

    public static AudioClip Lose()
    {
        return GenerateTone("lose", 0.6f, t =>
        {
            float v = 0f;
            if (t < 0.2f) v = Mathf.Sin(2f * Mathf.PI * 392f * t);
            else if (t < 0.4f) v = Mathf.Sin(2f * Mathf.PI * 311f * t);
            else v = Mathf.Sin(2f * Mathf.PI * 233f * t);
            return v * Envelope(t, 0.6f, 0.02f, 0.3f);
        });
    }

    public static AudioClip PopupOpen()
    {
        return GenerateTone("popup_open", 0.15f, t =>
        {
            float freq = 660f + 220f * t / 0.15f;
            return Mathf.Sin(2f * Mathf.PI * freq * t) * Envelope(t, 0.15f, 0.005f, 0.08f);
        });
    }

    public static AudioClip PopupClose()
    {
        return GenerateTone("popup_close", 0.12f, t =>
        {
            float freq = 660f - 220f * t / 0.12f;
            return Mathf.Sin(2f * Mathf.PI * freq * t) * Envelope(t, 0.12f, 0.005f, 0.06f);
        });
    }

    private static AudioClip GenerateTone(string name, float duration, System.Func<float, float> sampleFn)
    {
        int sampleCount = Mathf.CeilToInt(duration * SampleRate);
        var data = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / SampleRate;
            data[i] = Mathf.Clamp(sampleFn(t), -1f, 1f);
        }
        var clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private static float Envelope(float t, float duration, float attack, float release)
    {
        if (t < attack) return t / attack;
        if (t > duration - release) return (duration - t) / release;
        return 1f;
    }
}
