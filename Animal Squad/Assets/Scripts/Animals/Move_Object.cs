using System;
using Unity.Netcode;
using UnityEngine;

public class Move_Object : NetworkBehaviour
{
    private Rigidbody _Rb;
    [SerializeField] private bool _RotatePushing = true;
    [SerializeField] private float _Push_Memory_Time = 0.15f;

    private RigidbodyConstraints _Original_Constraints;
    private float _Last_Push_Time = -999f;

    [Header("Quem pode empurrar este objeto")]
    [SerializeField] private bool _Can_Be_Pushed_By_Bear = true;
    [SerializeField] private bool _Can_Be_Pushed_By_Mouse = false;
    [SerializeField] private bool _Can_Be_Pushed_By_Beaver = false;
    [SerializeField] private bool _Can_Be_Pushed_By_Bird = false;
    [SerializeField] private bool _Can_Be_Pushed_By_Chameleon = false;
    [SerializeField] private bool _Can_Be_Pushed_By_Spider = false;

    [Header("Velocidade do empurrão")]
    [SerializeField] private float _Bear_Push_Speed = 2.5f;
    [SerializeField] private float _Beaver_Push_Speed = 1.5f;

    [Header("Travão quando não está a ser empurrado")]
    [SerializeField] private float _Stop_Horizontal_Speed = 0.2f;

    // Variáveis para controlar o estado de empurrão
    private bool _Is_Being_Pushed = false;// Indica se o objeto está a ser empurrado no momento
    private Vector3 _Push_Direction = Vector3.zero;
    private float _Current_Push_Speed = 0f;

    private void Awake()
    {
        _Rb = GetComponent<Rigidbody>();
        _Original_Constraints = _Rb.constraints;
    }

    public override void OnNetworkSpawn()
    {
        _Rb.isKinematic = false;
        _Rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        bool _Recently_Pushed = Time.time - _Last_Push_Time <= _Push_Memory_Time;

        if (_Is_Being_Pushed || _Recently_Pushed)// Se o objeto está a ser empurrado ou foi empurrado recentemente, aplica o movimento de empurrão
        {
            ApplyPushMovement();
            FreezeRotation();
        }
        else
        {
            StopHorizontalMovementSlowly();
            UnfreezeRotation();
        }

        _Is_Being_Pushed = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryPushServerRpc(string _Animal_Type, Vector3 _Direction)// Este método é chamado pelos animais para tentar empurrar o objeto, passando o tipo do animal e a direção do empurrão
    {
        if (!IsServer) return;

        if (!CanAnimalPush(_Animal_Type))
            return;

        _Direction = Vector3.ProjectOnPlane(_Direction, Vector3.up);

        if (_Direction.sqrMagnitude < 0.01f)
            return;

        _Direction.Normalize();

        _Push_Direction = _Direction;
        _Current_Push_Speed = GetAnimalPushSpeed(_Animal_Type);
        _Is_Being_Pushed = true;
        _Last_Push_Time = Time.time;// Atualiza o tempo do último empurrão para controlar a duração do efeito de empurrão
    }

    private void ApplyPushMovement()// Aplica a velocidade de empurrão ao objeto, mantendo a velocidade vertical atual para não interferir com a gravidade
    {
        Vector3 _Current_Velocity = _Rb.linearVelocity;// Obtém a velocidade atual do objeto
        

        Vector3 _Horizontal_Velocity = _Push_Direction * _Current_Push_Speed;

        _Rb.linearVelocity = new Vector3(_Horizontal_Velocity.x, _Current_Velocity.y, _Horizontal_Velocity.z);// Aplica a velocidade de empurrão mantendo a velocidade vertical atual

        _Rb.angularVelocity = Vector3.zero;
    }

    private void StopHorizontalMovementSlowly()// Aplica um travão para parar o movimento horizontal lentamente quando não está a ser empurrado
    {
        /*if (_RotatePushing)
        {
            _Rb.freezeRotation = false;// Libera a rotação do objeto quando não está a ser empurrado para permitir que ele gire normalmente
        }*/

        Vector3 _Current_Velocity = _Rb.linearVelocity;

        Vector3 _Horizontal_Velocity = new Vector3(_Current_Velocity.x, 0f, _Current_Velocity.z);// Velocidade horizontal atual sem a componente vertical

        if (_Horizontal_Velocity.magnitude < _Stop_Horizontal_Speed)// Se a velocidade horizontal é muito baixa, para completamente
        {
            _Rb.linearVelocity = new Vector3(0f, _Current_Velocity.y, 0f);
        }
    }

    private bool CanAnimalPush(string _Animal_Type)
    {
        switch (_Animal_Type)
        {
            case "Bear":
                return _Can_Be_Pushed_By_Bear;

            case "Mouse":
                return _Can_Be_Pushed_By_Mouse;

            case "Beaver":
                return _Can_Be_Pushed_By_Beaver;

            case "Bird":
                return _Can_Be_Pushed_By_Bird;

            case "Chameleon":
                return _Can_Be_Pushed_By_Chameleon;

            case "Spider":
                return _Can_Be_Pushed_By_Spider;

            default:
                return false;
        }
    }

    private float GetAnimalPushSpeed(string _Animal_Type)
    {
        switch (_Animal_Type)
        {
            case "Bear":
                return _Bear_Push_Speed;

            case "Beaver":
                return _Beaver_Push_Speed;

            default:
                return 0f;
        }
    }

    private void FreezeRotation()
    {
        if (!_RotatePushing)
            return;

        _Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void UnfreezeRotation()
    {
        if (!_RotatePushing)
            return;

        _Rb.constraints = _Original_Constraints;
    }

}
