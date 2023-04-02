using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Mirror;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private List<Image> crewImgs;
    [SerializeField] private List<Button> imposterCountButtons; // 임포스터 수
    [SerializeField] private List<Button> maxPlayerCountButtons; // 최대 플레이어 수

    private CreateGameRoomData roomData;

    private void Start()
    {
        for (int i = 0; i < crewImgs.Count; i++)
        {
            Material matInstance = Instantiate(crewImgs[i].material);
            crewImgs[i].material = matInstance;
        }
        roomData = new CreateGameRoomData() {imposterCount = 1, maxPlayerCount = 10};
        UpdateCrewImages();
    }

    public void UpdateImposterCount(int count)
    {
        roomData.imposterCount = count;

        for (int i = 0; i < imposterCountButtons.Count; i++)
        {
            if (i == count - 1)
            {
                imposterCountButtons[i].image.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                imposterCountButtons[i].image.color = new Color(1f, 1f, 1f, 0f);
            }
        }
        
        // 임포스터 수에 따른 플레이어 수
        // 임포스터 1 = 플레이어 4  /  2 == 7 둘다 아닐경우 9 / 
        int limitMaxPlayer = count == 1 ? 4 : count == 2 ? 7 : 9;
        if (roomData.maxPlayerCount < limitMaxPlayer)
        {
            UpdateMaxPlayer(limitMaxPlayer);
        }
        else
        {
            UpdateMaxPlayer(roomData.maxPlayerCount);
        }
        
        // limitMaxPlayer보다 낮은 최대인원 수 선택 버튼을 비활성화한다.
        for (int i = 0; i < maxPlayerCountButtons.Count; i++)
        {
            var text = maxPlayerCountButtons[i].GetComponentInChildren<Text>();
            if (i < limitMaxPlayer - 4)
            {
                maxPlayerCountButtons[i].interactable = false;
                text.color = Color.gray;
            }
            else
            {
                maxPlayerCountButtons[i].interactable = true;
                text.color = Color.white;
            }
        }
    }

    public void UpdateMaxPlayer(int count)
    {
        roomData.maxPlayerCount = count;

        for (int i = 0; i < maxPlayerCountButtons.Count; i++)
        {
            if (i == count - 4)
            {
                maxPlayerCountButtons[i].image.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                maxPlayerCountButtons[i].image.color = new Color(1f, 1f, 1f, 0f);
            }
        }
        
        UpdateCrewImages();
    }
    private void UpdateCrewImages()
    {
        for (int i = 0; i < crewImgs.Count; i++)
        {
            crewImgs[i].material.SetColor("_PlayerColor",Color.white);
        }
        // 맵 배너위에 올라가있는 크루원의 이미지가 크루원의 수에 따라서 바뀌도록 만들어주는 함수
        int imposterCount = roomData.imposterCount;
        int idx = 0;
        while (imposterCount != 0)
        {
            if (idx >= roomData.maxPlayerCount)
            {
                idx = 0;
            }

            if (crewImgs[idx].material.GetColor("_PlayerColor") != Color.red && Random.Range(0, 5) == 0)
            {
                crewImgs[idx].material.SetColor("_PlayerColor",Color.red);
                imposterCount--;
            }

            idx++;
        }

        for (int i = 0; i < crewImgs.Count; i++)
        {
            if (i < roomData.maxPlayerCount)
            {
                crewImgs[i].gameObject.SetActive(true);
            }
            else
            {
                crewImgs[i].gameObject.SetActive(false);
            }
        }
    }

    public void CreateRoom()
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        // 방설정 작업 처리
        manager.minPlayerCount = roomData.imposterCount == 1 ? 4 : roomData.imposterCount == 2 ? 7 : 9;
        manager.imposterCount = roomData.imposterCount;
        manager.maxConnections = roomData.maxPlayerCount;
        manager.StartHost();
    }
}

public class CreateGameRoomData
{
    public int imposterCount;
    public int maxPlayerCount;
}
