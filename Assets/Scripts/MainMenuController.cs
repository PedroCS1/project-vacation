using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] AudioClip SFXAUDIOTEST;
    [SerializeField] AudioClip MUSICAUDIOTEST;
    [Header("Settings")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    private void Start()
    {
        Initialize();
    }

    private async void Initialize()
    {
        await Task.Yield();
        await Task.Yield();
        await Task.Yield();
        await Task.Yield();
        _musicSlider.value = ServiceManager.Instance.GameAudio.GetMusicVolume();
        _sfxSlider.value = ServiceManager.Instance.GameAudio.GetSFXVolume();
    }

    public void StartGame()
    {
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnMusicVolumeChange(float volume)
    {
        if (ServiceManager.Instance != null && ServiceManager.Instance.GameAudio != null)
        {
            ServiceManager.Instance.GameAudio.OnMusicVolumeChange(volume);
        }
    }

    public void OnSFXVolumeChange(float volume)
    {
        if (ServiceManager.Instance != null && ServiceManager.Instance.GameAudio != null)
        {
            ServiceManager.Instance.GameAudio.OnSFXVolumeChange(volume);
        }
    }

    public void PlayMusic()
    {
        if (ServiceManager.Instance != null && ServiceManager.Instance.GameAudio != null)
        {
            ServiceManager.Instance.GameAudio.PlayMusic(MUSICAUDIOTEST);
        }
    }
    public void PlaySFX()
    {
        if (ServiceManager.Instance != null && ServiceManager.Instance.GameAudio != null)
        {
            ServiceManager.Instance.GameAudio.Play(SFXAUDIOTEST);
        }
    }
}
