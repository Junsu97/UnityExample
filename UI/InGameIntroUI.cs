using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
public class InGameIntroUI : MonoBehaviour
{
   [SerializeField] private GameObject shhhhObj;
   [SerializeField] private GameObject crewmateObj;
   
   [SerializeField] private Text playerType;
   [SerializeField] private Image gradientImg;
   [SerializeField] private IntroCharacter myCharacter;
   [SerializeField] private List<IntroCharacter> otherCharacters = new List<IntroCharacter>();
   [SerializeField] private Color crewColor;
   [SerializeField] private Color imposterColor;

   [SerializeField] private CanvasGroup _canvasGroup;

   public IEnumerator ShowIntroSequence()
   {
      shhhhObj.SetActive(true);
      yield return new WaitForSeconds(3f);
      shhhhObj.SetActive(false);
      
      ShowPlayerType();
      crewmateObj.SetActive(true);
   }
   
   public void ShowPlayerType()
   {
      //GameSystem에서 플레이어 리스트를 가져와 자신의 인트로 캐릭터를 세팅한 다음
      //임포스터 여부에따라 임포스터일 때만 임포스터만 UI를 보이고 크루원일때는 모든 플레이어가 UI에 보이도록 배치함
      var players = GameSystem.Instance.GetPlayerList();
      InGameCharacterMover myPlayer = null;
      foreach (var player in players)
      {
         if (player.hasAuthority)
         {
            myPlayer = player;
            break;
         }
      }
      myCharacter.SetIntroCharacter(myPlayer.nickname,myPlayer.playerColor);
      if (myPlayer.playerType == EPlayerType.Imposter)
      {
         playerType.text = "임포스터";
         playerType.color = gradientImg.color = imposterColor;

         int i = 0;
         foreach (var player in players)
         {
            if (!player.hasAuthority && player.playerType == EPlayerType.Imposter)
            {
               otherCharacters[i].SetIntroCharacter(player.nickname,player.playerColor);
               otherCharacters[i].gameObject.SetActive(true);
               i++;
            }
         }
      }
      else
      {
         playerType.text = "크루원";
         playerType.color = gradientImg.color = crewColor;

         int i = 0;
         foreach (var player in players)
         {
            if (!player.hasAuthority)
            {
               otherCharacters[i].SetIntroCharacter(player.nickname,player.playerColor);
               otherCharacters[i].gameObject.SetActive(true);
               i++;
            }
         }
      }
   }

   public void Close()
   {
      StartCoroutine(FadeOut());
   }

   private IEnumerator FadeOut()
   {
      float timer = 0f;
      while (timer <= 1f)
      {
         yield return null;
         timer += Time.deltaTime;
         _canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
      }
      
      gameObject.SetActive(false);
   }
}
