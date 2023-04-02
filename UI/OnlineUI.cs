using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class OnlineUI : MonoBehaviour
{
   [SerializeField] private InputField nickNameInputField;
   [SerializeField] private GameObject createRoomUI;

   public void OnClickCreateRoomBT()
   {
      if (nickNameInputField.text != "")
      {
         PlayerSettings.nickName = nickNameInputField.text;
         createRoomUI.SetActive(true);
         gameObject.SetActive(false);
      }
      else
      {
         nickNameInputField.GetComponent<Animator>().SetTrigger("On");
      }
   }

   public void OnClickEnterGameRoomButton()
   {
      
      if (nickNameInputField.text != "")
      {
         //클라이언트 시작
         PlayerSettings.nickName = nickNameInputField.text;
         var manager = AmongUsRoomManager.singleton;
         manager.StartClient();
      }
      else
      {
         nickNameInputField.GetComponent<Animator>().SetTrigger("On");
      }
   }
}
