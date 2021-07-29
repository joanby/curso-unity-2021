using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyHUD : MonoBehaviour
{
  [SerializeField] private Text messageText;
  private PartyMemberHUD[] memberHuds;

  private List<Pokemon> pokemons;

  public void InitPartyHUD()
  {
    memberHuds = GetComponentsInChildren<PartyMemberHUD>(true);
  }

  public void SetPartyData(List<Pokemon> pokemons)
  {
    this.pokemons = pokemons;
    
    messageText.text = "Selecciona un Pokemon";
    
    for (int i = 0; i < memberHuds.Length; i++)
    {
      if (i < pokemons.Count)
      {
        memberHuds[i].SetPokemonData(pokemons[i]);
        memberHuds[i].gameObject.SetActive(true);
      }
      else
      {
        memberHuds[i].gameObject.SetActive(false);
      }
    }
  }

  public void UpdateSelectedPokemon(int selectedPokemon)
  {
    for (int i = 0; i < pokemons.Count; i++)
    {
      memberHuds[i].SetSelectedPokemon(i == selectedPokemon);
    }
  }

  public void SetMessage(string message)
  {
    messageText.text = message;
  }

  
}
