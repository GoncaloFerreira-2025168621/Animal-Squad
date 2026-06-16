using UnityEngine;
using UnityEngine.SceneManagement;

public class ButonManager : MonoBehaviour
{
    [SerializeField] private Login_Register _loginRegister;
    [SerializeField] private GameObject _Shop;
    [SerializeField] private GameObject _Maps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
