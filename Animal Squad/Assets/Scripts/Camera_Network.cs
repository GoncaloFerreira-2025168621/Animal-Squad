using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera_Network : NetworkBehaviour
{
    public Transform _target;

    public float _distance = 3f;
    public float _mouseSensitivity = 3f;

    public float _minY = -20f;
    public float _maxY = 60f;

    public Vector3 _offset = new Vector3(0, 0.7f, 0);

    private float _yaw;
    private float _pitch;

    [SerializeField] private AnimalSelection _selectionAnimal;

    public override void OnNetworkSpawn()
    {
 
        if (!IsClient) return;

        DistanceCamera(_selectionAnimal._selectedAnimal);
    }
    void LateUpdate()
    {
        if (_target == null) return;

        _yaw += Input.GetAxis("Mouse X") * _mouseSensitivity;// Atualiza o valor do yaw com base no movimento horizontal do mouse multiplicado pela sensibilidade
        _pitch -= Input.GetAxis("Mouse Y") * _mouseSensitivity;// Atualiza o valor do pitch com base no movimento vertical do mouse multiplicado pela sensibilidade (subtraímos para inverter o controle vertical)
        _pitch = Mathf.Clamp(_pitch, _minY, _maxY);// Limita o valor do pitch para evitar que a câmera gire demais para cima ou para baixo

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);// Cria uma rotação a partir dos valores de pitch e yaw para orientar a câmera na direção correta

        Vector3 targetPosition = _target.position + _offset;// Calcula a posição alvo da câmera com base na posição do alvo e no offset definido

        transform.position = targetPosition - rotation * Vector3.forward * _distance;
        transform.rotation = rotation;
    }

    private void DistanceCamera(int animalIndex)
    {
        switch (animalIndex)
        {
            case 0:// Se o animal selecionado for o urso
                _distance = 8f;
                break;
            case 1:// Se o animal selecionado for o castor
                _distance = 5f;
                break;
            case 2:// Se o animal selecionado for o rato
                _distance = 4f;
                break;
            case 3:// Se o animal selecionado for o pássaro
                _distance = 4f;
                break;
            default:
                _distance = 3f;
                break;
        }
    }
}
