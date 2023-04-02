using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class AmongUsRoomManager : NetworkRoomManager
{
   public GameRuleData gameRuleData;
   
   public int minPlayerCount;
   public int imposterCount;
   
   //OnRoomServerConnect 함수는 서버에 접속한 클라이언트를 감지하는 함수
   // RoomPlayer가 생성되기 이전이다.
   public override void OnRoomServerConnect(NetworkConnection conn)
   {
      base.OnRoomServerConnect(conn);
   }
}
