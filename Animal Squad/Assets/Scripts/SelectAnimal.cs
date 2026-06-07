using UnityEngine;

public class AnimalSelection : MonoBehaviour
{
    public int _selectedAnimal = 0;

    public void SelectBear()
    {
        _selectedAnimal = 0;
    }

    public void SelectBeaver()
    {
        _selectedAnimal = 1;
    }

    public void SelectMouse()
    {
        _selectedAnimal = 2;
    }

    public void SelectBird()
    {
        _selectedAnimal = 3;
    }
}
