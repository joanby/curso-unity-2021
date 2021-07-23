using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
  public PokemonBase _base;
  public int _level;
  public bool isPlayer;
  
  public Pokemon Pokemon { get; set; }

  private Image pokemonImage;
  private Vector3 initialPosition;
  
  private void Awake()
  {
    pokemonImage = GetComponent<Image>();
    initialPosition = pokemonImage.transform.localPosition;
  }

  public void SetupPokemon()
  {
    Pokemon = new Pokemon(_base, _level);

    pokemonImage.sprite = 
      (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
    PlayStartAnimation();
  }

  public void PlayStartAnimation()
  {
    pokemonImage.transform.localPosition = 
      new Vector3(initialPosition.x+(isPlayer?-1:1)*400, initialPosition.y);

    pokemonImage.transform.DOLocalMoveX(initialPosition.x, 1.0f);
  }

  public void PlayAttackAnimation()
  {
    
  }

  public void PlayFaintAnimation()
  {
    
  }
}
