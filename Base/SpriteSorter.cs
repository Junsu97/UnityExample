using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSorter : MonoBehaviour
{
    [SerializeField] private Transform Back;
    [SerializeField] private Transform Front;

    public int GetSortingOrder(GameObject obj)
    {
        //Abs는 절대값 반환
        float objDis = Mathf.Abs(Back.position.y - obj.transform.position.y);
        float totalDis = Mathf.Abs(Back.position.y - Front.position.y);

        return (int) (Mathf.Lerp(System.Int16.MinValue, System.Int16.MaxValue, objDis / totalDis));
    }
}
