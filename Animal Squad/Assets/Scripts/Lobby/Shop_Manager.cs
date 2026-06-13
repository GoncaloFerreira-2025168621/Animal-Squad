using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Shop_Manager : MonoBehaviour
{
    [Header("UI Moedas")]
    [SerializeField] private TMP_Text _coinsText;

    [Header("UI Animal")]
    [SerializeField] private TMP_Text _animalNameText;
    [SerializeField] private TMP_Text _descriptionText;

    [Header("Botão Comprar")]
    [SerializeField] private Button _buyButton;
    [SerializeField] private TMP_Text _buyButtonText;

    [Header("Mensagem")]
    [SerializeField] private TMP_Text _messageText;

    private string _serverURL = "http://localhost:3000";

    private AnimalShopData[] _animals;
    private int _currentAnimalIndex = 0;

    private void Start()
    {
        StartCoroutine(LoadShop());
    }

    private IEnumerator LoadShop()
    {
        string url = _serverURL + "/shop/" + PlayerSession.UserID;

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            _messageText.text = "Erro ao carregar shop";
            Debug.Log(request.error);
            yield break;
        }

        ShopResponse response = JsonUtility.FromJson<ShopResponse>(request.downloadHandler.text);

        if (!response.success)
        {
            _messageText.text = response.message;
            yield break;
        }

        PlayerSession.Coins = response.coins;

        _animals = response.animals;

        ShowAnimal(0);
    }

    private void ShowAnimal(int index)
    {
        if (_animals == null || _animals.Length == 0)
            return;

        _currentAnimalIndex = index;

        AnimalShopData animal = _animals[_currentAnimalIndex];

        _coinsText.text = PlayerSession.Coins.ToString();

        _animalNameText.text = animal.name;
        _descriptionText.text = animal.description;

        if (animal.owned == 1)
        {
            _buyButton.interactable = false;
            _buyButtonText.text = "Comprado";
        }
        else if (PlayerSession.Coins < animal.price_coins)
        {
            _buyButton.interactable = false;
            _buyButtonText.text = "Sem moedas";
        }
        else
        {
            _buyButton.interactable = true;
            _buyButtonText.text = "Comprar";
        }
    }

    public void NextAnimal()
    {
        if (_animals == null || _animals.Length == 0)
            return;

        int nextIndex = _currentAnimalIndex + 1;

        if (nextIndex >= _animals.Length)
            nextIndex = 0;

        ShowAnimal(nextIndex);
    }

    public void PreviousAnimal()
    {
        if (_animals == null || _animals.Length == 0)
            return;

        int previousIndex = _currentAnimalIndex - 1;

        if (previousIndex < 0)
            previousIndex = _animals.Length - 1;

        ShowAnimal(previousIndex);
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

        BuyAnimalRequest buyData = new BuyAnimalRequest();
        buyData.userID = PlayerSession.UserID;
        buyData.animalID = animal.id_animal;

        string json = JsonUtility.ToJson(buyData);

        UnityWebRequest request = new UnityWebRequest(_serverURL + "/shop/buy", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            _messageText.text = "Erro ao comprar animal";
            Debug.Log(request.error);
            yield break;
        }

        BuyAnimalResponse response = JsonUtility.FromJson<BuyAnimalResponse>(request.downloadHandler.text);

        _messageText.text = response.message;

        if (response.success)
        {
            PlayerSession.Coins = response.newCoins;

            animal.owned = 1;

            ShowAnimal(_currentAnimalIndex);
        }
    }
}