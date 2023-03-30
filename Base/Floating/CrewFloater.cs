using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrewFloater : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<Sprite> sprites;
    
    private bool[] crewStates = new bool[12];// 스프라이트 중첩이 되지 않게 관리할 배열
    private float timer = 0.5f; // 크루 소환 타이머
    private float distance = 11f; // 중심으로부터 소환될 위치

    private void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            SpawnFloatingCrew((EPlayerColor)i,Random.Range(0f,distance));
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnFloatingCrew((EPlayerColor)Random.Range(0,12),distance);
            timer = 1f;
        }
    }

    public void SpawnFloatingCrew(EPlayerColor playerColor, float dist)
    {
        // 크루원이 생성될 때 카메라를 벗어나서 원형으로 생성되게 해야함.
        // 0~360 사이를 Sin과 Cos를 이용하여 벡터를 구하면 중심으로부터 원형의 방향을 돌아가며 가리키는 벡터를 구할 수 있음
        if (!crewStates[(int) playerColor])
        {
            crewStates[(int) playerColor] = true;
            
            float angle = Random.Range(0f, 360f);
            Vector3 spawnPos = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * dist;
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f),0f);
            float floatingSpeed = Random.Range(1f, 4f);
            float rotateSpeed = Random.Range(-3f, 3f);



            var crew = Instantiate(prefab, spawnPos, Quaternion.identity).GetComponent<FloatingCrew>();
            crew.SetFloatingCrew(sprites[Random.Range(0,sprites.Count)],playerColor,
                direction,floatingSpeed,rotateSpeed,Random.Range(0.5f,1f));
        }
   
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var crew = other.GetComponent<FloatingCrew>();
        if (crew != null)
        {
            crewStates[(int) crew.PlayerColor] = false;
            Destroy(crew.gameObject);
        }
    }
}
