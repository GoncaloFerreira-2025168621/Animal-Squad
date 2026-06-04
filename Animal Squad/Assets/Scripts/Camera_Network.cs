using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Camera_Network : MonoBehaviour
{
    public Transform _target;

    public float _distance = 3f;
    public float _mouseSensitivity = 3f;

    public float _minY = -20f;
    public float _maxY = 60f;

    public Vector3 _offset = new Vector3(0, 0.7f, 0);

    private float _yaw;
    private float _pitch;

    void LateUpdate()
    {
        if (_target == null) return;

        _yaw += Input.GetAxis("Mouse X") * _mouseSensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * _mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, _minY, _maxY);

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);

        Vector3 targetPosition = _target.position + _offset;

        transform.position = targetPosition - rotation * Vector3.forward * _distance;
        transform.rotation = rotation;
    }
}
