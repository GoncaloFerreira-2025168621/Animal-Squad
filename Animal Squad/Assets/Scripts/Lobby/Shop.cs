using UnityEngine;
using System;

[Serializable]
public class ShopResponse
{
    public bool success;
    public string message;
    public int userID;
    public int coins;
    public AnimalShopData[] animals;
}

[Serializable]
public class AnimalShopData
{
    public int id_animal;
    public string name;
    public string description;
    public int price_coins;
    public int owned;
}

[Serializable]
public class BuyAnimalRequest
{
    public int userID;
    public int animalID;
}

[Serializable]
public class BuyAnimalResponse
{
    public bool success;
    public string message;
    public int newCoins;
}
public class Shop
{
    
}
