using UnityEngine;
using System;

[Serializable]
public class ShopResponse
{

    public bool success;    // Diz se o pedido correu bem    
    public string message;// Mensagem vinda do servidor   
    public int userID;// ID do utilizador
    public int coins;// Moedas atuais do utilizador
    public AnimalShopData[] animals;// Lista de animais vindos da base de dados
}

[Serializable]
public class AnimalShopData
{
    public int id_animal;// ID do animal na base de dados
    public string name;// Nome do animal
    public string description;// Descrição do animal
    public int price_coins;// Preço do animal

    // 0 = não comprado
    // 1 = comprado
    public int owned;
}

[Serializable]
public class BuyAnimalRequest
{
    public int userID;// ID do utilizador que está a comprar
    public int animalID;// ID do animal que está a comprar
}

[Serializable]
public class BuyAnimalResponse
{
    public bool success;// Diz se a compra correu bem
    public string message;// Mensagem do servidor
    public int newCoins;// Moedas novas depois da compra
}
public class Shop
{
    
}

[Serializable]
public class Players
{
    [Header("Animais que serão vistos na plataforma")]
    public GameObject _BirdObject;
    public GameObject _BeaverObject;
    public GameObject _BearObject;
    public GameObject _RatObject;

    public void HideAllAnimals()
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
}
