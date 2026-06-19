using UnityEngine;

[CreateAssetMenu(fileName = "SaveAnimal", menuName = "Scriptable Objects/SaveAnimal")]
public class SaveAnimal : ScriptableObject
{
    [SerializeField] public int _AnimalSelect;
}
