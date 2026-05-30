using Unity.Netcode;
using UnityEngine;

public class PlayerCameraSetup : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawnou: " + gameObject.name + " | IsOwner: " + IsOwner);

        if (!IsOwner) return;

        Camera mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogError("Não existe Camera.main! Verifica se a câmera tem a tag MainCamera.");
            return;
        }

        Camera_Network cam = mainCam.GetComponent<Camera_Network>();

        if (cam == null)
        {
            Debug.LogError("A Main Camera não tem o script Camera_Network.");
            return;
        }

        cam._target = transform;

        Mov_Network movement = GetComponent<Mov_Network>();

        if (movement == null)
        {
            Debug.LogError("Este player não tem Mov_Network.");
            return;
        }

        movement._cameraTransform = Camera.main.transform;

        Debug.Log("Camera vinculada ao player: " + gameObject.name);
    }
}
