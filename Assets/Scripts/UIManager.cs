using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class UIManager : MonoBehaviour {

    //// Resource bar UI ////
    public GameObject MapUI;
    public GameObject Actions;
    public GameObject ManagementUI;
    public GameObject QuestUI;
    public GameObject GameEND;

    public Text Gold;
    public Text Population;
    public Text Happiness;
    public Text Technology;
    public Text Labor;

    public GameObject UnitInfo;
    public Image UnitPortrait;
    public Text UnitName;
    public Text UnitAttack;
    public Text UnitDefence;
    public Text UnitEffect;
    public Text ActionPoint;

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

    public GameObject QuestPopPrefab;


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
        if (GameManager.I.Game.PlayerInTurn.IsAIControlled)
        {
            MapUI.transform.Find("EndTurn").GetComponentInChildren<Button>().enabled = false;
            MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "다른 플레이어가 턴 진행 중입니다. 기다려 주십시오.";
            MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().fontSize = Screen.height / 40;
        }
        else
        {
            MapUI.transform.Find("EndTurn").GetComponentInChildren<Button>().enabled = true;
            if (GameManager.I.isThereTodos && !PseudoFSM.I.DepState)
            {
                MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "유닛이 명령을 기다리고 있습니다";
                MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().fontSize = Screen.height / 40;
            }
            else if (PseudoFSM.I.DepState)
            {
                MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "배치 취소";
                MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().fontSize = Screen.height / 25;
            }
            else
            {
                MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "다음 턴";
                MapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().fontSize = Screen.height / 25;
            }
            Gold.text = "금 : " + GameManager.I.Game.PlayerInTurn.Gold + "(+" + GameManager.I.Game.PlayerInTurn.GoldIncome + ")";
            Population.text = "인구 : " + GameManager.I.Game.PlayerInTurn.Population;
            Happiness.text = "행복 : " + GameManager.I.Game.PlayerInTurn.Happiness;
            Technology.text = "기술력 : " + GameManager.I.Game.PlayerInTurn.Research;
            Labor.text = "노동력 : " + GameManager.I.Game.PlayerInTurn.Labor;
        }
    }
    public void MakeUnitInfo()
    {
        if (GameManager.I.SelectedActor != null)
        {
            UnitInfo.SetActive(true);
            //(GameManager.I.SelectedActor.Owner == ) 
            UnitPortrait.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetPortName(GameManager.I.SelectedActor)).ToLower()), typeof(Sprite)) as Sprite;
            UnitName.text = ProductionFactoryTraits.GetName(GameManager.I.SelectedActor);
            UnitAttack.text = GameManager.I.SelectedActor.AttackPower.ToString();
            UnitDefence.text = GameManager.I.SelectedActor.DefencePower.ToString();
            UnitEffect.text = "";
            ActionPoint.text = GameManager.I.SelectedActor.RemainAP + "/" + GameManager.I.SelectedActor.MaxAP;
            Actions.SetActive(true);
        }
        else
        {
            UnitInfo.SetActive(false);
            UnitName.text = "";
            UnitAttack.text = "";
            UnitDefence.text = "";
            UnitEffect.text = "";
            ActionPoint.text = "";
            Actions.SetActive(false);
        }
    }
    //// Resource bar UI ////
    public void MapUIActive()                   // Map UI tab
    {
        MapUI.SetActive(true);
        ManagementUI.SetActive(false);
        QuestUI.SetActive(false);
    }
    public void GameEnd()                   // Map UI tab
    {
        GameEND.SetActive(true);
        ManagementUI.SetActive(false);
        QuestUI.SetActive(false);
        MapUI.SetActive(false);
    }
    public void ManagementUIActive()            // Management UI tab
    {
        SkillSet.SetActive(false);
        ManagementUI.SetActive(true);
        uicontroller.ManageFunction();
        InvestUIController.I.initSlider();
        MapUI.SetActive(false);
        QuestUI.SetActive(false);
    }
    public void QuestUIActive()                 // Quest UI tab
    {
        SkillSet.SetActive(false);
        uicontroller.MakeQuestQueue();
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

    public void WaitActive()
    {
        SkillSet.SetActive(false);
        GameManager.I.SelectedActor.SkipFlag = true;
        GameManager.I.SelectNextUnit();
        MakeUnitInfo();
        PseudoFSM.I.NormalStateEnter();
    }
    public void SleepActive()
    {
        SkillSet.SetActive(false);
        GameManager.I.SelectedActor.SleepFlag = true;
        GameManager.I.SelectNextUnit();
        MakeUnitInfo();
        PseudoFSM.I.NormalStateEnter();
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
        if (GameManager.I.isThereTodos && !PseudoFSM.I.DepState)
        {
            GameManager.I.SelectNextUnit();
            MakeUnitInfo();
        }
        else if(PseudoFSM.I.DepState)
        {
            PseudoFSM.I.NormalStateEnter();
            MakeUnitInfo();
        }
        else
        {
            GameManager.I.ProceedTurn();
        }
    }

    public void ShowQuestEnd(Quest quest)
    {
        var Qrefab = Instantiate(QuestPopPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        Qrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        Qrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
        Button[] buttons = Qrefab.GetComponentsInChildren<Button>();
        Image[] images = Qrefab.GetComponentsInChildren<Image>();
        Text[] texts = Qrefab.GetComponentsInChildren<Text>();
        foreach (Button btn in buttons)
        {
            switch (btn.name)
            {
                case "Exit":
                    btn.onClick.AddListener(delegate { QuestEndEnd(Qrefab); });
                    break;
                default: break;
            }
        }
        foreach (Image img in images)
        {
            switch (img.name)
            {
                case "Image":
                    Debug.Log("Quests/" + ParseQuest.GetQuestName(quest));
                    img.sprite = Resources.Load(("Quests/" + ParseQuest.GetQuestName(quest)).ToLower(), typeof(Sprite)) as Sprite;
                    if(img.sprite == null)
                    {
                        Debug.Log("아씨바ㅏㅓㅇㄹ넝ㄹㅇㄴㅁㄹ");
                    }
                    break;
                default: break;
            }
        }
        foreach (Text txt in texts)
        {
            switch (txt.name)
            {
                case "RewardNotice":
                    txt.text = quest.CompleteNotice;
                    break;
                default: break;
            }
        }
    }

    public void QuestEndEnd(GameObject Qrefab)
    {
        Destroy(Qrefab);
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
    public void WaitTurn()
    {
        MapUI.SetActive(true);
        ManagementUI.SetActive(false);
        QuestUI.SetActive(false);
    }
}
