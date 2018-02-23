using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    //// Resource bar UI ////
    public GameObject MapUI;
    public GameObject ManagementUI;
    public GameObject QuestUI;

    public GameObject SpecialSpec;
/*
    public GameObject SkillSpec1;
    public GameObject SkillSpec2;
    public GameObject SkillSpec3;
    public GameObject SkillSpec4;
    public GameObject SkillSpec5;
    public GameObject SkillSpec6;
*/
    //// Map UI ////
    public GameObject SkillSet;

    //// Management UI (Production Selection) ////
    public GameObject UnitSelTab;
    public GameObject BuildingSelTab;
    public GameObject EpicTab, HighTab, IntermediateTab, LowTab;    // Unit production
    public GameObject CityTab, CityBuildingTab, NormalBuildingTab;  // Building production

    private ManagementUIController uicontroller;
    private static UIManager _uimanager;
    public static UIManager I { get { return _uimanager; } }
    void Awake()
    {
        // Singleton
        if (_uimanager != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            _uimanager = this;
        }
        // Use this when scene changing exists
        // DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        uicontroller = ManagementUIController.GetManagementUIController();   
    }
    void Update()
    {
        if (GameManager.I.isThereTodos)
        {
            MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "유닛이 명령을 기다리고 있습니다";
        }
        else
        {
            MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "다음 턴";
        }
    }

    //// Resource bar UI ////
    public void MapUIActive()                   // Map UI tab
    {
        MapUI.SetActive(true);
        ManagementUI.SetActive(false);
        QuestUI.SetActive(false);
    }
    public void ManagementUIActive()            // Management UI tab
    {
        SkillSet.SetActive(false);
        ManagementUI.SetActive(true);
        uicontroller.ManageFunction();
        MapUI.SetActive(false);
        QuestUI.SetActive(false);
    }
    public void QuestUIActive()                 // Quest UI tab
    {
        SkillSet.SetActive(false);
        QuestUI.SetActive(true);
        MapUI.SetActive(false);
        ManagementUI.SetActive(false);
    }

    public void SpecialMouseOver()              // 특수 자원
    {
        SpecialSpec.SetActive(true);
    }
    public void SpecialMouseExit()
    {
        SpecialSpec.SetActive(false);
    }

    //// Map UI ////
    public void MoveActive()
    {
        SkillSet.SetActive(false);
        PseudoFSM.I.MoveStateEnter();
    }

    public void AttackActive()
    {
        SkillSet.SetActive(false);
        PseudoFSM.I.AttackStateEnter();
    }

    public void SkillSetActive()
    {
        SkillSet.SetActive(!SkillSet.activeSelf);
    }

    public void Skill1Active()
    {
        PseudoFSM.I.SkillStateEnter(0);
    }
    /*
    public void SkillSpec1MouseOver()           // 특수 명령
    {
        SkillSpec1.SetActive(true);
    }
    public void SkillSpec1MouseExit()
    {
        SkillSpec1.SetActive(false);
    }
    public void SkillSpec2MouseOver()
    {
        SkillSpec2.SetActive(true);
    }
    public void SkillSpec2MouseExit()
    {
        SkillSpec2.SetActive(false);
    }
    public void SkillSpec3MouseOver()
    {
        SkillSpec3.SetActive(true);
    }
    public void SkillSpec3MouseExit()
    {
        SkillSpec3.SetActive(false);
    }
    public void SkillSpec4MouseOver()
    {
        SkillSpec4.SetActive(true);
    }
    public void SkillSpec4MouseExit()
    {
        SkillSpec4.SetActive(false);
    }
    public void SkillSpec5MouseOver()
    {
        SkillSpec5.SetActive(true);
    }
    public void SkillSpec5MouseExit()
    {
        SkillSpec5.SetActive(false);
    }
    public void SkillSpec6MouseOver()
    {
        SkillSpec6.SetActive(true);
    }
    public void SkillSpec6MouseExit()
    {
        SkillSpec6.SetActive(false);
    }
    */
    public void EndTurnActive()
    {
        SkillSet.SetActive(false);
        if (GameManager.I.isThereTodos)
        {
            GameManager.I.SelectNextUnit();
        }
        else
        {
            GameManager.I.ProceedTurn();
        }
    }

    //// Management UI (Production Selection) ////
    public void UnitSelTabActive()
    {
        UnitSelTab.SetActive(true);
        BuildingSelTab.SetActive(false);
    }
    public void BuildingSelTabActive()
    {
        UnitSelTab.SetActive(false);
        BuildingSelTab.SetActive(true);
    }
    
    public void EpicTabActive()                 // Unit production
    {
        EpicTab.SetActive(true);
        HighTab.SetActive(false);
        IntermediateTab.SetActive(false);
        LowTab.SetActive(false);
    }
    public void HighTabActive()
    {
        EpicTab.SetActive(false);
        HighTab.SetActive(true);
        IntermediateTab.SetActive(false);
        LowTab.SetActive(false);
    }
    public void IntermediateTabActive()
    {
        EpicTab.SetActive(false);
        HighTab.SetActive(false);
        IntermediateTab.SetActive(true);
        LowTab.SetActive(false);
    }
    public void LowTabActive()
    {
        EpicTab.SetActive(false);
        HighTab.SetActive(false);
        IntermediateTab.SetActive(false);
        LowTab.SetActive(true);
    }
    
    public void CityTabActive()                 // Building production
    {
        CityTab.SetActive(true);
        CityBuildingTab.SetActive(false);
        NormalBuildingTab.SetActive(false);
    }
    public void CityBuildingTabActive()
    {
        CityTab.SetActive(false);
        CityBuildingTab.SetActive(true);
        NormalBuildingTab.SetActive(false);
    }
    public void NormalBuildingTabActive()
    {
        CityTab.SetActive(false);
        CityBuildingTab.SetActive(false);
        NormalBuildingTab.SetActive(true);
    }
}
