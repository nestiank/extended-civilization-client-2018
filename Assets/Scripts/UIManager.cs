using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject MapUI;
    public GameObject ManagementUI;
    public GameObject QuestUI;

    public GameObject SkillSet;                   //** Map UI     ->      Does it need to be in MapUI.cs?

    public GameObject UnitSelUI;                  //** Production Selection in Management UI    ->      Do they need to be in ManagementUI.cs?
    public GameObject BuildingSelUI;
//    public GameObject Tier1, Tier2, Tier3, Tier4;                 // for tab under tab
//    public GameObject CIty, CityBuilding, NormalBuilding;

    public void MapUIActive()               // Map UI tab
    {
        MapUI.SetActive(true);
        ManagementUI.SetActive(false);
        QuestUI.SetActive(false);
    }
    public void ManagementUIActive()        // Management UI tab
    {
        ManagementUI.SetActive(true);
        MapUI.SetActive(false);
        QuestUI.SetActive(false);
    }
    public void QuestUIActive()             // Quest UI tab
    {
        QuestUI.SetActive(true);
        MapUI.SetActive(false);
        ManagementUI.SetActive(false);
    }


    public void SkillSetActive()               //** Map UI
    {
        SkillSet.SetActive(!SkillSet.activeSelf);
    }

    public void UnitSelUIActive()              //** Production Selection in Management UI
    {
        UnitSelUI.SetActive(true);
        BuildingSelUI.SetActive(false);
    }
    public void BuildingSelUIActive()
    {
        UnitSelUI.SetActive(false);
        BuildingSelUI.SetActive(true);
    }
}
