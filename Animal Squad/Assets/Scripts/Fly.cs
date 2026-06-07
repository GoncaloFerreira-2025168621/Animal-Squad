using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fly : NetworkBehaviour
{
    [Header("Fly Settings")]
    [SerializeField] private float _moveDirectionValue = 5f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDirection = Vector3.zero;// Inicializa a direção de movimento como zero

        if (Input.GetKey(KeyCode.Space))
        {
            moveDirection.y = _moveDirectionValue;// Se a barra de espaço for pressionada, define a direção de movimento para cima
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection.y = -_moveDirectionValue;// Se a tecla Shift for pressionada, define a direção de movimento para baixo
        }

        FlyServerRpc(moveDirection);
    }

    [ServerRpc]
    private void FlyServerRpc(Vector3 moveDirection)
    {
        if (_rb != null)
        {
            Vector3 velocity = _rb.linearVelocity;// Obtém a velocidade atual do Rigidbody
            velocity.y = moveDirection.y;// Define a velocidade vertical com base na direção de movimento recebida do cliente
            _rb.linearVelocity = velocity;
        }

        UpdateFlyClientRpc(moveDirection);
    }

    [ClientRpc]
    private void UpdateFlyClientRpc(Vector3 moveDirection)
    {
        Debug.Log("Atualizando voo nos clientes: " + moveDirection);
    }
}
