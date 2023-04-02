using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

// 0x : 0 - 크루원 1- 임포스터
// x0 : 0 - 살아있음 1- 죽어있음
//ex) 00 : 살아있는 크루원 01: 살아있는 임포스터
public enum EPlayerType
{
    Crew = 0,
    Imposter = 1,
    Ghost = 2,
    Crew_Alive = 0,
    Imposter_Alive = 1,
    Crew_Ghost = 2,
    Imposter_Ghost = 3,
}
public class InGameCharacterMover : CharacterMover
{
    [SyncVar(hook = nameof(SetPlayerType))]
    public EPlayerType playerType;
    public void SetPlayerType(EPlayerType _, EPlayerType type)
    {
        if (hasAuthority && type == EPlayerType.Imposter) 
        {
            InGameUIManager.Instance.KillButtonUI.Show(this);
            _playerFinder.SetKillRange(GameSystem.Instance.killRange * 1f);
        }
    }
    [SyncVar]
    private float killCooldown;
    public float KillCoolDown { get { return killCooldown; } }
    public bool isKillable { get { return killCooldown < 0f && _playerFinder.targets.Count != 0; } }

    [SerializeField] private PlayerFinder _playerFinder;

    public EPlayerColor foundDeadbodyColor;

    [SyncVar]
    public bool isReporter = false;
    [SyncVar]
    public bool isVote;
    [SyncVar]
    public int vote;
    
    public override void Start()
    {
        base.Start();
        if (hasAuthority)
        {
            IsMovable = true;
            
            var myRoomPlayer = AmongUsRoomPlayer.MyRoomPlayer;
            myRoomPlayer.myCharacter = this;
            CmdSetPlayerCharacter(myRoomPlayer.nickname, myRoomPlayer.playerColor);
        }
        
        GameSystem.Instance.AddPlayer(this);
    }
    
    [ClientRpc]
    public void RpcTeleport(Vector3 position)// 스폰위치
    {
        transform.position = position;
    }

    public void SetNickNameColor(EPlayerType type)
    {
        if (playerType == EPlayerType.Imposter && type == EPlayerType.Imposter)
        {
            nicknameText.color = Color.red;
        }
        
    }
    [Command]
    private void CmdSetPlayerCharacter(string nickName, EPlayerColor color)
    {
        this.nickname = nickName;
        playerColor = color;
    }
    public void SetKillCoolDown()
    {
        if (isServer)
        {
            killCooldown = GameSystem.Instance.killCooldown;
        }
    }

    public void Kill()
    {
        CmdKill(_playerFinder.GetFirstTarget().netId);
    }
    [Command]
    public void CmdKill(uint targetNetId)
    {
        InGameCharacterMover target = null;
        foreach (var player in GameSystem.Instance.GetPlayerList())
        {
            if (player.netId == targetNetId)
            {
                target = player;
            }
        }

        if (target != null)
        {
            RpcTeleport(target.transform.position);
            target.Dead(false,playerColor);
            killCooldown = GameSystem.Instance.killCooldown;
        }
    }

    public void Report()
    {
        CmdReport(foundDeadbodyColor);
    }
    [Command]
    public void CmdReport(EPlayerColor deadbodyColor)
    {
        GameSystem.Instance.StartReportMeeting(deadbodyColor);
        isReporter = true;
    }

    public void Dead(bool isEjection,EPlayerColor imposterColor = EPlayerColor.Black)
    {
        playerType |= EPlayerType.Ghost;
        RpcDead(isEjection,imposterColor,playerColor);
        if (!isEjection)
        {
            var manager = NetworkRoomManager.singleton as AmongUsRoomManager;
            var deadbody = Instantiate(manager.spawnPrefabs[1], transform.position, transform.rotation)
                .GetComponent<DeadBody>();
            NetworkServer.Spawn(deadbody.gameObject);
            deadbody.RpcSetColor(playerColor);
        }
       
    }
    [ClientRpc]
    private void RpcDead(bool isEjection,EPlayerColor imposterColor, EPlayerColor crewColor)
    {
        if (hasAuthority)
        {
            ani.SetBool("IsGhost",true);
            if (!isEjection)
            {
                InGameUIManager.Instance.KillUI.Open(imposterColor,crewColor);
                StartCoroutine(CloseKillUI());
            }

            var players = GameSystem.Instance.GetPlayerList();
            foreach (var player in players)
            {
                player.SetVisibiltiy(true);
            }
            GameSystem.Instance.ChangeLightMode(EPlayerType.Ghost);
        }
        else
        {
            //0x02는 EPlayerType의 Ghost 타입을 의미합니다. playerType에 0x02를 AND 연산하면
            //플레이어가 Ghost 상태일 때는 0x02가 나오고 살아있는 상태라면 0x00이 나옵니다.
            var myPlayer = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
            if (((int) myPlayer.playerType & 0x02) != (int) EPlayerType.Ghost)
            {
                SetVisibiltiy(false);
            }
        }

        var collier = GetComponent<BoxCollider2D>();
        if (collier)
        {
            collier.enabled = false;
        }
    }
 
    private IEnumerator CloseKillUI()
    {
        yield return new WaitForSeconds(3f);
        InGameUIManager.Instance.KillUI.Close();
    }
    private void Update()
    {
        if (isServer && playerType == EPlayerType.Imposter)
        {
            killCooldown -= Time.deltaTime;
        }
    }

    public void SetVisibiltiy(bool isVisible)
    {
        if (isVisible)
        {
            var color = PlayerColor.GetColor(playerColor);
            color.a = 1f;
            _spriteRenderer.material.SetColor("_PlayerColor", color);
            nicknameText.text = nickname;
        }
        else
        {
            var color = PlayerColor.GetColor(playerColor);
            color.a = 0f;
            _spriteRenderer.material.SetColor("_PlayerColor", color);
            nicknameText.text = "";
        }
    }
    [Command]
    public void CmdVoteEjectPlayer(EPlayerColor ejectColor)
    {
        isVote = true;
        GameSystem.Instance.RpcSignVoteEject(playerColor,ejectColor);
        var players = FindObjectsOfType<InGameCharacterMover>();
        InGameCharacterMover ejectedPlayer = null;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].playerColor == ejectColor)
            {
                ejectedPlayer = players[i];
            }
        }
        ejectedPlayer.vote += 1;
    }
    [Command]
    public void CmdSkipVote() 
    {
        isVote = true;
        GameSystem.Instance.skipVotePlayerCount += 1;
        GameSystem.Instance.RpcSignSkipVote(playerColor);
    }
}
