using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public static GameUI Instance = null;

    public GameObject mapUI;
    public GameObject EndTurn;
    public Text goldText, populationText, happinessText, researchText, laborText;
    public GameObject endingScene;

    private UIController uicontroller;
    private ManagementController managementcontroller;
    private SpecialResourceView specialResourceView;

    bool checkButtonText = true;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        endingScene.SetActive(false);
        //mapUI = GameObject.Find("MapUI");
        uicontroller = UIController.GetUIController();
        managementcontroller = ManagementController.GetManagementController();
        specialResourceView = SpecialResourceView.GetSpecialResourceView();
    }
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.Instance.Game.PlayerInTurn.IsAIControlled)
        {
            EndTurn.GetComponentInChildren<Button>().enabled = true;

            if (GameManager.Instance.isThereTodos)
            {
                EndTurn.GetComponentInChildren<Text>().text = "유닛이 명령을 기다리고 있습니다";
                EndTurn.GetComponentInChildren<Text>().fontSize = 30;
            }
            else if(checkButtonText)
            {
                EndTurn.GetComponentInChildren<Text>().text = "다음 턴";
                EndTurn.GetComponentInChildren<Text>().fontSize = 40;
            }

            updatePanel();
        }
    }

    public void updatePanel()
    {
        GameManager.Instance.Game.PlayerInTurn.EstimateResourceInputs();

        double gold = GameManager.Instance.Game.PlayerInTurn.Gold;
        double goldTurn = GameManager.Instance.Game.PlayerInTurn.GoldNetIncome;
        goldText.text = Math.Round(gold, 1) + "(+" + Math.Round(goldTurn, 1) + ")";

        double population = GameManager.Instance.Game.PlayerInTurn.Population;
        populationText.text = Math.Round(population, 1).ToString();

        double happiness = GameManager.Instance.Game.PlayerInTurn.Happiness;
        double happinessTurn = GameManager.Instance.Game.PlayerInTurn.HappinessIncome;
        happinessText.text = Math.Round(happiness, 1) + "(+" + Math.Round(happinessTurn, 1) + ")";

        double research = GameManager.Instance.Game.PlayerInTurn.Research;
        double researchTurn = GameManager.Instance.Game.PlayerInTurn.ResearchIncome;
        researchText.text = Math.Round(research, 1) + "(+" + Math.Round(researchTurn, 1) + ")";

        double labor = GameManager.Instance.Game.PlayerInTurn.Labor;
        laborText.text = Math.Round(labor, 1).ToString();
    }

    public void updateQuest()
    {
        uicontroller.MakeQuestQueue();
    }

    public void updateManagement()
    {
        managementcontroller.begin();
        InvestmentController.I.Tax.GetComponentInChildren<Slider>().normalizedValue = (float)GameManager.Instance.Game.PlayerInTurn.TaxRate;
        InvestmentController.I.EcoInv.GetComponentInChildren<Slider>().normalizedValue = (float)GameManager.Instance.Game.PlayerInTurn.EconomicInvestmentRatio / 2;
        InvestmentController.I.TechInv.GetComponentInChildren<Slider>().normalizedValue = (float)GameManager.Instance.Game.PlayerInTurn.ResearchInvestmentRatio / 2;
        InvestmentController.I.Logistics.GetComponentInChildren<Slider>().normalizedValue = (float)GameManager.Instance.Game.PlayerInTurn.RepairInvestmentRatio;
    }

    public void updateSpecialResource()
    {
        specialResourceView.begin();
    }

    public void CheckEnd()
    {
        if(GameManager.Instance.Game.PlayerInTurn.IsVictoried)
        {
            if (GameManager.Instance.Game.PlayerInTurn == GameManager.Instance.Game.Players[0])
            {
                endingScene.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Endings/Hwan_ending");
            }
            else
            {
                endingScene.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Endings/Finno_ending");
            }
            endingScene.SetActive(true);
        }
        else if (GameManager.Instance.Game.PlayerInTurn.IsDefeated)
        {
            if (GameManager.Instance.Game.PlayerInTurn == GameManager.Instance.Game.Players[0])
            {
                endingScene.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Endings/Finno_ending");
            }
            else
            {
                endingScene.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Endings/Hwan_ending");
            }
            endingScene.SetActive(true);
        }
        else if (GameManager.Instance.Game.PlayerInTurn.IsDrawed)
        {
            endingScene.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Endings/Draw_ending");
            endingScene.SetActive(true);
        }

    }

    private void NextTurn()
    {
        GameManager.Instance.Game.EndTurn();
        GameManager.Instance.Game.StartTurn();
        
        //Proceeds AI's Turns
        while (GameManager.Instance.Game.PlayerInTurn.IsAIControlled)
        {
            GameManager.Instance.Game.PlayerInTurn.DoAITurnAction().GetAwaiter().GetResult();
            GameManager.Instance.Game.EndTurn();
            GameManager.Instance.Game.StartTurn();
        }
        checkButtonText = true;
        EndTurn.GetComponentInChildren<Button>().interactable = true;

        AlarmManager.Instance.updateAlarmQueue();

        GameManager.Instance.UpdateMap();
        GameManager.Instance.UpdateUnit();
        GameManager.Instance.UpdateMinimap();
        UIManager.Instance.ButtonInteractChange();

        GameManager.Instance.CheckCompletedProduction();

        managementcontroller.MakeProductionQ();
        managementcontroller.MakeDeploymentQ();
        uicontroller.MakeQuestQueue();

        GameManager.Instance.CheckNewQuest();
    }

    public void onClickNextTurn()
    {
        if (GameManager.Instance.isThereTodos)
        {
            GameManager.Instance.FocusOnNextActableUnit();
        }
        else
        {
            if (GameManager.Instance.Game.PlayerInTurn == GameManager.Instance.Game.Players[GameInfo.UserPlayer])
            {
                /*
                GameManager.Instance.Game.EndTurn();
                GameManager.Instance.Game.StartTurn();

                EndTurn.GetComponentInChildren<Text>().text = "다른 플레이어가 턴 진행 중입니다.";
                EndTurn.GetComponentInChildren<Text>().fontSize = 30;
                EndTurn.GetComponentInChildren<Button>().interactable = false;

                // Proceeds AI's Turns
                while (GameManager.Instance.Game.PlayerInTurn.IsAIControlled)
                {
                    GameManager.Instance.Game.PlayerInTurn.DoAITurnAction().GetAwaiter().GetResult();
                    GameManager.Instance.Game.EndTurn();
                    GameManager.Instance.Game.StartTurn();
                } */
                checkButtonText = false;
                EndTurn.GetComponentInChildren<Text>().text = "다른 플레이어가 턴 진행 중입니다.";
                EndTurn.GetComponentInChildren<Text>().fontSize = 30;
                EndTurn.GetComponentInChildren<Button>().interactable = false;
                Invoke("NextTurn", 0.2f);
            }
        }
    }
}

