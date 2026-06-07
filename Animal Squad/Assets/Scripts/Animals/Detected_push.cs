using UnityEngine;

public class Detected_push : MonoBehaviour
{
    [Header("Tipo do animal")]
    [SerializeField] private string _Animal_Type = "Bear";

    [Header("Transform do animal")]
    [SerializeField] private Transform _Animal_Transform;// Transform do animal para saber a direção de empurrar

    private void OnTriggerStay(Collider _Other)//Detecta o objeto para empurrar
    {
        Move_Object _Move_Object = _Other.GetComponent<Move_Object>();// Verifica se o objeto tem o script Move_Object para empurrar

        if (_Move_Object == null)
            return;

        Vector3 _Direction = Vector3.ProjectOnPlane(_Animal_Transform.forward, Vector3.up); // Projeta a direção do animal no plano horizontal
        _Direction.Normalize();

        _Move_Object.TryPushServerRpc(_Animal_Type, _Direction);
    }


}
