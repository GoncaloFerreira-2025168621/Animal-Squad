using Unity.Netcode;
using UnityEngine;

public class To_Damage : NetworkBehaviour
{
    [Header("Take Damage")]
    [SerializeField] private GameObject _damagePoint; // Ponto de referência para a posição do dano

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return; // Garante que apenas o jogador local possa controlar o ataque

        if (Input.GetKeyDown(KeyCode.Mouse0))// Detecta o clique do mouse para iniciar o ataque
        {
            // Aqui você pode implementar a lógica de ataque, como detectar inimigos próximos e aplicar dano
            Debug.Log("Ataque realizado! Implementar lógica de dano aqui.");
            ApplyDamageServerRpc(true); // Ativa o ponto de dano para detectar colisões com inimigos e aplicar dano
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))// Detecta quando o botão do mouse é solto para parar o ataque
        {
            ApplyDamageServerRpc(false); // Desativa o ponto de dano para parar de detectar colisões com inimigos
        }
    }

    [ServerRpc]
    public void ApplyDamageServerRpc(bool isAttacking)// Este método é chamado no cliente, mas executado no servidor
    {
        //Ativar o ponto de dano para detectar colisões com inimigos e aplicar dano
        if (isAttacking)
        {
            _damagePoint.SetActive(true);
            ApplyDamageClientRpc(true); // Chama o ClientRpc para atualizar o estado do ponto de dano em todos os clientes
            Debug.Log("Ponto de dano ativado para ataque.");
        }
        else
        {
            _damagePoint.SetActive(false);
            ApplyDamageClientRpc(false); // Chama o ClientRpc para atualizar o estado do ponto de dano em todos os clientes 
            Debug.Log("Ponto de dano desativado para parar ataque.");
        }

        // Aqui você pode implementar a lógica de redução de vida do personagem e verificar se ele morreu
        Debug.Log("A caixa levou dano! Implementar lógica de redução de vida aqui.");
    }

    [ClientRpc]
    public void ApplyDamageClientRpc(bool isAttacking)
    {
        //Ativar o ponto de dano para detectar colisões com inimigos e aplicar dano
        if (isAttacking)
        {
            _damagePoint.SetActive(true);
            Debug.Log("Ponto de dano ativado para ataque.");
        }
        else
        {
            _damagePoint.SetActive(false);
            Debug.Log("Ponto de dano desativado para parar ataque.");
        }

        // Aqui você pode implementar a lógica de redução de vida do personagem e verificar se ele morreu
        Debug.Log("A caixa levou dano! Implementar lógica de redução de vida aqui.");
    }
}
