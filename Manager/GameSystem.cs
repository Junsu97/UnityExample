using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;
using UnityEngine.Experimental.Rendering.Universal;
public class GameSystem : NetworkBehaviour
{
    public static GameSystem Instance;
    // 네트워크 게임에서 GameSystem의 Start함수가 호출되는 시점에 각 플레이어의 객체가 모두 생성되었는가가 보장되지않음
    // 그렇기 때문에 플레이어 객체가 스스로 GameSystem을 찾아 자신을 등록하도록 함 
    public List<InGameCharacterMover> players = new List<InGameCharacterMover>();

    [SerializeField]private Transform spawnTransform;
    [SerializeField] private float spawnDistance;
    
    [SyncVar]
    public float killCooldown;
    [SyncVar]
    public int killRange;
    
    [SyncVar] 
    public float remainTime;
    
    [SyncVar] 
    public int skipVotePlayerCount;

    [SerializeField] private Light2D shadowLight;
    [SerializeField] private Light2D lightmapLight;
    [SerializeField] private Light2D globalLight;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (isServer)
        {
            StartCoroutine(GameReady());
        }
    }

    public void AddPlayer(InGameCharacterMover player)
    {
        if (!players.Contains(player))
        { 
            players.Add(player);
        }
    }

    private IEnumerator GameReady() 
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        killCooldown = manager.gameRuleData.killCooldown;
        killRange = (int)manager.gameRuleData.KillRange;
        while (manager.roomSlots.Count != players.Count)
        {
            yield return null;
        }

        for (int i = 0; i < manager.imposterCount; i++)
        {
            var player = players[Random.Range(0,players.Count)];
            if (player.playerType != EPlayerType.Imposter)
            {
                player.playerType = EPlayerType.Imposter;
            }
            else
            {
                i--;
            }
        }

        AllocatePlayerToAroundTable(players.ToArray());

        yield return new WaitForSeconds(2f);
        RpcStartGame();
        foreach (var player in players)
        {
            player.SetKillCoolDown();
        }
    }

    private void AllocatePlayerToAroundTable(InGameCharacterMover[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            float radian = (2f * Mathf.PI) / players.Length;
            radian += i;
            players[i].RpcTeleport(spawnTransform.position +
                                   (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * spawnDistance));
        }
    }

    [ClientRpc]
    private void RpcStartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return StartCoroutine(InGameUIManager.Instance.InGameIntroUI.ShowIntroSequence());
        InGameCharacterMover myCharacter = null;
        foreach (var player in players)
        {
            if (player.hasAuthority)
            {
                myCharacter = player;
                break;
            }   
        }

        foreach (var player in players)
        {
            player.SetNickNameColor(myCharacter.playerType);
        }
       
        yield return new WaitForSeconds(3f);
        InGameUIManager.Instance.InGameIntroUI.Close();
    }

    public List<InGameCharacterMover> GetPlayerList()
    {
        return players;
    }

    public void ChangeLightMode(EPlayerType type)
    {
        if (type == EPlayerType.Ghost)
        {
            lightmapLight.lightType = Light2D.LightType.Global;
            shadowLight.intensity = 0f;
            globalLight.intensity = 1f;
        }
        else
        {
            lightmapLight.lightType = Light2D.LightType.Point;
            shadowLight.intensity = 0.5f;
            globalLight.intensity = 0.5f;
        }
    }

    public void StartReportMeeting(EPlayerColor deadbodyColor)
    {
        RpcSendReportSign(deadbodyColor);
        StartCoroutine(MeetingProcess_Coroutine());
    }
    [ClientRpc]
    public void RpcSendReportSign(EPlayerColor deadbodyColor)
    {
        InGameUIManager.Instance.ReportUI.Open(deadbodyColor);

        StartCoroutine(StartMeeting_Coroutine());
    }

    private IEnumerator StartMeeting_Coroutine()
    {
        yield return new WaitForSeconds(3f);
        InGameUIManager.Instance.ReportUI.Close();
        InGameUIManager.Instance.MeetingUI.Open();
        InGameUIManager.Instance.MeetingUI.ChangeMeetingState(EMeetingState.Meeting);
    }
    [ClientRpc]
    public void RpcSignVoteEject(EPlayerColor voterColor, EPlayerColor ejectColor)
    {
        InGameUIManager.Instance.MeetingUI.UpdateVote(voterColor,ejectColor);
    }
    [ClientRpc]
    public void RpcSignSkipVote(EPlayerColor skipVotePlayerColor)
    {
        InGameUIManager.Instance.MeetingUI.UpdateSkipVotePlayer(skipVotePlayerColor);
    }

    [ClientRpc]
    public void RpcStartVoteTime()
    {
        InGameUIManager.Instance.MeetingUI.ChangeMeetingState(EMeetingState.Vote);
    }
    [ClientRpc]
    public void RpcEndVoteTime()
    {
        InGameUIManager.Instance.MeetingUI.CompleteVote();   
    }

    private IEnumerator MeetingProcess_Coroutine()
    {
        var players = FindObjectsOfType<InGameCharacterMover>();
        foreach (var player in players)
        {
            player.isVote = true;
        }

        yield return new WaitForSeconds(3f);
        
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        remainTime = manager.gameRuleData.meetingsTime;
        while (true)
        {
            remainTime -= Time.deltaTime;
            yield return null;
            if (remainTime <= 0f)
            {
                break;
            }
        }

        skipVotePlayerCount = 0;
        foreach (var player in players)
        {
            if ((player.playerType & EPlayerType.Ghost) != EPlayerType.Ghost)
            {
                player.isVote = false;
            }

            player.vote = 0;
        }

        RpcStartVoteTime();
        remainTime = manager.gameRuleData.voteTime;
        while (true)
        {
            remainTime -= Time.deltaTime;
            yield return null;
            if (remainTime <= 0f)
            {
                break;
            }
        }

        foreach (var player in players)
        {
            if (!player.isVote && (player.playerType & EPlayerType.Ghost) != EPlayerType.Ghost)
            {
                player.isVote = true;
                skipVotePlayerCount += 1;
                RpcSignSkipVote(player.playerColor);
            }
        }
        RpcEndVoteTime();

        yield return new WaitForSeconds(3f);

        StartCoroutine(CalculateVoteResult_Coroutin(players));
    }

    private class CharacterVoteComparer : IComparer // 배열을 빠르게 정리 가능
    {
        public int Compare(object x, object y)
        {
            InGameCharacterMover xPlayer = (InGameCharacterMover) x;
            InGameCharacterMover yPlayer = (InGameCharacterMover) y;
            return xPlayer.vote <= yPlayer.vote ? 1 : -1;
        }
    }

    private IEnumerator CalculateVoteResult_Coroutin(InGameCharacterMover[] players)// 투표결과 계산
    {
        System.Array.Sort(players, new CharacterVoteComparer());
        int remainInposter = 0;
        foreach (var player in players)
        {
            if ((player.playerType & EPlayerType.Imposter_Alive) == EPlayerType.Imposter_Alive)
            {
                remainInposter++;
            }
        }

        if (skipVotePlayerCount >= players[0].vote)
        {
            RpcOpenEjectionUI(false,EPlayerColor.Black,false,remainInposter);
        }
        else if (players[0].vote == this.players[1].vote)
        {
            RpcOpenEjectionUI(false,EPlayerColor.Black,false,remainInposter);
        }
        else
        {
            bool isImposter = (players[0].playerType & EPlayerType.Imposter) == EPlayerType.Imposter;
            RpcOpenEjectionUI(true,players[0].playerColor, isImposter,isImposter? remainInposter - 1 : remainInposter);
            
            players[0].Dead(true);
        }

        var deadbodies = FindObjectsOfType<DeadBody>();
        for (int i = 0; i < deadbodies.Length; i++)
        {
            Destroy(deadbodies[i].gameObject);
        }

        AllocatePlayerToAroundTable(players);

        yield return new WaitForSeconds(10f);
        RpcCloseEjetionUI();
    } 
    [ClientRpc]
    public void RpcOpenEjectionUI(bool isEjection, EPlayerColor ejectionPlayerColor, bool isImposter,
        int remainImposterCount)
    {
        InGameUIManager.Instance.EjectionUI.Open(isEjection,ejectionPlayerColor,isImposter,remainImposterCount);
        InGameUIManager.Instance.MeetingUI.Close();
    }

    [ClientRpc]
    public void RpcCloseEjetionUI()
    {
        InGameUIManager.Instance.EjectionUI.Close();
        AmongUsRoomPlayer.MyRoomPlayer.myCharacter.IsMovable = true;
    }
}
