using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Mov_Network : NetworkBehaviour
{
    [Header("Movement")]
    public float _moveSpeed = 5f;
    public float _rotationSpeed = 10f;

    [Header("References")]
    public Transform _cameraTransform;

    private Rigidbody _rb;

    [Header("Animator")]
    public Animator _animator;

    private bool _lastMovingState;

    

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        // Procura automaticamente o Animator no filho Rat
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        if (IsOwner)// Só bloqueia o cursor para o jogador local
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (_cameraTransform == null)// Verificação de segurança para evitar erros se a câmera não estiver atribuída
        {
            Debug.LogWarning("Camera Transform ainda não está atribuída.");
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");// Input.GetAxisRaw retorna -1, 0 ou 1, o que é ideal para movimento digital (teclado)
        float vertical = Input.GetAxisRaw("Vertical");// Input.GetAxisRaw retorna -1, 0 ou 1, o que é ideal para movimento digital (teclado)

        Vector3 camForward = _cameraTransform.forward;// Direção para a frente da câmera
        Vector3 camRight = _cameraTransform.right;// Direção para os lados da câmera

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * vertical + camRight * horizontal;// Combina as direções da câmera com o input do jogador para criar a direção de movimento
        moveDirection.Normalize();// Normaliza para garantir que a velocidade seja consistente, mesmo quando se move diagonalmente

        Vector3 lookDirection = camForward;// O personagem olha na direção para a frente da câmera, o que é comum em jogos de terceira pessoa

        MoveServerRpc(moveDirection, lookDirection);// Envia a direção de movimento e olhar para o servidor

        bool isMoving = moveDirection.magnitude > 0.1f;// Verifica se o personagem está se movendo com um pequeno limiar para evitar flutuações quando parado

        // Só envia quando muda de parado para andar ou de andar para parado
        if (isMoving != _lastMovingState)
        {
            _lastMovingState = isMoving;
            IsMovingServerRpc(isMoving);
        }
    }

    [ServerRpc]
    void MoveServerRpc(Vector3 moveDirection, Vector3 lookDirection)// Este método é chamado no cliente, mas executado no servidor
    {
        if (_rb == null)// Verificação de segurança para garantir que o Rigidbody esteja disponível
            _rb = GetComponent<Rigidbody>();// Tenta obter o Rigidbody se ainda não tiver sido atribuído

        Vector3 velocity = moveDirection * _moveSpeed;// Calcula a velocidade com base na direção de movimento e na velocidade definida
        velocity.y = _rb.linearVelocity.y;// Mantém a velocidade vertical atual para permitir que a física do salto ou queda funcione corretamente

        _rb.linearVelocity = velocity;

        if (lookDirection != Vector3.zero)// Verifica se a direção de olhar é válida para evitar erros de rotação
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);// Calcula a rotação alvo com base na direção de olhar

            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.deltaTime);// Interpola suavemente entre a rotação atual e a rotação alvo
        }
    }

    [ServerRpc]
    public void IsMovingServerRpc(bool isMoving)
    {
        Debug.Log("Servidor recebeu isMoving: " + isMoving);

        UpdateAnimationClientRpc(isMoving);
    } 

    [ClientRpc]
    public void UpdateAnimationClientRpc(bool isMoving)
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        if (_animator == null)
        {
            Debug.LogError("Animator não encontrado no player: " + gameObject.name);
            return;
        }

        Debug.Log("Cliente recebeu animação isMoving: " + isMoving);

        _animator.SetBool("isMoving", isMoving);
    }

   
}