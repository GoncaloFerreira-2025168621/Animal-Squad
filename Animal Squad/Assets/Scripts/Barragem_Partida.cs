using UnityEngine;

public class Barragem_Partida : MonoBehaviour
{
    [Header("Barragem")]
    public GameObject _damNormal;
    public GameObject _damBroken;

    [Header("Água visual")]
    public GameObject _waterCalm;
    public GameObject _waterFlowing;

    [Header("Triggers da corrente")]
    public RiverCurret[] _riverCurrents;

    [Header("Efeitos")]
    public GameObject _breakEffect;
    public AudioSource _waterSound;

    private bool _isBroken = false;

    // Método para quebrar a barragem
    public void BreakDam()
    {
        if (_isBroken) return;

        _isBroken = true;

        if (_damNormal != null)// Desativa a barragem normal
            _damNormal.SetActive(false);

        if (_damBroken != null)// Ativa a barragem quebrada
            _damBroken.SetActive(true);

        if (_waterCalm != null)// Desativa a água calma
            _waterCalm.SetActive(false);

        if (_waterFlowing != null)// Ativa a água fluindo
            _waterFlowing.SetActive(true);

        
        foreach (RiverCurret current in _riverCurrents)// Ativa cada corrente do rio
        {
            if (current != null)
                current.ActivateCurrent();
        }

        if (_breakEffect != null)// Instancia o efeito de quebra da barragem
            Instantiate(_breakEffect, transform.position, Quaternion.identity);

        if (_waterSound != null)// Toca o som da água fluindo
            _waterSound.Play();
    }
}
