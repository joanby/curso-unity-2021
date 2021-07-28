using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        healthbar.SetHP((float)_pokemon.HP / _pokemon.MaxHP);
        StartCoroutine(UpdatePokemonData(pokemon.HP));
    }

    public IEnumerator UpdatePokemonData(int oldHPVal)
    {
        StartCoroutine(healthbar.SetSmoothHP((float)_pokemon.HP / _pokemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPVal));
        yield return null;
    }

    private IEnumerator DecreaseHealthPoints(int oldHPVal)
    {
        while (oldHPVal>_pokemon.HP)
        {
            oldHPVal--;
            pokemonHealth.text = $"{oldHPVal}/{_pokemon.MaxHP}";
            yield return new WaitForSeconds(0.1f);
        }
        pokemonHealth.text = $"{_pokemon.HP}/{_pokemon.MaxHP}";
    }
    
}
