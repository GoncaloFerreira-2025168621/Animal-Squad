using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera_Network : MonoBehaviour
{
    public Transform _target;// O alvo que a câmera irá seguir (geralmente o jogador)

    public float _distance = 5f;
    public float _mouseSensitivity = 3f;

    public float _minY = -20f;
    public float _maxY = 60f;

    public Vector3 _offset = new Vector3(0, 2f, 0);// Um offset para ajustar a altura da câmera em relação ao alvo

    private float _yaw;
    private float _pitch;

    void LateUpdate()
    {

        if (_target == null) return;

        _yaw += Input.GetAxis("Mouse X") * _mouseSensitivity;// Atualiza o yaw com base no movimento horizontal do mouse
        _pitch -= Input.GetAxis("Mouse Y") * _mouseSensitivity;// Atualiza o pitch com base no movimento vertical do mouse
        _pitch = Mathf.Clamp(_pitch, _minY, _maxY);// Limita o pitch para evitar que a câmera gire demais para cima ou para baixo

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);// Cria uma rotação a partir do pitch e yaw calculados

        Vector3 targetPosition = _target.position + _offset;// Calcula a posição do alvo com o offset aplicado

        transform.position = targetPosition - rotation * Vector3.forward * _distance;// Calcula a posição da câmera com base na rotação e distância
        transform.rotation = rotation;// Aplica a rotação à câmera
    }
}
