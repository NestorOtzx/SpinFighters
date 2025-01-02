using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SquarePulse : MonoBehaviour
{
    private AudioSource audioSource; 
    public float sensitivity = 1.0f; 
    public float smoothness = 0.5f; 
    public float minScale = 0.8f; 
    public float maxScale = 1.2f; 

    private RectTransform rectTransform; 
    private Vector2 initialSize; 
    private float[] spectrum = new float[256]; 

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialSize = rectTransform.sizeDelta;

        audioSource = AudioManager.instance.backgroundAudioSource;
    }

    void Update()
    {
        if (audioSource != null)
        {
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

            float intensity = 0;
            for (int i = 0; i < spectrum.Length; i++)
            {
                intensity += spectrum[i];
            }
            intensity /= spectrum.Length;

            float scale = Mathf.Lerp(minScale, maxScale, intensity * sensitivity);
            scale = Mathf.Clamp(scale, minScale, maxScale);
            rectTransform.sizeDelta = initialSize * scale;
        }
    }
}