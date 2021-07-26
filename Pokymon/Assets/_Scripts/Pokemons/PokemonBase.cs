using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Nuevo Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private int ID;
    
    [SerializeField] private string name;
    public string Name => name;

    [TextArea] [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    public Sprite FrontSprite => frontSprite;

    public Sprite BackSprite => backSprite;
    
    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;
    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;

    //Stats
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
    
    public int MaxHP => maxHP;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;

    [SerializeField] private List<LearnableMove> learnableMoves;
    
   public List<LearnableMove> LearnableMoves => learnableMoves;
}


public enum PokemonType
{
    None, 
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fight,
    Poison,
    Ground,
    Fly,
    Psychic, 
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark, 
    Steel,
    Fairy
}

public class TypeMatrix
{
    //TODO: completar el resto de la matriz cuando haya tiempo
    private static float[][] matrix =
    {
        //                   NOR  FIR   WAT  ELE  GRA  ICE  FIG  POI  GRO  FLY  PSY  BUG  ROC  GHO  DRA  DAR  STE  FAI
        /*NOR*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,0.5f,  0f,  1f,  1f,0.5f,  1f}, 
        /*FIR*/ new float[] { 1f,0.5f,0.5f,  1f,  2f,  2f,  1f,  1f,  1f,  1f,  1f,  2f,0.5f,  1f,0.5f,  1f,  2f,  1f},
        /*WAT*/ new float[] { 1f,  2f,0.5f,  1f,0.5f,  1f,  1f,  1f,  2f,  1f,  1f,  1f,  2f,  1f,0.5f,  1f,  1f,  1f},
        /*ELE*/ new float[] { 1f,  1f,  2f,0.5f,0.5f,  1f,  1f,  1f,  0f,  2f,  1f,  1f,  1f,  1f,0.5f,  1f,  1f,  1f},
        /*GRA*/ new float[] { 1f,0.5f,  2f,  1f,0.5f,  1f,  1f,0.5f,  2f,0.5f,  1f,0.5f,  2f,  1f,0.5f,  1f,0.5f,  1f},
        /*ICE*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*FIG*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*POI*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*GRO*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*FLY*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*PSY*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*BUG*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*ROC*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*GHO*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*DRA*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*DAR*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*STE*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f},
        /*FAI*/ new float[] { 1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f,  1f}
    };

    public static float GetMultEffectiveness(PokemonType attackType, PokemonType pokemonDefenderType)
    {
        if (attackType == PokemonType.None || pokemonDefenderType == PokemonType.None)
        {
            return 1.0f;
        }

        int row = (int) attackType;
        int col = (int) pokemonDefenderType;

        return matrix[row - 1][col - 1];

    }

}





[Serializable]
public class LearnableMove
{
[SerializeField] private MoveBase move;
[SerializeField] private int level;

public MoveBase Move => move;
public int Level => level;
}