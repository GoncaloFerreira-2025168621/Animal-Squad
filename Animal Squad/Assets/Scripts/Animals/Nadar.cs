using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class Nadar : NetworkBehaviour
{
    [SerializeField] private Image _WaterVisual;// Referência ao objeto visual da água para ativar ou desativar conforme a corrente é ativada ou desativada
    private int _WaterContacts = 0;

    [Header("Swim Settings")]
    [SerializeField] private float _SpeedSwim = 5f;
    [SerializeField] private bool _IsSwimming = false;

    private Rigidbody _rb;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        _rb.useGravity = true;// Garante que a gravidade esteja ativada inicialmente, permitindo que o personagem caia normalmente quando não estiver nadando

        GameObject WaterVisual = GameObject.FindGameObjectWithTag("WaterVisual");
        if (WaterVisual != null)
        {
            _WaterVisual = WaterVisual.GetComponent<Image>();
            _WaterVisual.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDirection = Vector3.zero;// Inicializa a direção de movimento como zero

        if (Input.GetKey(KeyCode.Space) && _IsSwimming)
        {
            moveDirection.y = _SpeedSwim;// Se a barra de espaço for pressionada, define a direção de movimento para cima
            SwimServerRpc(moveDirection);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && _IsSwimming)
        {
            moveDirection.y = -_SpeedSwim;// Se a tecla Shift for pressionada, define a direção de movimento para baixo
            SwimServerRpc(moveDirection);
        }

        if (_IsSwimming == true)
        {
            _rb.useGravity = false;// Desativa a gravidade para permitir que o personagem nade sem ser afetado pela gravidade
        }
        else if (_IsSwimming == false)
        {
            _rb.useGravity = true;// Ativa a gravidade quando o personagem não estiver nadando, permitindo que ele caia normalmente
        }

        
    }

    [ServerRpc]
    private void SwimServerRpc(Vector3 moveDirection)
    {
        if (_rb != null)
        {
            Vector3 velocity = _rb.linearVelocity;// Obtém a velocidade atual do Rigidbody
            velocity.y = moveDirection.y;// Define a velocidade vertical com base na direção de movimento recebida do cliente
            _rb.linearVelocity = velocity;
        }

        UpdateSwimClientRpc(moveDirection);
    }

    [ClientRpc]
    private void UpdateSwimClientRpc(Vector3 moveDirection)
    {
        if (_rb != null)
        {
            Vector3 velocity = _rb.linearVelocity;// Obtém a velocidade atual do Rigidbody
            velocity.y = moveDirection.y;// Define a velocidade vertical com base na direção de movimento recebida do cliente
            _rb.linearVelocity = velocity;
        }

        Debug.Log("Atualizando natação nos clientes: " + moveDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Water"))
        {
            _WaterContacts++;
            _IsSwimming = true;

            if (_WaterVisual != null)
                _WaterVisual.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Water"))
        {
            _WaterContacts--;

            if (_WaterContacts <= 0)
            {
                _WaterContacts = 0;
                _IsSwimming = false;
                _WaterVisual.enabled = false;
            }
        }
    }
}
