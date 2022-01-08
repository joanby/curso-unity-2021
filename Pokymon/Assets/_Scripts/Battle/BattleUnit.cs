using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
  public PokemonBase _base;
  public int _level;
  [SerializeField] bool isPlayer;
  [SerializeField] BattleHUD hud;
  
  public Pokemon Pokemon { get; set; }

  public bool IsPlayer => isPlayer;

  public BattleHUD Hud => hud;

  private Image pokemonImage;
  private Vector3 initialPosition;
  private Color initialColor;
  
  [SerializeField] private float startTimeAnim = 1.0f, attackTimeAnim=0.3f,
                                 hitTimeAnim = 0.15f, dieTimeAnim=1f, 
                                 capturedTimeAnim = 0.6f;

  private void Awake()
  {
    pokemonImage = GetComponent<Image>();
    initialPosition = pokemonImage.transform.localPosition;
    initialColor = pokemonImage.color;
  }

  public void SetupPokemon(Pokemon pokemon)
  {
    Pokemon = pokemon;
    
    pokemonImage.sprite = 
      (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
    pokemonImage.color = initialColor;
    
    hud.gameObject.SetActive(true);
    hud.SetPokemonData(pokemon);
    transform.localScale = new Vector3(1,1,1);
    
    PlayStartAnimation();
  }

  public void ClearHUD()
  {
    hud.gameObject.SetActive(false);
  }

  public void PlayStartAnimation()
  {
    pokemonImage.transform.localPosition = 
      new Vector3(initialPosition.x+(isPlayer?-1:1)*400, initialPosition.y);

    pokemonImage.transform.DOLocalMoveX(initialPosition.x, startTimeAnim);
  }

  public void PlayAttackAnimation()
  {
    var seq = DOTween.Sequence();
    seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x + (isPlayer ? 1 : -1) * 60, attackTimeAnim));
    seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x, attackTimeAnim));
  }

  public void PlayReceiveAttackAnimation()
  {
    var seq = DOTween.Sequence();
    seq.Append(pokemonImage.DOColor(Color.grey, hitTimeAnim));
    seq.Append(pokemonImage.DOColor(initialColor, hitTimeAnim));
  }

  public void PlayFaintAnimation()
  {
    var seq = DOTween.Sequence();
    seq.Append(pokemonImage.transform.DOLocalMoveY(initialPosition.y - 200, dieTimeAnim));
    seq.Join(pokemonImage.DOFade(0f, dieTimeAnim));
  }

  public IEnumerator PlayCapturedAnimation()
  {
    var seq = DOTween.Sequence();
    seq.Append(pokemonImage.DOFade(0, capturedTimeAnim));
    seq.Join(transform.DOScale(new Vector3(0.25f, 0.25f, 1f), capturedTimeAnim));
    seq.Join(transform.DOLocalMoveY(initialPosition.y + 50f, capturedTimeAnim));
    yield return seq.WaitForCompletion();
  }
  
  public IEnumerator PlayBreakOutAnimation()
  {
    var seq = DOTween.Sequence();
    seq.Append(pokemonImage.DOFade(1, capturedTimeAnim));
    seq.Join(transform.DOScale(new Vector3(1f, 1f, 1f), capturedTimeAnim));
    seq.Join(transform.DOLocalMoveY(initialPosition.y, capturedTimeAnim));
    yield return seq.WaitForCompletion();
  }
}
