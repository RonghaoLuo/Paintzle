using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField] private Slider masterVolSlider;
    [SerializeField] private Slider uiVolSlider;
    [SerializeField] private Slider sfxVolSlider;
    [SerializeField] private Slider musicVolSlider;
    [SerializeField] private Slider mouseSensitivitySlider;

    private void Start()
    {
        mouseSensitivitySlider.value = GameManager.Instance.MouseSensitivity;
        masterVolSlider.value = GameManager.Instance.MasterVolume;
        uiVolSlider.value = GameManager.Instance.UiVolume;
        sfxVolSlider.value = GameManager.Instance.SfxVolume;
        musicVolSlider.value = GameManager.Instance.MusicVolume;

        masterVolSlider.onValueChanged.AddListener(OnMasterVolumeChange);
        uiVolSlider.onValueChanged.AddListener(OnUIVolumeChange);
        sfxVolSlider.onValueChanged.AddListener(OnSFXVolumeChange);
        musicVolSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChange);
    }

    private void OnDestroy()
    {
        masterVolSlider.onValueChanged.RemoveListener(OnMasterVolumeChange);
        uiVolSlider.onValueChanged.RemoveListener(OnUIVolumeChange);
        sfxVolSlider.onValueChanged.RemoveListener(OnSFXVolumeChange);
        musicVolSlider.onValueChanged.RemoveListener(OnMusicVolumeChange);
        mouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSensitivityChange);
    }

    private void OnMasterVolumeChange(float volume)
    {
        AudioManager.Instance.SetMasterVolume(volume);
        GameManager.Instance.MasterVolume = volume;
    }

    private void OnUIVolumeChange(float volume)
    {
        AudioManager.Instance.SetUiVolume(volume);
        GameManager.Instance.UiVolume = volume;
    }

    private void OnSFXVolumeChange(float volume)
    {
        AudioManager.Instance.SetSfxVolume(volume);
        GameManager.Instance.SfxVolume = volume;
    }

    private void OnMusicVolumeChange(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
        GameManager.Instance.MusicVolume = volume;
    }

    private void OnMouseSensitivityChange(float sensitivity)
    {
        GameManager.Instance.MouseSensitivity = sensitivity;
    }
}
