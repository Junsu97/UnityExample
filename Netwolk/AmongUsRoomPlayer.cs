using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class AmongUsRoomPlayer : NetworkRoomPlayer
{
   private static AmongUsRoomPlayer myRoomPlayer;

   public static AmongUsRoomPlayer MyRoomPlayer
   {
      get
      {
         if (myRoomPlayer == null)
         {
            var _players = FindObjectsOfType<AmongUsRoomPlayer>();
            foreach (var player in _players)
            {
               if (player.hasAuthority)
               {
                  myRoomPlayer = player;
               }
            }
         }

         return myRoomPlayer;
      }
   }

   [SyncVar(hook = nameof(SetPlayerColor_Hook))] // SyncVar = 네트워크 동기화
   public EPlayerColor playerColor;
   [SyncVar]
   public string nickname;
   public void Start()
   {
      base.Start();
      if (isServer)
      {
         SpawnLobbyPlayerCharacter();
         LobbyUIManager.Instance.ActiveStartButton();
      }

      if (isLocalPlayer)
      {
         CmdSetNickname(PlayerSettings.nickName);
      }
      LobbyUIManager.Instance.GameRoomPlayerCounter.UpdatePlayerCount();
   }

   private void OnDestroy()
   {
      if (LobbyUIManager.Instance != null)
      {
         LobbyUIManager.Instance.GameRoomPlayerCounter.UpdatePlayerCount();
         LobbyUIManager.Instance.CustomizeUI.UpdateUnSelectColorButton(playerColor);
      }
   }
   
   public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor)
   {
      LobbyUIManager.Instance.CustomizeUI.UpdateUnSelectColorButton(oldColor);
      LobbyUIManager.Instance.CustomizeUI.UpdateSelectColorButton(newColor);
   }

   public CharacterMover myCharacter;
   
   // 클라이언트에서 서버로 색변경 정보를 보내는 함수
   [Command]// Command == Mirror API 제공 어트리뷰트
   // 클라이언트에서 함수를 호출하면 함수 내부의 동작이 서버에서 실행되도록 만들어주는 것
   // Command 어트리뷰트를 사용하는 함수는 반드시 'Cmd'를 붙여야한다
   public void CmdSetPlayerColor(EPlayerColor color)
   {
      playerColor = color;
      myCharacter.playerColor = color;
   }
   [Command]
   public void CmdSetNickname(string nick)
   {
      nickname = nick;
      myCharacter.nickname = nick;
   }

   private void SpawnLobbyPlayerCharacter()
   {
      var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
      EPlayerColor color = EPlayerColor.Red;
      
      // RoomPlayer들이 들어있는 roomSlots 를 순회하면서 플레이어들이 사용하지 않는 색을 고르게함
      for (int i = 0; i < (int) EPlayerColor.Lime + 1; i++)
      {
         bool isFindSameColor = false;

         foreach (var roomPlayer in roomSlots)
         {
            var amongUsRoomPlayer = roomPlayer as AmongUsRoomPlayer;
            if (amongUsRoomPlayer.playerColor == (EPlayerColor) i && roomPlayer.netId != netId)
            {
               isFindSameColor = true;
               break;
            }
         }

         if (!isFindSameColor)
         {
            color = (EPlayerColor)i;
            break;
         }
      }
      // 색상을 고른후 자신의 플레이어 컬러에 저장
      playerColor = color;
      var spawnPostions =  FindObjectOfType<SpawnPostions>();
      int idx = spawnPostions.Index;
      Vector3 spawnPos = spawnPostions.GetSpawnPos();
      
      var player = Instantiate(AmongUsRoomManager.singleton.spawnPrefabs[0],spawnPos, Quaternion.identity).GetComponent<LobbyCharacterMover>();
      player.transform.localScale = idx < 5 ? new Vector3(0.5f, 0.5f, 1f) : new Vector3(-0.5f, 0.5f, 1f);
      NetworkServer.Spawn(player.gameObject,connectionToClient);
      player.ownerNetId = netId;
      player.playerColor = color;
   }
}
