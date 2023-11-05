using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MicAtt
{
    public int maxSample_Editor;
    public float loudnessPow_Editor;

    public int maxSamples_VR;
    public float loudnessPow_VR;

    [HideInInspector]
    public bool isLoud;
    [HideInInspector]
    public float volume;

    private List<float> samples;

    public void Update()
    {
        if (maxSample_Editor == 0) maxSample_Editor = 30;
        if (loudnessPow_Editor < 0.00001f) loudnessPow_Editor = 0.75f;
        if (maxSamples_VR == 0) maxSamples_VR = 30;
        if (loudnessPow_VR < 0.00001f) loudnessPow_VR = 0.35f;
        if (samples == null) samples = new List<float>();

        int maxSamples = GameManager.isVR ? maxSamples_VR : maxSample_Editor;
        float loudnessPow = GameManager.isVR ? loudnessPow_VR : loudnessPow_Editor;

        float loudness = GameManager.micLoudness;
        while (samples.Count >= maxSamples)
        {
            samples.RemoveAt(0);
        }
        samples.Add(Mathf.Pow(loudness, loudnessPow));

        float avg = 0.0f;
        foreach (var sample in samples)
        {
            avg += sample;
        }
        avg /= samples.Count;

        isLoud = avg > 0.15f;
        volume = Mathf.Clamp(avg, 0.01f, 1.0f);
    }
}
