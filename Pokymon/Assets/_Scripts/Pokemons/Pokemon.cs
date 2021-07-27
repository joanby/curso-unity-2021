using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public class Pokemon
{
    
    [SerializeField] private PokemonBase _base;

    public PokemonBase Base
    {
        get => _base;
    }

    [SerializeField] private int _level;

    public int Level
    {
        get => _level;
        set => _level = value;
    }

    private List<Move> _moves;

    public List<Move> Moves
    {
        get => _moves;
        set => _moves = value;
    }

    //Vida actual del Pokemon
    private int _hp;

    public int HP
    {
        get => _hp;
        set => _hp = value;
    }

    public void InitPokemon()
    {
      
        _hp = MaxHP;
        
        _moves = new List<Move>();

        foreach (var lMove in _base.LearnableMoves)
        {
            if (lMove.Level <= _level)
            {
                _moves.Add(new Move(lMove.Move));
            }

            if (_moves.Count >= 4)
            {
                break;
            }
        }
    }

    public int MaxHP => Mathf.FloorToInt((_base.MaxHP*_level)/20.0f)+10;
    public int Attack => Mathf.FloorToInt((_base.Attack*_level)/100.0f)+2;
    public int Defense => Mathf.FloorToInt((_base.Defense*_level)/100.0f)+2;
    public int SpAttack => Mathf.FloorToInt((_base.SpAttack*_level)/100.0f)+2;
    public int SpDefense => Mathf.FloorToInt((_base.SpDefense*_level)/100.0f)+2;
    public int Speed => Mathf.FloorToInt((_base.Speed*_level)/100.0f)+2;


    public DamageDescription ReceiveDamage(Pokemon attacker, Move move)
    {
        float critical = 1f;
        if (Random.Range(0f, 100f) < 8f)
        {
            critical = 2f;
        }
        
        float type1 = TypeMatrix.GetMultEffectiveness(move.Base.Type, this.Base.Type1);
        float type2 = TypeMatrix.GetMultEffectiveness(move.Base.Type, this.Base.Type2);
        
        var damageDesc = new DamageDescription()
        {
            Critical = critical,
            Type = type1*type2,
            Fainted = false
        };

        float attack = (move.Base.IsSpecialMove ? attacker.SpAttack : attacker.Attack);
        float defense = (move.Base.IsSpecialMove ? this.SpDefense : this.Defense);

        float modifiers = Random.Range(0.85f, 1.0f) * type1 * type2 * critical;
        float baseDamage = ((2 * attacker.Level / 5f + 2) * move.Base.Power * ((float) attack/defense)) /
            50f + 2;
        int totalDamage = Mathf.FloorToInt(baseDamage * modifiers);

        HP -= totalDamage;
        if (HP<=0)
        {
            HP = 0;
            damageDesc.Fainted = true;
        }

        return damageDesc;
        
    }

    public Move RandomMove()
    {
        int randId = Random.Range(0, Moves.Count);
        return Moves[randId];
    }
    
    
}


public class DamageDescription
{
    public float Critical { get; set; }
    public float Type { get; set; } 
    public bool Fainted { get; set; }
}
