using UnityEngine;

public class RiverCurret : MonoBehaviour
{
    [Header("Força da corrente")]
    public float strength = 6f;

    [Header("Estado da corrente")]
    public bool startsActive = false;

    private bool activeCurrent;

    private void Start()
    {
        activeCurrent = startsActive;
    }

    public void ActivateCurrent()// Método para ativar a corrente
    {
        activeCurrent = true;
    }

    public void DeactivateCurrent()// Método para desativar a corrente
    {
        activeCurrent = false;
    }

    private void OnTriggerStay(Collider other)// Aplica a força da corrente ao objeto que estiver dentro do trigger
    {
        if (!activeCurrent) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb == null) return;

        rb.AddForce(transform.forward * strength, ForceMode.Acceleration);
    }
}
