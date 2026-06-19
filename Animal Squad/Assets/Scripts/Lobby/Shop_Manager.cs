using System.Collections;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Shop_Manager : MonoBehaviour
{
    [Header("Animais entre cenas")]
    [SerializeField] private SaveAnimal _SaveAnimal;

    [Header("UI Moedas")]
    [SerializeField] private TMP_Text _coinsText;

    [Header("UI Animal")]
    [SerializeField] private TMP_Text _animalNameText;
    [SerializeField] private TMP_Text _descriptionText;

    [Header("Botoes dos Animais")]
    [SerializeField] private Button[] _animalButtons;

    [Tooltip("IDs dos animais na base de dados. Tem de ter o mesmo tamanho que _animalButtons.")]
    [SerializeField] private int[] _animalIDs;

    [Header("Animais que serao vistos na plataforma")]
    [SerializeField] private GameObject _BirdObject;
    [SerializeField] private GameObject _BeaverObject;
    [SerializeField] private GameObject _BearObject;
    [SerializeField] private GameObject _RatObject;

    [Tooltip("Objetos dos cadeados por cima dos botoes. Opcional.")]
    [SerializeField] private GameObject[] _lockObjects;

    [Header("Botao Comprar")]
    [SerializeField] private Button _buyButton;
    [SerializeField] private TMP_Text _buyButtonText;

    [Header("Botao Selecionar")]
    [SerializeField] private Button _selectButton;
    [SerializeField] private TMP_Text _selectText;
    [SerializeField] private TMP_Text _selectedText;

    [Header("Mensagem")]
    [SerializeField] private TMP_Text _messageText;

    // Lista de animais carregada pelo Server
    private AnimalShopData[] _animals;

    // Indice do animal que esta atualmente a ser mostrado
    private int _currentAnimalIndex = 0;

    private void Start()
    {
        // Liga automaticamente os botoes dos animais as funcoes certas
        SetupAnimalButtons();

        // Comeca com os botoes escondidos ate receber dados do Server
        if (_buyButton != null)
        {
            _buyButton.gameObject.SetActive(false);
        }

        if (_selectButton != null)
        {
            _selectButton.gameObject.SetActive(false);
        }

        HideAllPreviewAnimals();

        // Se o jogador ja fez login, pode carregar a shop
        if (PlayerSession.UserID > 0)
        {
            LoadShopFromServer();
        }
    }

    private void OnEnable()
    {
        // Quando o painel da shop abre, tenta carregar a shop
        if (PlayerSession.UserID > 0 && _animals == null)
        {
            LoadShopFromServer();
        }
    }

    private void SetupAnimalButtons()
    {
        // Seguranca para evitar erros se esqueceres de preencher no Inspector
        if (_animalButtons == null || _animalIDs == null)
        {
            Debug.LogError("Animal Buttons ou Animal IDs nao foram preenchidos.");
            return;
        }

        if (_animalButtons.Length != _animalIDs.Length)
        {
            Debug.LogError("O numero de botoes tem de ser igual ao numero de IDs.");
            return;
        }

        for (int i = 0; i < _animalButtons.Length; i++)
        {
            int animalID = _animalIDs[i];

            // Limpa eventos antigos para evitar chamar duas vezes
            _animalButtons[i].onClick.RemoveAllListeners();

            // Quando clicar neste botao, escolhe o animal com este ID
            _animalButtons[i].onClick.AddListener(() =>
            {
                SelectAnimalByID(animalID);
                ShowPreviewAnimal(animalID);
            });
        }
    }

    // Chamado pelo LoginNetwork depois do login ou pelo botao da Shop
    public void LoadShopFromServer()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
        {
            SetMessage("Ainda nao estas ligado ao servidor.");
            return;
        }

        if (ShopNetwork.Instance == null)
        {
            SetMessage("ShopNetwork nao encontrado na cena.");
            return;
        }

        SetMessage("A carregar shop...");

        if (_buyButton != null)
        {
            _buyButton.gameObject.SetActive(false);
        }

        if (_selectButton != null)
        {
            _selectButton.gameObject.SetActive(false);
        }

        // Pede ao Server para carregar a shop.
        // O Client nao fala com Node.js nem BD.
        ShopNetwork.Instance.LoadShopServerRpc();
    }

    // Chamado pelo ShopNetwork quando o Server recebe a resposta do Node.js
    public void ReceiveShopFromServer(string json)
    {
        ShopResponse response = JsonUtility.FromJson<ShopResponse>(json);

        if (!response.success)
        {
            SetMessage(response.message);
            return;
        }

        // Guarda moedas atuais
        PlayerSession.Coins = response.coins;

        // Guarda lista de animais
        _animals = response.animals;

        // Atualiza visual dos cadeados
        RefreshAnimalButtonsVisual();

        // Mostra o primeiro animal da lista
        ShowAnimal(0);

        SetMessage(response.message);
    }

    public void SelectAnimalByID(int animalID)
    {
        if (_animals == null || _animals.Length == 0)
        {
            Debug.Log("Os animais ainda nao foram carregados.");
            SetMessage("Shop ainda nao carregada.");
            return;
        }

        // Procura dentro da lista qual animal tem o ID recebido
        for (int i = 0; i < _animals.Length; i++)
        {
            if (_animals[i].id_animal == animalID)
            {
                ShowAnimal(i);
                return;
            }
        }

        Debug.Log("Animal nao encontrado com ID: " + animalID);
    }

    private void ShowAnimal(int index)
    {
        if (_animals == null || _animals.Length == 0)
            return;

        if (index < 0 || index >= _animals.Length)
            return;

        // Guarda qual animal estamos a ver agora
        _currentAnimalIndex = index;

        AnimalShopData animal = _animals[_currentAnimalIndex];

        // Atualiza textos principais
        if (_coinsText != null)
        {
            _coinsText.text = PlayerSession.Coins.ToString();
        }

        if (_animalNameText != null)
        {
            _animalNameText.text = animal.name;
        }

        if (_descriptionText != null)
        {
            _descriptionText.text = animal.description;
        }

        bool isOwned = animal.owned == 1;
        bool isSelected = PlayerSession.SelectedAnimalID == animal.id_animal;

        // Se o animal ainda NAO foi comprado
        if (!isOwned)
        {
            if (_buyButton != null)
            {
                _buyButton.gameObject.SetActive(true);
                _buyButton.interactable = PlayerSession.Coins >= animal.price_coins;
            }

            if (_selectButton != null)
            {
                _selectButton.gameObject.SetActive(false);
            }

            if (_buyButtonText != null)
            {
                _buyButtonText.text = animal.price_coins.ToString();
            }

            return;
        }

        // Se o animal JA foi comprado
        if (_buyButton != null)
        {
            _buyButton.gameObject.SetActive(false);
        }

        if (_selectButton != null)
        {
            _selectButton.gameObject.SetActive(true);
        }

        // Se este animal ja esta selecionado
        if (isSelected)
        {
            if (_selectButton != null)
            {
                _selectButton.interactable = false;
            }

            if (_selectText != null)
            {
                _selectText.gameObject.SetActive(false);
            }

            if (_selectedText != null)
            {
                _selectedText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (_selectButton != null)
            {
                _selectButton.interactable = true;
            }

            if (_selectText != null)
            {
                _selectText.gameObject.SetActive(true);
            }

            if (_selectedText != null)
            {
                _selectedText.gameObject.SetActive(false);
            }
        }
    }

    public void BuyCurrentAnimal()
    {
        if (_animals == null || _animals.Length == 0)
            return;

        if (ShopNetwork.Instance == null)
        {
            SetMessage("ShopNetwork nao encontrado.");
            return;
        }

        AnimalShopData animal = _animals[_currentAnimalIndex];

        // Evita duplo clique enquanto esta a comprar
        if (_buyButton != null)
        {
            _buyButton.interactable = false;
        }

        SetMessage("A comprar animal...");

        // Pede ao Server para comprar.
        // O Client nao envia UserID, porque o Server ja sabe qual UserID pertence a este client.
        ShopNetwork.Instance.BuyAnimalServerRpc(animal.id_animal);
    }

    // Chamado pelo ShopNetwork quando o Server recebe a resposta da compra
    public void ReceiveBuyResultFromServer(bool success, string message, int newCoins, int animalID)
    {
        SetMessage(message);

        if (success)
        {
            // Atualiza moedas
            PlayerSession.Coins = newCoins;

            // Marca o animal como comprado na lista local
            AnimalShopData animal = FindAnimalByID(animalID);

            if (animal != null)
            {
                animal.owned = 1;
            }

            // Atualiza cadeados
            RefreshAnimalButtonsVisual();
        }

        // Atualiza UI
        ShowAnimal(_currentAnimalIndex);
    }

    public void SelectCurrentAnimal()
    {
        if (_animals == null || _animals.Length == 0)
            return;

        AnimalShopData animal = _animals[_currentAnimalIndex];

        // Nao deixa selecionar animal que ainda nao foi comprado
        if (animal.owned != 1)
        {
            SetMessage("Tens de comprar este animal primeiro.");
            return;
        }

        // Guarda o animal escolhido pelo jogador local
        PlayerSession.SelectedAnimalID = animal.id_animal;

        SetMessage(animal.name + " selecionado");

        // Atualiza o botao Select / Selected dentro da loja
        ShowAnimal(_currentAnimalIndex);

        // Atualiza o animal na plataforma do lobby
        LobbyRoomManager lobbyRoomManager = FindFirstObjectByType<LobbyRoomManager>();

        if (lobbyRoomManager != null)
        {
            lobbyRoomManager.UpdateSelectedAnimal();
        }
    }

    private void RefreshAnimalButtonsVisual()
    {
        // Se nao quiseres usar cadeados por script, podes deixar _lockObjects vazio.
        if (_lockObjects == null || _lockObjects.Length == 0)
            return;

        if (_animals == null)
            return;

        for (int i = 0; i < _animalIDs.Length; i++)
        {
            int animalID = _animalIDs[i];

            AnimalShopData animal = FindAnimalByID(animalID);

            if (animal == null)
                continue;

            // Se existir um cadeado nesta posicao
            if (i < _lockObjects.Length && _lockObjects[i] != null)
            {
                // Se o animal esta comprado, esconde o cadeado.
                // Se nao esta comprado, mostra o cadeado.
                _lockObjects[i].SetActive(animal.owned == 0);
            }
        }
    }

    private AnimalShopData FindAnimalByID(int animalID)
    {
        if (_animals == null)
            return null;

        for (int i = 0; i < _animals.Length; i++)
        {
            if (_animals[i].id_animal == animalID)
                return _animals[i];
        }

        return null;
    }

    private void ShowPreviewAnimal(int animalID)
    {
        if (animalID == 1)
        {
            ShowBird();

            if (_SaveAnimal != null)
            {
                _SaveAnimal._AnimalSelect = 3;
            }
        }
        else if (animalID == 2)
        {
            ShowRat();

            if (_SaveAnimal != null)
            {
                _SaveAnimal._AnimalSelect = 2;
            }
        }
        else if (animalID == 3)
        {
            ShowBeaver();

            if (_SaveAnimal != null)
            {
                _SaveAnimal._AnimalSelect = 1;
            }
        }
        else if (animalID == 4)
        {
            ShowBear();

            if (_SaveAnimal != null)
            {
                _SaveAnimal._AnimalSelect = 0;
            }
        }
    }

    private void HideAllPreviewAnimals()
    {
        if (_BirdObject != null)
            _BirdObject.SetActive(false);

        if (_BeaverObject != null)
            _BeaverObject.SetActive(false);

        if (_BearObject != null)
            _BearObject.SetActive(false);

        if (_RatObject != null)
            _RatObject.SetActive(false);
    }

    public void ShowBird()
    {
        if (_BirdObject != null)
            _BirdObject.SetActive(true);

        if (_BeaverObject != null)
            _BeaverObject.SetActive(false);

        if (_BearObject != null)
            _BearObject.SetActive(false);

        if (_RatObject != null)
            _RatObject.SetActive(false);
    }

    public void ShowRat()
    {
        if (_RatObject != null)
            _RatObject.SetActive(true);

        if (_BeaverObject != null)
            _BeaverObject.SetActive(false);

        if (_BearObject != null)
            _BearObject.SetActive(false);

        if (_BirdObject != null)
            _BirdObject.SetActive(false);
    }

    public void ShowBear()
    {
        if (_BearObject != null)
            _BearObject.SetActive(true);

        if (_BeaverObject != null)
            _BeaverObject.SetActive(false);

        if (_RatObject != null)
            _RatObject.SetActive(false);

        if (_BirdObject != null)
            _BirdObject.SetActive(false);
    }

    public void ShowBeaver()
    {
        if (_BeaverObject != null)
            _BeaverObject.SetActive(true);

        if (_BearObject != null)
            _BearObject.SetActive(false);

        if (_RatObject != null)
            _RatObject.SetActive(false);

        if (_BirdObject != null)
            _BirdObject.SetActive(false);
    }

    private void SetMessage(string message)
    {
        Debug.Log(message);

        if (_messageText != null)
        {
            _messageText.text = message;
        }
    }
}