using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButonManager : MonoBehaviour
{

    [SerializeField] private Login_Register _loginRegister;
    [SerializeField] private GameObject _Shop;
    [SerializeField] private GameObject _Maps;

    [SerializeField] private GameObject _Menu;


    [Header("Audio Source")]
    [SerializeField] private AudioSource _sfxSource;

    [Header("Sons UI")]
    [SerializeField] private AudioClip _buttonClick;
    [SerializeField] private AudioClip _buttonHover;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ShowMenu();
        }

    }  

    public void PlayButtonClick()
    {
        _sfxSource.PlayOneShot(_buttonClick);
    }

    public void PlayButtonHover()
    {
        _sfxSource.PlayOneShot(_buttonHover);
    }

    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void ShowPassword()
    {
        _loginRegister.ShowPassword();
    }

    public void Register()
    {
        _loginRegister.Register();
    }

    public void Login()
    {
        _loginRegister.Login();
    }

    public void Play()
    {
        SceneManager.LoadScene("Login_Register");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Options()
    {
        Debug.Log("Carregando cena de opções...");
        SceneManager.LoadScene("Options");
    }

    public void Credits()
    {
        Debug.Log("Carregando cena de créditos...");
        SceneManager.LoadScene("Credits");
    }

    public void ShowShop()
    {
        _Shop.SetActive(true);
    }

    public void HideShop()
    {
        _Shop.SetActive(false);
    }

    public void HideMaps()
    {
        _Maps.SetActive(false);
    }

    public void SairDoClient()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("Main_Menu"); // ou "Lobby", depende do teu nome da cena
    }

    public void ShowMenu()
    {
        _Menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideMenu()
    {
        _Menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
