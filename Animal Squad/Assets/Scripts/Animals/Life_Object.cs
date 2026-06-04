using Unity.Netcode;
using UnityEngine;
using static Life_Object_Scriptable;

public class Life_Object : MonoBehaviour
{
    [SerializeField] private Life_Object_Scriptable _lifeObjectScriptable;
    [SerializeField] private float _currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentHealth = _lifeObjectScriptable._maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_currentHealth <= 0)
        {
            DieServerRpc();
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _lifeObjectScriptable._maxHealth);// Garante que a vida atual não fique abaixo de 0 ou acima do máximo
    }


    [ServerRpc]
    public void DieServerRpc()
    {
        // Destroi o objeto
        Destroy(gameObject);
        DieClientRpc();
        // Instancia o efeito de morte na posição do objeto
        Instantiate(_lifeObjectScriptable._Object_Efect, transform.position, Quaternion.identity);
    }

    [ClientRpc]
    public void DieClientRpc()
    {
        // Destroi o objeto
        Destroy(gameObject);
        // Instancia o efeito de morte na posição do objeto
        Instantiate(_lifeObjectScriptable._Object_Efect, transform.position, Quaternion.identity);

    }
}
