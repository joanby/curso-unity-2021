using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get => pokemons;
    }

    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.InitPokemon();
        }
    }

    public Pokemon GetFirstNonFaintedPokemon()
    {
        return pokemons.Where(p => p.HP > 0).FirstOrDefault();
    }

    public int GetPositionFromPokemon(Pokemon pokemon)
    {
        for (int i = 0; i < Pokemons.Count; i++)
        {
            if (pokemon == Pokemons[i])
            {
                return i;
            }
        }

        return -1;
    }
    
}