// HWAN ONLY PLAY
        //else
        //{
        //    // Debug.Log(Game.PlayerNumberInTurn);
        //    if (GameManager.Instance.Game.PlayerInTurn == GameManager.Instance.Game.Players[0])
        //        GameManager.Instance.Game.EndTurn();
        //    GameManager.Instance.Game.StartTurn();
        //    while (GameManager.Instance.Game.PlayerInTurn.IsAIControlled)
        //    {
        //        // Debug.Log(Game.PlayerNumberInTurn);
        //        GameManager.Instance.Game.PlayerInTurn.DoAITurnAction().GetAwaiter().GetResult();
        //        GameManager.Instance.Game.EndTurn();
        //        GameManager.Instance.Game.StartTurn();
        //    }
        //    GameManager.Instance.UpdateMap();
        //    GameManager.Instance.UpdateUnit();
        //    UIManager.Instance.UpdateUnitInfo();
        //    UIManager.Instance.ButtonInteractChange();

        //    AlarmManager.Instance.updateAlarmQueue();
        //    GameManager.Instance.CheckCompletedProduction();
        //    //AlarmManager.Instance.AddAlarm(null, "HI", null, 0);
        //}


 // Finno도 플레이하고 싶을 때 else 안을 대체
//if (GameManager.Instance.Game.PlayerInTurn == GameManager.Instance.Game.Players[0])
            //{
            //    GameManager.Instance.Game.EndTurn();
            //    GameManager.Instance.Game.StartTurn();
            //}
            //else if (GameManager.Instance.Game.PlayerInTurn == GameManager.Instance.Game.Players[1])
            //{
            //    GameManager.Instance.Game.EndTurn();
            //    GameManager.Instance.Game.StartTurn();
            //    while (GameManager.Instance.Game.PlayerInTurn.IsAIControlled)
            //    {
            //        // Debug.Log(Game.PlayerNumberInTurn);
            //        GameManager.Instance.Game.PlayerInTurn.DoAITurnAction().GetAwaiter().GetResult();
            //        GameManager.Instance.Game.EndTurn();
            //        GameManager.Instance.Game.StartTurn();
            //    }
            //    GameManager.Instance.UpdateMap();
            //    GameManager.Instance.UpdateUnit();
            //    UIManager.Instance.UpdateUnitInfo();
            //    UIManager.Instance.ButtonInteractChange();

            //    AlarmManager.Instance.updateAlarmQueue();
            //    GameManager.Instance.CheckCompletedProduction();
            ////AlarmManager.Instance.AddAlarm(null, "HI", null, 0);
            //}