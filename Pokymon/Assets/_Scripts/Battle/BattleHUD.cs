using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text pokemonName;
    public Text pokemonLevel;
    public HealthBar healthbar;
    public Text pokemonHealth;

    public void SetPokemonData(Pokemon pokemon)
    {
        pokemonName.text = pokemon.Base.Name;
        pokemonLevel.text = $"Lv: {pokemon.Level}";
        healthbar.SetHP(pokemon.HP/pokemon.MaxHP);
        pokemonHealth.text = $"{pokemon.HP}/{pokemon.MaxHP}";
    }
}
