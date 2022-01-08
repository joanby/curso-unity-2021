using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
   [SerializeField] private string trainerName;
   [SerializeField] private Sprite trainerSprite;
   [SerializeField] private Dialog dialog, afterLoseDialog;
   [SerializeField] private GameObject exclamationMessage;
   [SerializeField] private GameObject fov;
   private Character character;

   private bool trainerLostBattle = false;
   
   public string TrainerName => trainerName;

   public Sprite TrainerSprite => trainerSprite;
   
   private void Awake()
   {
      character = GetComponent<Character>();
   }

   private void Start()
   {
      SetFovDirection(character.Animator.DefaultDirection);
   }

   private void Update()
   {
      character.HandleUpdate();
   }

   IEnumerator ShowExclamationMark()
   {
      exclamationMessage.SetActive(true);
      yield return new WaitForSeconds(0.6f);
      exclamationMessage.SetActive(false);
   }
   public IEnumerator TriggerTrainerBattle(PlayerController player)
   {
      yield return ShowExclamationMark();
      
      var diff = player.transform.position - transform.position;
      var moveVect = diff - diff.normalized;
      moveVect = new Vector2(Mathf.RoundToInt(moveVect.x), Mathf.RoundToInt(moveVect.y));
      yield return character.MoveTowards(moveVect);
      
      DialogManager.SharedInstance.ShowDialog(dialog, () =>
      {
         GameManager.SharedInstance.StartTrainerBattle(this);
      });
   }

   public void SetFovDirection(FacingDirection direction)
   {
      float angle = 0f;
      if (direction == FacingDirection.Right)
      {
         angle = 90f;
      }else if (direction == FacingDirection.Up)
      {
         angle = 180f;
      }else if (direction == FacingDirection.Left)
      {
         angle = 270f;
      }

      fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
   }

   public void Interact(Vector3 source)
   {
      if (!trainerLostBattle)
      {
         StartCoroutine(ShowExclamationMark());
      }
      
      character.LookTowards(source);

      if (!trainerLostBattle)
      {
         DialogManager.SharedInstance.ShowDialog(dialog,
            () => { GameManager.SharedInstance.StartTrainerBattle(this); });
      }
      else
      {
         DialogManager.SharedInstance.ShowDialog(afterLoseDialog);
      }
   }

   public void AfterTrainerLostBattle()
   {
      trainerLostBattle = true;
      fov.gameObject.SetActive(false);
   }
}
