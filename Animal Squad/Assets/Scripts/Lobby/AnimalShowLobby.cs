using UnityEngine;

public class AnimalShowLobby : MonoBehaviour
{
    [Header("Plataformas dos jogadores")]
    [SerializeField] public Players[] _players;

    private void Start()
    {
        HideAllSlots();
    }

    // Mostra apenas o animal local do jogador, antes de entrar numa sala
    public void ShowLocalPreview(int animalID)
    {
        HideAllSlots();

        // Fora de uma sala, o jogador vê-se sempre na plataforma 0
        ShowAnimalInSlot(0, animalID);
    }

    // Mostra os animais dos jogadores que estão dentro da mesma sala
    public void ShowRoomAnimals(int animalSlot0, int animalSlot1, int animalSlot2, int animalSlot3)
    {
        HideAllSlots();

        ShowAnimalInSlot(0, animalSlot0);
        ShowAnimalInSlot(1, animalSlot1);
        ShowAnimalInSlot(2, animalSlot2);
        ShowAnimalInSlot(3, animalSlot3);
    }

    public void HideAllSlots()
    {
        if (_players == null)
            return;

        for (int i = 0; i < _players.Length; i++)
        {
            _players[i].HideAllAnimals();
        }
    }

    private void ShowAnimalInSlot(int slotIndex, int animalID)
    {
        if (_players == null)
            return;

        if (slotIndex < 0 || slotIndex >= _players.Length)
            return;

        _players[slotIndex].HideAllAnimals();

        if (animalID == 1)
        {
            _players[slotIndex]._BirdObject.SetActive(true);
        }
        else if (animalID == 2)
        {
            _players[slotIndex]._RatObject.SetActive(true);
        }
        else if (animalID == 3)
        {
            _players[slotIndex]._BeaverObject.SetActive(true);
        }
        else if (animalID == 4)
        {
            _players[slotIndex]._BearObject.SetActive(true);
        }
    }
}

