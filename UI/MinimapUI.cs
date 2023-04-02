using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MinimapUI : MonoBehaviour
{
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private Transform top;
    [SerializeField] private Transform bottom;

    [SerializeField] private Image minimapImage;
    [SerializeField] private Image minimapPlayerImage;

    private CharacterMover targetPlayer;

    private void Start()
    {
        var inst = Instantiate(minimapImage.material);
        minimapImage.material = inst;

        targetPlayer = AmongUsRoomPlayer.MyRoomPlayer.myCharacter;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);   
    }

    private void Update()
    {
        if (targetPlayer != null)
        {
            // left,right,top,bottom을 기준으로 캐릭터의 위치를 측정하고
            // 그 위치를 0~1사이 값으로 변환하는 정규화 과정을 거친 뒤 이미지상의 좌표로 변환시킨다.
            
            // 위치 측정
            Vector2 mapArea = new Vector2(Vector3.Distance(left.position, right.position),
                Vector3.Distance(bottom.position, top.position));
            Vector2 charPos = new Vector2(Vector3.Distance(left.position,
                new Vector3(targetPlayer.transform.position.x, 0f, 0f)),
                Vector3.Distance(bottom.position, new Vector3(0f, targetPlayer.transform.position.y, 0f)));
            
            // 정규화
            Vector2 normalPos = new Vector2(charPos.x / mapArea.x, charPos.y / mapArea.y);

            minimapPlayerImage.rectTransform.anchoredPosition = new Vector2(
                minimapImage.rectTransform.sizeDelta.x * normalPos.x,
                minimapImage.rectTransform.sizeDelta.y * normalPos.y);
        }
    }
}
