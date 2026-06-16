using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Shop_Manager : MonoBehaviour
{
    [Header("Servidor")]
    [SerializeField] private string _serverURL = "http://localhost:3000";

    [Header("UI Moedas")]
    [SerializeField] private TMP_Text _coinsText;

    [Header("UI Animal")]
    [SerializeField] private TMP_Text _animalNameText;
    [SerializeField] private TMP_Text _descriptionText;

    [Header("Botões dos Animais")]
    [SerializeField] private Button[] _animalButtons;

    [Tooltip("IDs dos animais na base de dados. Tem de ter o mesmo tamanho que _animalButtons.")]
    [SerializeField] private int[] _animalIDs;

    [Header("Animais que seram vistos na plataforma")]
    [SerializeField] private GameObject _BirdObject;
    [SerializeField] private GameObject _BeaverObject;
    [SerializeField] private GameObject _BearObject;
    [SerializeField] private GameObject _RatObject;

    [Tooltip("Objetos dos cadeados por cima dos botões. Opcional.")]
    [SerializeField] private GameObject[] _lockObjects;

    [Header("Botão Comprar")]
    [SerializeField] private Button _buyButton;
    [SerializeField] private TMP_Text _buyButtonText;

    [Header("Botão Selecionar")]
    [SerializeField] private Button _selectButton;
    [SerializeField] private TMP_Text _selectText;
    [SerializeField] private TMP_Text _selectedText;

    [Header("Mensagem")]
    [SerializeField] private TMP_Text _messageText;

    // Lista de animais carregada da base de dados
    private AnimalShopData[] _animals;

    // Índice do animal que está atualmente a ser mostrado
    private int _currentAnimalIndex = 0;
    [SerializeField] private GameObject _currentAnimal;

    private void Start()
    {
        // Liga automaticamente os botões dos animais às funções certas
        SetupAnimalButtons();

        // Começa com os botões escondidos até receber dados da base de dados
        _buyButton.gameObject.SetActive(false);
        _selectButton.gameObject.SetActive(false);

        // Carrega dados do shop
        StartCoroutine(LoadShop());
    }

    private void SetupAnimalButtons()
    {
        // Segurança para evitar erros se esqueceres de preencher no Inspector
        if (_animalButtons == null || _animalIDs == null)
        {
            Debug.LogError("Animal Buttons ou Animal IDs não foram preenchidos.");
            return;
        }

        if (_animalButtons.Length != _animalIDs.Length)
        {
            Debug.LogError("O número de botões tem de ser igual ao número de IDs.");
            return;
        }

        for (int i = 0; i < _animalButtons.Length; i++)
        {
            // Guardamos o ID numa variável local.
            // Isto é importante para cada botão ficar com o ID certo.
            int animalID = _animalIDs[i];

            // Limpa eventos antigos para evitar chamar duas vezes
            _animalButtons[i].onClick.RemoveAllListeners();

            // Quando clicar neste botão, escolhe o animal com este ID
            _animalButtons[i].onClick.AddListener(() =>
            {
                SelectAnimalByID(animalID);
            });

            if (animalID == 1)
            {
                _animalButtons[i].onClick.AddListener(() =>
                {
                    ShowBird();
                });
            }
            else if (animalID == 2)
            {
                _animalButtons[i].onClick.AddListener(() =>
                {
                    ShowRat();
                });
            }
            else if (animalID == 3) 
            {
                _animalButtons[i].onClick.AddListener(() =>
                {
                    ShowBeaver();
                });
            }
            else if (animalID == 4)
            {
                _animalButtons[i].onClick.AddListener(() =>
                {
                    ShowBear();
                });
            }
        }
    }

    private IEnumerator LoadShop()
    {
        // Monta o URL.
        // Exemplo: http://localhost:3000/shop/1
        string url = _serverURL + "/shop/" + PlayerSession.UserID;

        Debug.Log("A chamar URL da shop: " + url);

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        // Se não conseguir ligar ao Node.js
        if (request.result != UnityWebRequest.Result.Success)
        {
            _messageText.text = "Erro ao carregar shop";
            Debug.Log(request.error);
            yield break;
        }

        // Converte JSON recebido para objeto C#
        ShopResponse response = JsonUtility.FromJson<ShopResponse>(request.downloadHandler.text);

        if (!response.success)
        {
            _messageText.text = response.message;
            yield break;
        }

        // Guarda moedas atuais
        PlayerSession.Coins = response.coins;

        // Guarda lista de animais
        _animals = response.animals;

        // Atualiza visual dos cadeados
        RefreshAnimalButtonsVisual();

        // Mostra o primeiro animal da lista
        ShowAnimal(0);
    }

    public void SelectAnimalByID(int animalID)
    {
        
        if (_animals == null || _animals.Length == 0)
        {
            Debug.Log("Os animais ainda não foram carregados.");
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

        Debug.Log("Animal não encontrado com ID: " + animalID);
    }

    private void ShowAnimal(int index)
    {
        if (_animals == null || _animals.Length == 0)
            return;

        // Guarda qual animal estamos a ver agora
        _currentAnimalIndex = index;

        AnimalShopData animal = _animals[_currentAnimalIndex];

        // Atualiza textos principais
        _coinsText.text = PlayerSession.Coins.ToString();
        _animalNameText.text = animal.name;
        _descriptionText.text = animal.description;

        bool isOwned = animal.owned == 1;
        bool isSelected = PlayerSession.SelectedAnimalID == animal.id_animal;

        // Se o animal ainda NÃO foi comprado
        if (!isOwned)
        {
            // Mostra botão comprar
            _buyButton.gameObject.SetActive(true);

            // Esconde botão selecionar
            _selectButton.gameObject.SetActive(false);

            // O botão de comprar mostra o preço
            _buyButtonText.text = animal.price_coins.ToString();

            // Só deixa comprar se tiver moedas suficientes
            _buyButton.interactable = PlayerSession.Coins >= animal.price_coins;

            return;
        }

        // Se o animal JÁ foi comprado

        // Esconde botão comprar
        _buyButton.gameObject.SetActive(false);

        // Mostra botão selecionar
        _selectButton.gameObject.SetActive(true);

        // Se este animal já está selecionado
        if (isSelected)
        {
            // Desativa botão porque já está selecionado
            _selectButton.interactable = false;

            // Esconde texto "Select"
            _selectText.gameObject.SetActive(false);

            // Mostra texto "Selected"
            _selectedText.gameObject.SetActive(true);
        }
        else
        {
            // Ativa botão porque pode selecionar este animal
            _selectButton.interactable = true;

            // Mostra texto "Select"
            _selectText.gameObject.SetActive(true);

            // Esconde texto "Selected"
            _selectedText.gameObject.SetActive(false);
        }
    }

    public void BuyCurrentAnimal()
    {
        if (_animals == null || _animals.Length == 0)
            return;

        StartCoroutine(BuyAnimal());
    }

    private IEnumerator BuyAnimal()
    {
        AnimalShopData animal = _animals[_currentAnimalIndex];

        // Cria os dados que vão ser enviados para o Node.js
        BuyAnimalRequest buyData = new BuyAnimalRequest();
        buyData.userID = PlayerSession.UserID;
        buyData.animalID = animal.id_animal;

        string json = JsonUtility.ToJson(buyData);

        UnityWebRequest request = new UnityWebRequest(_serverURL + "/shop/buy", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        // Evita duplo clique enquanto está a comprar
        _buyButton.interactable = false;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            _messageText.text = "Erro ao comprar animal";
            Debug.Log(request.error);

            ShowAnimal(_currentAnimalIndex);
            yield break;
        }

        BuyAnimalResponse response = JsonUtility.FromJson<BuyAnimalResponse>(request.downloadHandler.text);

        _messageText.text = response.message;

        if (response.success)
        {
            // Atualiza moedas
            PlayerSession.Coins = response.newCoins;

            // Marca este animal como comprado na lista local
            animal.owned = 1;

            // Atualiza cadeados
            RefreshAnimalButtonsVisual();

            // Atualiza UI.
            // Agora o botão Comprar desaparece e aparece Select.
            ShowAnimal(_currentAnimalIndex);
        }
        else
        {
            // Se falhou, volta a atualizar o botão
            ShowAnimal(_currentAnimalIndex);
        }
    }

    public void SelectCurrentAnimal()
    {
        if (_animals == null || _animals.Length == 0)
            return;

        AnimalShopData animal = _animals[_currentAnimalIndex];

        // Não deixa selecionar animal que ainda não foi comprado
        if (animal.owned != 1)
        {
            _messageText.text = "Tens de comprar este animal primeiro.";
            return;
        }

        // Guarda o animal escolhido pelo jogador
        PlayerSession.SelectedAnimalID = animal.id_animal;

        _messageText.text = animal.name + " selecionado";

        // Atualiza o botão Select / Selected dentro da loja
        ShowAnimal(_currentAnimalIndex);

        // Procura o script que mostra os animais no Lobby
        LobbyRoomManager lobbyRoomManager = FindFirstObjectByType<LobbyRoomManager>();

        if (lobbyRoomManager != null)
        {
            lobbyRoomManager.UpdateSelectedAnimal();
        }
    }

    private void RefreshAnimalButtonsVisual()
    {
        // Se não quiseres usar cadeados por script, podes deixar _lockObjects vazio.
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

            // Se existir um cadeado nesta posição
            if (i < _lockObjects.Length && _lockObjects[i] != null)
            {
                // Se o animal está comprado, esconde o cadeado.
                // Se não está comprado, mostra o cadeado.
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

    public void ShowBird()
    {
        _BirdObject.SetActive(true);
        _BeaverObject.SetActive(false);
        _BearObject.SetActive(false);
        _RatObject.SetActive(false);
    }

    public void ShowRat()
    {
        _RatObject.SetActive(true);
        _BeaverObject.SetActive(false);
        _BearObject.SetActive(false);
        _BirdObject.SetActive(false);
    }

    public void ShowBear()
    {
        _BearObject.SetActive(true);
        _BeaverObject.SetActive(false);
        _RatObject.SetActive(false);
        _BirdObject.SetActive(false);
    }

    public void ShowBeaver()
    { 
        _BeaverObject.SetActive(true); 
        _BearObject.SetActive(false);
        _RatObject.SetActive(false);
        _BirdObject.SetActive(false);
    }
}