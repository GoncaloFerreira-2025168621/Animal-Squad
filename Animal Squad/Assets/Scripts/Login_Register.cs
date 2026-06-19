using System.Collections;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Login_Register : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField _password;
    public TMP_InputField _username;

    [Header("Mensagem")]
    public TMP_Text _messageText;

    [Header("Botões Password")]
    [SerializeField] private Button _showPasswordButton;
    [SerializeField] private Button _hidePasswordButton;

    [Header("Painéis")]
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private GameObject _lobbyPanel;

    void Start()
    {
        // A password começa escondida
        _password.contentType = TMP_InputField.ContentType.Password;
        _password.ForceLabelUpdate();

        // O botão de esconder começa invisível
        if (_hidePasswordButton != null)
        {
            _hidePasswordButton.gameObject.SetActive(false);
        }

        if (_showPasswordButton != null)
        {
            _showPasswordButton.gameObject.SetActive(true);
        }

        // O lobby pode começar escondido até fazer login
        if (_lobbyPanel != null)
        {
            _lobbyPanel.SetActive(false);
        }
    }

    public void ShowPassword()
    {
        // Alterna entre password visível e escondida
        if (_password.contentType == TMP_InputField.ContentType.Password)
        {
            _password.contentType = TMP_InputField.ContentType.Standard;

            if (_hidePasswordButton != null)
            {
                _hidePasswordButton.gameObject.SetActive(true);
            }

            if (_showPasswordButton != null)
            {
                _showPasswordButton.gameObject.SetActive(false);
            }
        }
        else
        {
            _password.contentType = TMP_InputField.ContentType.Password;

            if (_hidePasswordButton != null)
            {
                _hidePasswordButton.gameObject.SetActive(false);
            }

            if (_showPasswordButton != null)
            {
                _showPasswordButton.gameObject.SetActive(true);
            }
        }

        // Atualiza visualmente o campo
        _password.ForceLabelUpdate();
    }

    public void Login()
    {
        // Verifica se escreveu username e password
        if (string.IsNullOrWhiteSpace(_username.text) || string.IsNullOrWhiteSpace(_password.text))
        {
            _messageText.text = "Preenche todos os campos";
            return;
        }

        // Verifica se o client já está ligado ao server
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsClient == false)
        {
            _messageText.text = "Primeiro liga-te ao servidor";
            return;
        }

        // Verifica se existe o LoginNetwork na cena
        if (LoginNetwork.Instance == null)
        {
            _messageText.text = "LoginNetwork não encontrado";
            return;
        }

        _messageText.text = "A fazer login...";

        // Envia o login para o servidor Unity
        LoginNetwork.Instance.LoginServerRpc(_username.text, _password.text);
    }

    public void Register()
    {
        // Verifica se escreveu username e password
        if (string.IsNullOrWhiteSpace(_username.text) || string.IsNullOrWhiteSpace(_password.text))
        {
            _messageText.text = "Preenche todos os campos";
            return;
        }

        // Verifica se o client já está ligado ao server
        if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsClient == false)
        {
            _messageText.text = "Primeiro liga-te ao servidor";
            return;
        }

        // Verifica se existe o LoginNetwork na cena
        if (LoginNetwork.Instance == null)
        {
            _messageText.text = "LoginNetwork não encontrado";
            return;
        }

        _messageText.text = "A registar...";

        // Envia o registo para o servidor Unity
        LoginNetwork.Instance.RegisterServerRpc(_username.text, _password.text);
    }

    public void ReceiveLoginResult(bool success, string message, int userID, string username, bool isLogin)
    {
        // Mostra a resposta do servidor
        _messageText.text = message;

        if (success == false)
        {
            return;
        }

        // Se for apenas registo, não entra logo no lobby
        if (isLogin == false)
        {
            return;
        }

        Debug.Log("Login feito com sucesso");
        Debug.Log("User ID: " + userID);

        // Guarda dados do jogador
        PlayerSession.UserID = userID;
        PlayerSession.Username = username;

        // Esconde o login
        if (_loginPanel != null)
        {
            _loginPanel.SetActive(false);
        }

        // Mostra o lobby
        if (_lobbyPanel != null)
        {
            _lobbyPanel.SetActive(true);
        }
    }

    /*public void LoadScene()// Muda para a cena depois de um login bem-sucedido
    {
        SceneManager.LoadScene("Mapa_1");
    }*/

}