using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Unity.Collections.LowLevel.Unsafe;

public enum EKillRange
{
    Short, Normal, Long
}

public enum ETaskBarUpdates
{
    Always, Meetings, Never
}

public struct GameRuleData
{
    public bool confirmEjects;
    public int emergencyMeetings;
    public int emergencyMeetingCooldown;
    public int meetingsTime;
    public int voteTime;
    public bool anonymousVotes;
    public float moveSpeed;
    public float crewSight;
    public float imposterSight;
    public float killCooldown;
    public EKillRange KillRange;
    public bool visualTasks;
    public ETaskBarUpdates taskBarUpdates;
    public int commonTask;
    public int complexTask;
    public int simpleTask;
}
public class GameRuleStroe : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetIsRecommendRule_Hook))] 
    private bool isRecommendRule;
    [SerializeField] private Toggle isRecommendRuleToggle;

    public void SetIsRecommendRule_Hook(bool _, bool value)
    {
        UpdateGameRuleOverView();
    }

    public void OnRecommendToggle(bool value)
    {
        isRecommendRule = value;
        if (isRecommendRule)
        {
            SetRecommendGameRule();
        }
    }
    
    [SyncVar(hook = nameof(SetConfirmEjects))]
    private bool confirmEjects;
    [SerializeField] private Toggle confirmEjectsToggle;
    public void SetConfirmEjects(bool _, bool value)
    {
        UpdateGameRuleOverView();
    }

    public void OnConfirmEjectsToggle(bool value)
    {
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
        confirmEjects = value;
    }
    
    [SyncVar(hook = nameof(SetEmergencyMeetings_Hook))]
    private int emergencyMeetings;
    [SerializeField] private Text emergencyMeetingsText;

    public void SetEmergencyMeetings_Hook(int _, int value)
    {
        emergencyMeetingsText.text = value.ToString();
        UpdateGameRuleOverView();
    }

    public void OnChangeEmergencyMeetings(bool isPlus)
    {
        emergencyMeetings = Mathf.Clamp(emergencyMeetings + (isPlus ? 1 : -1), 0, 9);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetEmergencyMeetingCoolDown_Hook))]
    private int emergencyMeetingCooldown;
    [SerializeField] private Text emergencyMeetingCooldownText;
    public void SetEmergencyMeetingCoolDown_Hook(int _, int value)
    {
        emergencyMeetingCooldownText.text = string.Format("{0}s", value);
        UpdateGameRuleOverView();
    }
    public void OnChangeEmergencyMeetingCoolDown(bool isPlus)
    {
        emergencyMeetingCooldown = Mathf.Clamp(emergencyMeetingCooldown + (isPlus ? 5 : -5), 0, 60);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetMeetingTime_Hook))]
    private int meetingsTime;
    [SerializeField] private Text meetingTimeText;
    public void SetMeetingTime_Hook(int _, int value)
    {
        meetingTimeText.text = string.Format("{0}s", value);
        UpdateGameRuleOverView();
    }

    public void OnChangeMeetingTime(bool isPlus)
    {
        meetingsTime = Mathf.Clamp(meetingsTime + (isPlus ? 5 : -5), 0, 120);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    [SyncVar(hook = nameof(SetVoteTime_Hook))]
    private int voteTime;
    [SerializeField] private Text voteTimeText;
    public void SetVoteTime_Hook(int _, int value)
    {
        voteTimeText.text = string.Format("{0}s", value);
        UpdateGameRuleOverView();
    }
    public void OnChangeVoteTime(bool isPlus)
    {
        voteTime = Mathf.Clamp(voteTime + (isPlus ? 5 : -5), 0, 300);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetAnonymousVotes_Hook))]
    private bool anonymousVotes;
    [SerializeField] private Toggle anonymouseVotesToggle;
    public void SetAnonymousVotes_Hook(bool _, bool value)
    {
        UpdateGameRuleOverView();
    }

    public void OnAnonymousVotesToggle(bool value)
    {
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
        anonymousVotes = value;
    }
    
    [SyncVar(hook = nameof(SetMoveSpeed_Hookt))]
    private float moveSpeed;
    [SerializeField] private Text moveSpeedText;

    public void SetMoveSpeed_Hookt(float _, float value)
    {
        moveSpeedText.text = string.Format("{0:0.0}x", value);
        UpdateGameRuleOverView();
    }
    public void OnChangeMoveSpeed(bool isPlus)
    {
        moveSpeed = Mathf.Clamp(moveSpeed + (isPlus ? 0.25f : -0.25f), 0.5f, 3f);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetCrewSight_Hook))]
    private float crewSight;
    [SerializeField] private Text crewSightText;
    public void SetCrewSight_Hook(float _, float value)
    {
        crewSightText.text = string.Format("{0:0.0}x", value);
        UpdateGameRuleOverView();
    }
    public void OnChangeCrewSight(bool isPlus)
    {
        crewSight = Mathf.Clamp(crewSight + (isPlus ? 0.25f : -0.25f), 0.25f, 5f);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetImposterSight_Hook))]
    private float imposterSight;
    [SerializeField] private Text imposterSightText;
    public void SetImposterSight_Hook(float _, float value)
    {
        imposterSightText.text = string.Format("{0:0.0}x", value);
        UpdateGameRuleOverView();
    }
    public void OnChangeImposterSight(bool isPlus)
    {
        imposterSight = Mathf.Clamp(imposterSight + (isPlus ? 0.25f : -0.25f), 0.5f, 5f);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetKillCoolDown_Hook))]
    private float killCooldown;
    [SerializeField] private Text killCooldownText;
    public void SetKillCoolDown_Hook(float _, float value)
    {
        killCooldownText.text = string.Format("{0:0.0}s", value);
        UpdateGameRuleOverView();
    }
    public void OnChangeKillCoolDown(bool isPlus)
    {
        killCooldown = Mathf.Clamp(killCooldown + (isPlus ? 2.5f : -2.5f), 10f, 60f);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetKillRange_Hook))]
    private EKillRange killRange;
    [SerializeField] private Text killRangeText;

    public void SetKillRange_Hook(EKillRange _, EKillRange value)
    {
        killRangeText.text = value.ToString();
        UpdateGameRuleOverView();
    }
    public void OnChangeKillRange(bool isPlus)
    {
        killRange = (EKillRange)Mathf.Clamp((int)killRange + (isPlus ? 1 : -1), 0, 2);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetVisualTasks_Hook))]
    private bool visualTasks;
    [SerializeField] private Toggle visualTasksToggle;
    public void SetVisualTasks_Hook(bool _, bool value)
    {
        UpdateGameRuleOverView();
    }

    public void OnVisualTasksToggle(bool value)
    {
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
        visualTasks = value;
    }
    
    [SyncVar(hook = nameof(SetTaskBarUpdate_Hook))]
    private ETaskBarUpdates taskBarUpdates;
    [SerializeField] private Text taskBarUpdatesText;
    public void SetTaskBarUpdate_Hook(ETaskBarUpdates _, ETaskBarUpdates value)
    {
        taskBarUpdatesText.text = value.ToString();
        UpdateGameRuleOverView();
    }
    public void OnChangeTaskBarUpdates(bool isPlus)
    {
        taskBarUpdates = (ETaskBarUpdates)Mathf.Clamp((int)taskBarUpdates + (isPlus ? 1 : -1), 0, 2);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetCommonTask_Hook))]
    private int commonTask;
    [SerializeField] private Text commonTaskText;
    public void SetCommonTask_Hook(int _, int value)
    {
        commonTaskText.text = value.ToString();
        UpdateGameRuleOverView();
    }
    public void OnChangeCommonTask(bool isPlus)
    {
        commonTask = Mathf.Clamp(commonTask + (isPlus ? 1 : -1), 0, 2);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetComplexTask_Hook))]
    private int complexTask;
    [SerializeField] private Text complexTaskText;
    public void SetComplexTask_Hook(int _, int value)
    {
        complexTaskText.text = value.ToString();
        UpdateGameRuleOverView();
    }
    public void OnChangeComplexTask(bool isPlus)
    {
        complexTask = Mathf.Clamp(commonTask + (isPlus ? 1 : -1), 0, 3);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }
    
    [SyncVar(hook = nameof(SetSimpleTask_Hook))]
    private int simpleTask;
    [SerializeField] private Text simpleTaskText;
    public void SetSimpleTask_Hook(int _, int value)
    {
        simpleTaskText.text = value.ToString();
        UpdateGameRuleOverView();
    }
    public void OnChangeSimpleTask(bool isPlus)
    {
        simpleTask = Mathf.Clamp(simpleTask + (isPlus ? 1 : -1), 0, 5);
        isRecommendRule = false;
        isRecommendRuleToggle.isOn = false;
    }

    [SyncVar(hook = nameof(SetImposterCount_Hook))]
    private int imposterCount;
    public void SetImposterCount_Hook(int _, int value)
    {
        UpdateGameRuleOverView();
    }

    [SerializeField] 
    private Text gameRuleOverview;

    public void UpdateGameRuleOverView()
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        
        //문자열 상수더하기 연산으로하면 성능상 이슈가 있을 수 있기 때문에 StringBuilder를 사용
        StringBuilder sb = new StringBuilder(isRecommendRule ? "추천설정\n" : "커스텀 설정\n");
        sb.Append("맵: TheSkeld\n");
        sb.Append($"#임포스터: {imposterCount}\n");
        sb.Append(string.Format("Confirm Ejects: {0}\n", confirmEjects ? "켜짐" : "꺼짐"));
        sb.Append($"긴급회의: {emergencyMeetings}\n");
        sb.Append(string.Format("Anonymous Votes: {0}\n", anonymousVotes ? "켜짐" : "꺼짐"));
        sb.Append($"긴급 회의 쿨타임: {emergencyMeetingCooldown}\n");
        sb.Append($"회의 제한 시간: {meetingsTime}\n");
        sb.Append($"투표 제한 시간: {voteTime}\n");
        sb.Append($"이동 속도: {moveSpeed}\n");
        sb.Append($"크루원 시야: {crewSight}\n");
        sb.Append($"임포스터 시야: {imposterSight}\n");
        sb.Append($"킬 쿨타임: {killCooldown}\n");
        sb.Append($"킬 범위: {killRange}\n");
        sb.Append($"Task Bar Updates: {taskBarUpdates}\n");
        sb.Append(string.Format("Visual Task: {0}\n",visualTasks? "켜짐" : "꺼짐"));
        sb.Append($"공통 임무: {commonTask}\n");
        sb.Append($"복잡한 임무: {complexTask}\n");
        sb.Append($"간단한 임무: {simpleTask}\n");
        gameRuleOverview.text = sb.ToString();
    }

    private void SetRecommendGameRule()
    {
        isRecommendRule = true;
        confirmEjects = true;
        emergencyMeetings = 1;
        emergencyMeetingCooldown = 15;
        meetingsTime = 15;
        voteTime = 120;
        moveSpeed = 1f;
        crewSight = 1f;
        imposterSight = 1.5f;
        killCooldown = 45f;
        killRange = EKillRange.Normal;
        visualTasks = true;
        commonTask = 1;
        complexTask = 1;
        simpleTask = 2;
    }

    private void Start()
    {
        if (isServer)
        {
            var manager = NetworkManager.singleton as AmongUsRoomManager;
            imposterCount = manager.imposterCount;
            anonymousVotes = false;
            taskBarUpdates = ETaskBarUpdates.Always;
            SetRecommendGameRule();
        }
    }

    public GameRuleData GetGameRuleData()
    {
        return new GameRuleData()
        {
            anonymousVotes = anonymousVotes,
            commonTask = commonTask,
            complexTask = complexTask,
            confirmEjects = confirmEjects,
            crewSight = crewSight,
            emergencyMeetings = emergencyMeetings,
            emergencyMeetingCooldown = emergencyMeetingCooldown,
            imposterSight = imposterSight,
            killCooldown = killCooldown,
            KillRange = killRange,
            meetingsTime = meetingsTime,
            moveSpeed = moveSpeed,
            simpleTask = simpleTask,
            taskBarUpdates = taskBarUpdates,
            visualTasks = visualTasks,
            voteTime = voteTime
        };
    }
}
