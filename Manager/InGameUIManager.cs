using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance;

    [SerializeField] private InGameIntroUI ingameIntroUI;
    public InGameIntroUI InGameIntroUI { get { return ingameIntroUI; } }
    
    [SerializeField] private KillButtonUI killButtonUI;
    public KillButtonUI KillButtonUI { get { return killButtonUI; } }

    [SerializeField] private KillUI killUI;
    public KillUI KillUI { get { return killUI; } }

    [SerializeField] private ReportButtonUI _reportButtonUI;
    public ReportButtonUI ReportButtonUI { get { return _reportButtonUI; } }
    
    [SerializeField] private ReportUI reportUI;
    public ReportUI ReportUI { get { return reportUI; } }

    [SerializeField] private MeetingUI _meetingUI;
    public MeetingUI MeetingUI { get { return _meetingUI; } }

    [SerializeField] private EjectionUI _ejectionUI;
    public EjectionUI EjectionUI { get { return _ejectionUI; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
