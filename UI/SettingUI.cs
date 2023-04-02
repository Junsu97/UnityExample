using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class SettingUI : MonoBehaviour
{
   [SerializeField] private Button MouseControlBT;
   [SerializeField] private Button KeyboardMouseBT;
   private Animator ani;
   private int hash;
   private void Awake()
   {
      ani = GetComponent<Animator>();
      hash = Animator.StringToHash("Close");
   }

   private void OnEnable()
   {
      switch (PlayerSettings.ControlType)
      {
         case EControlType.Mouse :
            MouseControlBT.image.color = Color.green;
            KeyboardMouseBT.image.color = Color.white;
            break;
         case EControlType.KeyboardMouse :
            MouseControlBT.image.color = Color.white;
            KeyboardMouseBT.image.color = Color.green;
            break;
      }
   }

   public void SetControlMode(int controlType)
   {
      PlayerSettings.ControlType = (EControlType) controlType;
      switch (PlayerSettings.ControlType)
      {
         case EControlType.Mouse :
            MouseControlBT.image.color = Color.green;
            KeyboardMouseBT.image.color = Color.white;
            break;
         case EControlType.KeyboardMouse :
            MouseControlBT.image.color = Color.white;
            KeyboardMouseBT.image.color = Color.green;
            break;
      }
   }

   public virtual void Close()
   {
      StartCoroutine(CloseAfterDelay());
   }

   private IEnumerator CloseAfterDelay()
   {
      ani.SetTrigger(hash);
      yield return new WaitForSeconds(0.5f);
      gameObject.SetActive(false);
      ani.ResetTrigger(hash);
   }
}
