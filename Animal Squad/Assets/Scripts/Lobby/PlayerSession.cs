using UnityEngine;

public static class PlayerSession
{
    // ID do utilizador que veio da base de dados após o login
    public static int UserID;

    // Nome do utilizador logado
    public static string Username;

    // Moedas atuais do jogador
    public static int Coins;

    // Animal atualmente selecionado no Shop
    // -1 significa que ainda nenhum animal foi selecionado
    public static int SelectedAnimalID = -1;
}
