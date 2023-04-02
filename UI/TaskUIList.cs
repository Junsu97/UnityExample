using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TaskUIList : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float offset;
    [SerializeField] private RectTransform TaskListUITransfrom;

    private bool isOpen = true;
    private float timer;
    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(OpenAndHideUI());
    }

    private IEnumerator OpenAndHideUI()
    {
        isOpen = !isOpen;
        if (timer != 0f)
        {
            timer = 1f - timer;
        }

        while (timer <= 1f)
        {
            timer += Time.deltaTime * 2f;

            float start = isOpen ? -TaskListUITransfrom.sizeDelta.x : offset;
            float dest = isOpen ? offset : -TaskListUITransfrom.sizeDelta.x;
            TaskListUITransfrom.anchoredPosition =
                new Vector2(Mathf.Lerp(start, dest, timer), TaskListUITransfrom.anchoredPosition.y);
            yield return null;
        }
    }
}
