using UnityEngine;

public class Detected_push : MonoBehaviour
{
    [SerializeField] private Move_Object _Move;

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
        if (other.gameObject.CompareTag("Object_Bear"))
        {
            _Move = other.gameObject.GetComponent<Move_Object>(); // Obtém o componente Life_Object do objeto que entrou na trigger
            _Move._Moved = true; //passa a variável para true para que o objeto possa ser movido
        }
    }
}
