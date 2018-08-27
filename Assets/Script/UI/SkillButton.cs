using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        UIManager.Instance.skillDescription.SetActive(true);
        UIManager.Instance.skillDescription.transform.GetChild(0).GetComponent<Text>().text = GameManager.Instance.selectedActor.ActiveSkills[(int)(gameObject.name[5]) - 49].SkillDescription;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        UIManager.Instance.skillDescription.SetActive(false);
    }
}
