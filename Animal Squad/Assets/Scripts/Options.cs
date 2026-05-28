using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject _AudioLinha;
    [SerializeField] private GameObject _VideoLinha;
    [SerializeField] private GameObject _GameplayLinha;
    [SerializeField] private GameObject _OptionsMenu;
    [SerializeField] private GameObject _AudioMenu;
    [SerializeField] private GameObject _VideoMenu;
    [SerializeField] private GameObject _GameplayMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Screen.fullScreen = true;
        _FPSvalidation = false;
    }

    // Update is called once per frame
    void Update()
    {
        fps = 1.0f / Time.deltaTime;
        if (_FPSvalidation == true)
        {
            _ShowFPS.text = "FPS: " + Mathf.Round(fps).ToString();
            _CheckFPSActive.SetActive(true);
            _CheckFPSInactive.SetActive(false);
        }
        else if (_FPSvalidation == false)
        {
            _CheckFPSActive.SetActive(false);
            _CheckFPSInactive.SetActive(true);
            _ShowFPS.text = "";
        }
    }

    #region Menu
    public void BackOptions()//Voltar
    {
        _OptionsMenu.SetActive(false);
    }

    public void OptionsMenu()//Opções
    {
        _OptionsMenu.SetActive(true);
    }

    public void Audio()//Audio
    {
        _AudioLinha.SetActive(true);
        _AudioMenu.SetActive(true);
        _VideoLinha.SetActive(false);
        _VideoMenu.SetActive(false);
        _GameplayLinha.SetActive(false);
        _GameplayMenu.SetActive(false);
        //_AudioMenu.SetActive(true);
    }

    public void Video()//Video
    {
        _VideoLinha.SetActive(true);
        _VideoMenu.SetActive(true);
        _AudioLinha.SetActive(false);
        _AudioMenu.SetActive(false);
        _GameplayLinha.SetActive(false);
        _GameplayMenu.SetActive(false);
        //_VideoMenu.SetActive(true);
    }
    
    public void Gameplay()//Gameplay
    {
        _GameplayLinha.SetActive(true);
        _GameplayMenu.SetActive(true);
        _AudioLinha.SetActive(false);
        _AudioMenu.SetActive(false);
        _VideoLinha.SetActive(false);
        _VideoMenu.SetActive(false);
        //_GameplayMenu.SetActive(true);
    }
    #endregion


    #region Video

    [Header("Screen")]
    public GameObject _ScreenDropdown;
    public TextMeshProUGUI _ScreenText;

    [Header("Resolution")]
    public GameObject _ResolutionDropdown;
    public TextMeshProUGUI _ResolutionText;

    [Header("FPS")]
    public TextMeshProUGUI _ShowFPS;
    public GameObject _CheckFPSActive;
    public GameObject _CheckFPSInactive;
    public bool _FPSvalidation;
    public float fps;

    #region Screen
    public void DropDownScreen()//Dropdown de Screen
    {
        _ScreenDropdown.SetActive(true);
        
    }

    public void WindowScreen()//Janela Normal
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Debug.Log("Windowed");
        _ScreenText.text = "Windowed";
        _ScreenDropdown.SetActive(false);
    }

    public void MaxWindowScreen()//Janela Maximizada
    {
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        Debug.Log("Maximized Window");
        _ScreenText.text = "Max Window";
        _ScreenDropdown.SetActive(false);
    }

    public void ExFullScreen()//Tela Cheia Exclusiva
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Debug.Log("Ex Full Screen");
        _ScreenText.text = "Ex Full Screen";
        _ScreenDropdown.SetActive(false);
    }

    public void FullScreen()//Tela Cheia
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Debug.Log("Full Screen");
        _ScreenText.text = "Full Screen";
        _ScreenDropdown.SetActive(false);
    }
    #endregion

    #region Resolution
    public void DropDownResolution()//Dropdown de Resolução
    {
        _ResolutionDropdown.SetActive(true);
    }

    public void Res720p()//720p
    {
        Screen.SetResolution(1280, 720, true);
        Debug.Log("720p");
        _ResolutionText.text = "720p";
        _ResolutionDropdown.SetActive(false);
    }

    public void Res768p()//768p
    {
        Screen.SetResolution(1366, 768, true);
        Debug.Log("768p");
        _ResolutionText.text = "768p";
        _ResolutionDropdown.SetActive(false);
    }

    public void Res1080p()//1080p
    {
        Screen.SetResolution(1920, 1080, true);
        Debug.Log("1080p");
        _ResolutionText.text = "1080p";
        _ResolutionDropdown.SetActive(false);
    }

    public void Res1440p()//1440p
    {
        Screen.SetResolution(2560, 1440, true);
        Debug.Log("1440p");
        _ResolutionText.text = "1440p";
        _ResolutionDropdown.SetActive(false);
    }

    public void Res2160p()//2160p
    {
        Screen.SetResolution(3840, 2160, true);
        Debug.Log("2160p");
        _ResolutionText.text = "2160p";
        _ResolutionDropdown.SetActive(false);
    }
    #endregion

    public void ShowFPS()//Mostrar FPS
    {
        if (_FPSvalidation == false)
        {
            _FPSvalidation = true;
        }
        else
        {
            _FPSvalidation = false;
        }
    }
    #endregion

    #region Audio
    public AudioMixer _audioMixer;

    public Slider _masterSlider;
    public Slider _sfxSlider;
    public Slider _musicSlider;

    public void SetMasterVolume(float value)
    {
        SetVolume("Master", value);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("Music", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFX", value);
    }

    private void SetVolume(string parameterName, float value)
    {
        if (value <= 0.0001f)
            value = 0.0001f;

        float volume = Mathf.Log10(value) * 20f;

        bool result = _audioMixer.SetFloat(parameterName, volume);

        Debug.Log(parameterName + " | Slider: " + value + " | dB: " + volume + " | Result: " + result);
    }

    #endregion
}