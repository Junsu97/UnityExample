using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KillButtonUI : MonoBehaviour
{
    [SerializeField] private Button killButton;
    [SerializeField] private Text coolDownText;

    private InGameCharacterMover targetPlayer;
    public void Show(InGameCharacterMover player)
    {
        gameObject.SetActive(true);
        targetPlayer = player;
    }

    public void OnCklickKill()
    {
        targetPlayer.Kill();
    }

    private void Update()
    {
        if (targetPlayer != null)
        {
            if (!targetPlayer.isKillable)
            {
                coolDownText.text = targetPlayer.KillCoolDown > 0 ? ((int) targetPlayer.KillCoolDown).ToString() : "";
                killButton.interactable = false;
            }
            else
            {
                coolDownText.text = "";
                killButton.interactable = true;
            }
        }
    }
}
