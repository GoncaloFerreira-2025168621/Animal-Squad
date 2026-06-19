using UnityEngine;

public class TestBreak : MonoBehaviour
{
    [SerializeField] private Barragem_Partida _Barragem;
    [SerializeField] private GameObject _CheckBarragens;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            _Barragem.BreakDam();
        }

        if (_CheckBarragens == false)// Verifica se o objeto de verificação das barragens está desativado, o que indica que as barragens foram quebradas
        {
            _Barragem.BreakDam();
        }
    }
}
