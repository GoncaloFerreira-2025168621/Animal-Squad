using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Mov_Network : NetworkBehaviour
{
    [Header("Movement")]
    public float _moveSpeed = 5f;
    public float _rotationSpeed = 10f;

    [Header("References")]
    public Transform _cameraTransform; //Referencia a camera para movimentar o personagem de acordo com a direção da câmera

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // Captura a entrada do jogador para movimentação
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Calcula a direção de movimento com base na orientação da câmera
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

        // Ignora a componente Y para manter o movimento no plano horizontal
        camForward.y = 0;
        camRight.y = 0;

        // Normaliza as direções para garantir que o movimento seja consistente
        camForward.Normalize();
        camRight.Normalize();

        // Combina as direções da câmera com a entrada do jogador para obter a direção de movimento final
        Vector3 moveDirection = camForward * vertical + camRight * horizontal;
        moveDirection.Normalize();

        Vector3 lookDirection = camForward;// O personagem sempre olha na direção da câmera

        MoveServerRpc(moveDirection, lookDirection);
    }

    // Executa a movimentação e rotação no servidor para garantir que todos os clientes vejam a mesma coisa
    [ServerRpc]
    void MoveServerRpc(Vector3 moveDirection, Vector3 lookDirection)
    {
        Vector3 velocity = moveDirection * _moveSpeed;
        velocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = velocity;

        // Rotaciona o personagem para olhar na direção da câmera
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);// Calcula a rotação alvo com base na direção da câmera

            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);// Suaviza a rotação usando Slerp para uma transição mais suave
        }
    }


}