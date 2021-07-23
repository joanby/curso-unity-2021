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

    private Pokemon _pokemon;
    
    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        pokemonName.text = pokemon.Base.Name;
        pokemonLevel.text = $"Lv {pokemon.Level}";
        //Si con el Update se ve mal, actualizar vida aqui al inicio de batalla.
        UpdatePokemonData();
    }

    public void UpdatePokemonData()
    {
        StartCoroutine(healthbar.SetSmoothHP((float)_pokemon.HP / _pokemon.MaxHP));
        pokemonHealth.text = $"{_pokemon.HP}/{_pokemon.MaxHP}";
    }
}
