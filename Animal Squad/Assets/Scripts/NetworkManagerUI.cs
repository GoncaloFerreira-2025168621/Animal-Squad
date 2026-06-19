using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("IP do servidor")]
    [SerializeField] private TMP_InputField _ipInput;

    [Header("Mensagem")]
    [SerializeField] private TMP_Text _messageText;

    [Header("Configuração")]
    [SerializeField] private string _lobbySceneName = "Lobby";
    [SerializeField] private ushort _port = 7777;

    public void StartServer()
    {
        // O servidor escuta ligações dos clientes
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // 0.0.0.0 permite receber clientes de outros PCs
        transport.SetConnectionData("127.0.0.1", _port, "0.0.0.0");

        bool started = NetworkManager.Singleton.StartServer();

        if (started)
        {
            Debug.Log("Server iniciado");

            if (_messageText != null)
            {
                _messageText.text = "Server iniciado";
            }

            // Só o servidor muda a cena
            NetworkManager.Singleton.SceneManager.LoadScene(_lobbySceneName, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Erro ao iniciar Server");

            if (_messageText != null)
            {
                _messageText.text = "Erro ao iniciar Server";
            }
        }
    }

    public void StartClient()
    {
        string ip = "127.0.0.1";

        // Se escreveres IP no input, usa esse IP
        if (_ipInput != null && string.IsNullOrWhiteSpace(_ipInput.text) == false)
        {
            ip = _ipInput.text;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // O cliente liga ao IP do PC servidor
        transport.SetConnectionData(ip, _port);

        bool started = NetworkManager.Singleton.StartClient();

        if (started)
        {
            Debug.Log("Client iniciado");

            if (_messageText != null)
            {
                _messageText.text = "A ligar ao servidor...";
            }
        }
        else
        {
            Debug.Log("Erro ao iniciar Client");

            if (_messageText != null)
            {
                _messageText.text = "Erro ao iniciar Client";
            }
        }
    }
}
