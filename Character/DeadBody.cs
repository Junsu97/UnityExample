using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class DeadBody : NetworkBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private EPlayerColor deadBodyColor;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    [ClientRpc]
    public void RpcSetColor(EPlayerColor color)
    {
        deadBodyColor = color;
        _spriteRenderer.material.SetColor("_PlayerColor",PlayerColor.GetColor(color));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        var player = col.GetComponent<InGameCharacterMover>();
        if (player != null && player.hasAuthority && (player.playerType & EPlayerType.Ghost) != EPlayerType.Ghost)
        {
            InGameUIManager.Instance.ReportButtonUI.SetInteractable(true);
            var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
            myCharacter.foundDeadbodyColor = deadBodyColor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponent<InGameCharacterMover>();
        if (player != null && player.hasAuthority && (player.playerType & EPlayerType.Ghost) != EPlayerType.Ghost)
        {
            InGameUIManager.Instance.ReportButtonUI.SetInteractable(false);
        }
    }
}
