using UnityEngine;

public class RiverCurrent : MonoBehaviour
{
    [SerializeField] private GameObject _WaterVisual;// Referência ao objeto visual da água para ativar ou desativar conforme a corrente é ativada ou desativada
    [SerializeField] private bool _Swim;
    private Collider _collider;// Referência ao collider do objeto para controlar quando ele deve ser um trigger ou não

    [Header("Força da corrente")]
    public float _Strength = 6f;

    [Header("Estado")]
    [SerializeField] private bool _ActiveCurrent = false;

    private void Start()
    {
        _collider = GetComponent<Collider>();// Obtém o componente Collider do objeto para controlar suas propriedades posteriormente
        _collider.isTrigger = false;// Garante que o collider do personagem não seja um trigger inicialmente, permitindo que ele colida normalmente com outros objetos quando não estiver nadando
    }

    public void ActivateCurrent()
    {
        _ActiveCurrent = true;// Ativa a corrente, permitindo que os objetos sejam afetados por ela
    }

    public void DeactivateCurrent()
    {
        _ActiveCurrent = false;// Desativa a corrente, impedindo que os objetos sejam afetados por ela
    }

    public void ActiveVisual()
    {
        if (_WaterVisual != null)
            _WaterVisual.SetActive(true);// Ativa o visual da água, indicando que a corrente está ativa
    }

    public void DeactiveVisual()
    {
        if (_WaterVisual != null)
            _WaterVisual.SetActive(false);// Desativa o visual da água, indicando que a corrente está inativa
    }

    private void OnTriggerStay(Collider other)//
    {
        _WaterVisual.SetActive(true);// Ativa o visual da água sempre que um objeto estiver dentro do trigger, indicando que a corrente está ativa

        if (!_ActiveCurrent) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();// Tenta pegar o Rigidbody do objeto que entrou no trigger

        if (rb == null) return;

        rb.AddForce(transform.forward * _Strength, ForceMode.Acceleration);// Aplica uma força na direção do fluxo da corrente, multiplicada pela força definida
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Castor"))
        {
            _collider.isTrigger = true;// Se o personagem colidir com o castor, torna o collider um trigger para permitir que ele entre na corrente sem colidir fisicamente
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _WaterVisual.SetActive(false);// Desativa o visual da água quando um objeto sai do trigger, indicando que a corrente está inativa
        if (other.CompareTag("Castor"))
        {
            _collider.isTrigger = false;// Se o personagem sair do trigger, torna o collider não ser mais um trigger para permitir que ele colida normalmente com outros objetos novamente
        }

    }
}
