using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio; // Importar el namespace para TMP_Dropdown

public class Settings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;  // TMP_Dropdown para las resoluciones
    public Toggle vSyncToggle;              // Toggle para Vertical Sync
    public Toggle fullscreenToggle;         // Toggle para pantalla completa
    public AudioMixer audioMixer;           // Referencia al Audio Mixer
    private Resolution[] resolutions;

    public Slider backgroundMusicSlider, sfxSlider;

    [SerializeField] private GameObject viewContainer;

    public void SetViewContainer(bool t)
    {
        viewContainer.SetActive(t);
    }

    public void ToggleViewContainer()
    {
        viewContainer.SetActive(!viewContainer.activeSelf);
    }

    private void Start()
    {
        InitializeResolutions();
        SyncSettingsWithUI();
    }

    #region Pantalla
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    private void SyncFullscreenToggle()
    {
        // Sincronizar el estado del toggle con la configuración real
        fullscreenToggle.isOn = Screen.fullScreen;

        // Agregar listener para manejar cambios del toggle
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
    }

    public void InitializeResolutions()
    {
        // Obtener todas las resoluciones disponibles
        resolutions = Screen.resolutions;

        // Limpiar opciones existentes en el TMP_Dropdown
        resolutionDropdown.ClearOptions();

        // Crear una lista de opciones para el TMP_Dropdown
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);
        }

        // Agregar las opciones al TMP_Dropdown
        resolutionDropdown.AddOptions(options);

        // Agregar listener al TMP_Dropdown para actualizar la resolución al seleccionarla
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }
    #endregion

    #region Vertical Sync
    public void SetVSync(bool isEnabled)
    {
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isEnabled ? 1 : 0);
    }

    private void SyncVSyncToggle()
    {
        // Sincronizar el estado del toggle con la configuración real
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

        // Agregar listener para manejar cambios del toggle
        vSyncToggle.onValueChanged.AddListener(SetVSync);
    }
    #endregion

    #region Volumen
    public void SetMusicVolume(float volume)
    {
        float val = -80;
        if (volume > 0f)
        {
            val = Mathf.Log(volume, 5);
        }
        Debug.Log("Set volume"+ volume);
        audioMixer.SetFloat("MusicVolume", val * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetEffectsVolume(float volume)
    {
        float val = -80;
        if (volume > 0f)
        {
            val = Mathf.Log(volume, 2);
        }
        audioMixer.SetFloat("EffectsVolume", (val * 20)+20);
        PlayerPrefs.SetFloat("EffectsVolume", volume);
    }
    #endregion

    private void SyncSettingsWithUI()
    {
        SyncFullscreenToggle();
        SyncVSyncToggle();

        int resolutionIndex = PlayerPrefs.GetInt("Resolution", resolutions.Length - 1);
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        backgroundMusicSlider.value = musicVolume;
        SetMusicVolume(musicVolume);

        float effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 0.6f);
        sfxSlider.value = effectsVolume;
        SetEffectsVolume(effectsVolume);
    }
}
