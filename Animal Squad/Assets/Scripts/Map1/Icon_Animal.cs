using UnityEngine;

public class Icon_Animal : MonoBehaviour
{
    [SerializeField] private SaveAnimal _Id_Animal;
    [SerializeField] private GameObject _Bear;
    [SerializeField] private GameObject _Bird;
    [SerializeField] private GameObject _Mouse;
    [SerializeField] private GameObject _Beaver;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_Id_Animal._AnimalSelect == 0)
        {
            _Bear.gameObject.SetActive(true);
        }
        else if (_Id_Animal._AnimalSelect == 1)
        {
            _Beaver.gameObject.SetActive(true);
        }
        else if (_Id_Animal._AnimalSelect == 2)
        {
            _Mouse.gameObject.SetActive(true);
        }
        else if (_Id_Animal._AnimalSelect == 3)
        {
            _Bird.gameObject.SetActive(true);
        }
    }

}
