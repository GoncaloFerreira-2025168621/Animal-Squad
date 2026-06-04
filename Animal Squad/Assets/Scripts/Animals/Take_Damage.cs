using UnityEngine;

public class Take_Damage : MonoBehaviour
{
    [SerializeField] private Life_Object _lifeObject;
    [SerializeField] private float _damageAmount = 10f; // Quantidade de dano a ser aplicada
    [SerializeField] private string _targetTag; // Tag do objeto que pode receber dano

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)// Verifica se o objeto que entrou na trigger tem a tag "Object_Rat"
    {
        if (other.gameObject.CompareTag(_targetTag))
        {
            _lifeObject = other.gameObject.GetComponent<Life_Object>(); // Obtém o componente Life_Object do objeto que entrou na trigger
            _lifeObject.TakeDamage(_damageAmount); // Exemplo de dano
        }
    }
}
