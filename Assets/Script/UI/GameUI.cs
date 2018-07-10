using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;

public class GameUI : MonoBehaviour {

    public GameObject mapUI;
    public Text goldText, populationText, happinessText, researchText, laborText;

    private UIController uicontroller;
    private ManagementController managementcontroller;

    // Use this for initialization
    void Start () {
        mapUI = GameObject.Find("MapUI");
        uicontroller = UIController.GetUIController();
        managementcontroller = ManagementController.GetManagementController();
    }
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.Game.PlayerInTurn.IsAIControlled)
        {
            mapUI.transform.Find("EndTurn").GetComponentInChildren<Button>().enabled = false;
            mapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "다른 플레이어가 턴 진행 중입니다. 기다려 주십시오.";
            mapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().fontSize = Screen.height / 40;
        }
        else
        {
            mapUI.transform.Find("EndTurn").GetComponentInChildren<Button>().enabled = true;
            /*
            if (GameManager.Instance.isThereTodos && !PseudoFSM.Instance.DepState)
            {
                mapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "유닛이 명령을 기다리고 있습니다";
                mapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().fontSize = Screen.height / 40;
            }
            else if (PseudoFSM.Instance.DepState)
            {
                mapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "배치 취소";
                mapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().fontSize = Screen.height / 25;
            }
            else
            {
            */
                mapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().text = "다음 턴";
                mapUI.transform.Find("EndTurn").GetComponentInChildren<Text>().fontSize = Screen.height / 25;
            //}

            updatePanel();
            if (GameObject.Find("GameUI").activeSelf) updateQuest();
            else if (GameObject.Find("ManagementUI").activeSelf) updateManagement();
        }
    }

    public void updatePanel()
    {
        double gold = GameManager.Instance.Game.PlayerInTurn.Gold;
        double goldTurn = GameManager.Instance.Game.PlayerInTurn.GoldIncome;
        goldText.text = "금: " + gold + "\n(턴당 " + goldTurn + ")";

        double population = GameManager.Instance.Game.PlayerInTurn.Population;
        populationText.text = "인구: " + population;

        double happiness = GameManager.Instance.Game.PlayerInTurn.Happiness;
        double happinessTurn = GameManager.Instance.Game.PlayerInTurn.HappinessIncome;
        happinessText.text = "행복: " + happiness + "\n(턴당 " + happinessTurn + ")";

        double research = GameManager.Instance.Game.PlayerInTurn.Research;
        double researchTurn = GameManager.Instance.Game.PlayerInTurn.ResearchIncome;
        researchText.text = "기술력: " + research + "\n(턴당 " + researchTurn + ")";

        double labor = GameManager.Instance.Game.PlayerInTurn.Labor;
        laborText.text = "노동력: " + labor;
    }

    public void updateQuest()
    {
        uicontroller.MakeQuestQueue();
    }

    public void updateManagement()
    {
        managementcontroller.begin();
    }
}