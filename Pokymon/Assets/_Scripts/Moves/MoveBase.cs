using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Nuevo Movimiento")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;
    public string Name => name;
    
    [TextArea] [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;
    
    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;

    public bool IsSpecialMove
    {
        get
        {
            if (type == PokemonType.Fire || type == PokemonType.Water ||
                type == PokemonType.Grass || type == PokemonType.Ice ||
                type == PokemonType.Electric || type == PokemonType.Dragon ||
                type == PokemonType.Dark || type == PokemonType.Psychic)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
