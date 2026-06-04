using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Life_Object", menuName = "Scriptable Objects/Life_Object")]
public class Life_Object_Scriptable : ScriptableObject
{
    public float _maxHealth;
    public GameObject _Object_Efect; // Prefab do efeito de morte

    //mostrar as variaveis do scriptable no inspector



}
