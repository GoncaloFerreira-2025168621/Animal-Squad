using Unity.Netcode;
using UnityEngine;

public class Take_Object : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _BicoTransform;

    [Header("Settings")]
    [SerializeField] private float _DropForce = 2f;

    private NetworkObject _ObjectToTake;// Referência ao objeto que o pássaro pode pegar (definida quando o pássaro entra na trigger de um objeto)
    private NetworkObject _ObjectCarried;// Referência ao objeto que o pássaro está carregando (definida quando o pássaro pega um objeto)

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (_ObjectToTake != null)
            {
                TakeObjectServerRpc(_ObjectToTake);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))// Verifica se o botão do mouse foi solto para largar o objeto
        {
            DropObjectServerRpc();
        }
    }

    void FixedUpdate()
    {
        if (!IsServer) return;

        if (_ObjectCarried != null)// Se estiver carregando um objeto, mantenha-o na posição do bico
        {
            _ObjectCarried.transform.position = _BicoTransform.position;// Atualiza a posição do objeto carregado para a posição do bico
            _ObjectCarried.transform.rotation = _BicoTransform.rotation;// Atualiza a rotação do objeto carregado para a rotação do bico
        }
        
    }

    [ServerRpc]
    private void TakeObjectServerRpc(NetworkObjectReference objectReference)// Este método é chamado no servidor para pegar o objeto referenciado
    {
        if (objectReference.TryGet(out NetworkObject objectNetwork))// Tenta obter o objeto de rede a partir da referência
        {
            _ObjectCarried = objectNetwork;// Define o objeto carregado como o objeto de rede obtido

            Rigidbody rb = _ObjectCarried.GetComponent<Rigidbody>();

            if (rb != null)// Verifica se o objeto possui um Rigidbody
            {
                rb.linearVelocity = Vector3.zero;// Zera a velocidade linear do objeto para evitar que ele continue se movendo
                rb.angularVelocity = Vector3.zero;// Zera a velocidade angular do objeto para evitar que ele continue girando
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            Debug.Log("Objeto apanhado pelo pássaro.");
        }
    }

    [ServerRpc]
    private void DropObjectServerRpc()
    {
        Debug.Log("Tentando largar o objeto...");
        if (_ObjectCarried == null) return;

        Rigidbody rb = _ObjectCarried.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            rb.AddForce(transform.forward * _DropForce, ForceMode.Impulse);
        }

        _ObjectCarried = null;

        Debug.Log("Objeto largado.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Object_Bird"))// Verifica se o objeto colidido é um objeto que o pássaro pode pegar
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                _ObjectToTake = networkObject;
                Debug.Log("Objeto disponível para apanhar.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Object_Bird"))// Verifica se o objeto que saiu da trigger é um objeto que o pássaro pode pegar
        {
            NetworkObject networkObject = other.GetComponent<NetworkObject>();

            if (networkObject != null && networkObject == _ObjectToTake)// Verifica se o objeto que saiu da trigger é o objeto que o pássaro pode pegar
            {
                _ObjectToTake = null;
                Debug.Log("Saiu da zona do objeto.");
            }
        }
    }
}
