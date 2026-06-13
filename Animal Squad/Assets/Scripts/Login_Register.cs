using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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

    [Header("StartServer ou StarClient")]
    [SerializeField] private NetworkManagerUI _networkManagerUI;

    // Endereço do servidor Node.js
    // Como estás a testar no teu PC, usa localhost
    private string serverURL = "http://localhost:3000";

    void Start()
    {
        // A password começa escondida
        _password.contentType = TMP_InputField.ContentType.Password;

        // O botão de esconder começa invisível
        _hidePasswordButton.gameObject.SetActive(false);
    }

    public void ShowPassword()
    {
        // Alterna entre password visível e escondida
        if (_password.contentType == TMP_InputField.ContentType.Password)
        {
            _password.contentType = TMP_InputField.ContentType.Standard;
            _hidePasswordButton.gameObject.SetActive(true);
            _showPasswordButton.gameObject.SetActive(false);
        }
        else
        {
            _password.contentType = TMP_InputField.ContentType.Password;
            _hidePasswordButton.gameObject.SetActive(false);
            _showPasswordButton.gameObject.SetActive(true);
        }

        // Atualiza visualmente o campo depois de mudar o contentType
        _password.ForceLabelUpdate();
    }

    public void Login()
    {
        StartCoroutine(SendRequest("/login"));
    }

    public void Register()
    {
        StartCoroutine(SendRequest("/register"));
    }

    IEnumerator SendRequest(string endpoint)// Envia os dados para o servidor Node.js e espera pela resposta
    {
        // Impede enviar dados vazios para o servidor
        if (string.IsNullOrWhiteSpace(_username.text) || string.IsNullOrWhiteSpace(_password.text))
        {
            _messageText.text = "Preenche todos os campos";
            yield break;
        }

        // Dados que vão ser enviados para o Node.js
        UserData data = new UserData();
        data.username = _username.text;
        data.password = _password.text;

        // Converte o objeto para JSON
        string json = JsonUtility.ToJson(data);

        // Cria uma request POST para /login ou /register
        UnityWebRequest request = new UnityWebRequest(serverURL + endpoint, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json); // Converte o JSON para bytes

        request.uploadHandler = new UploadHandlerRaw(bodyRaw); // Envia o JSON no corpo da request
        request.downloadHandler = new DownloadHandlerBuffer(); // Prepara para receber a resposta do servidor

        // Diz ao servidor que estamos a enviar JSON
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        // Caso não consiga ligar ao servidor
        if (request.result != UnityWebRequest.Result.Success)
        {
            _messageText.text = "Erro ao ligar ao servidor";
            Debug.Log(request.error);
            yield break;
        }

        // Converte a resposta JSON do servidor para objeto C#
        ResponseData response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);

        _messageText.text = response.message;// Mostra a mensagem de sucesso ou erro do servidor

        // Se o login for bem-sucedido, depois podes mudar para o Lobby
        if (response.success && endpoint == "/login")
        {
            Debug.Log("Login feito com sucesso!");
            Debug.Log("User ID: " + response.userID);

            PlayerSession.UserID = response.userID;
            PlayerSession.Username = _username.text;

            SceneManager.LoadScene("Lobby");
        }
    }

    public void LoadScene()// Muda para a cena depois de um login bem-sucedido
    {
        SceneManager.LoadScene("Mapa_1");
    }
       
    public void StartServerOrClient()
    {
        if (_username.text == "server" && _password.text == "server")
        {
            _networkManagerUI.StartServer();
        }
        else
        {
            _networkManagerUI.StartClient();
        }
    }

}