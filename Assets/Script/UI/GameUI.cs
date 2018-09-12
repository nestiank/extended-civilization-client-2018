using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using CivModel.Quests;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;


public class GameUI : MonoBehaviour {

    public static GameUI Instance = null;

    public GameObject mapUI;
    public GameObject EndTurn;
    public Text goldText, populationText, happinessText, researchText, laborText;
    public GameObject endingScene;
    public GameObject Music;
    private UIController uicontroller;
    private ManagementController managementcontroller;
    private SpecialResourceView specialResourceView;

    //public GameObject ultimateProgress = null;
    public GameObject myUltimateProgress;
    public GameObject oppositeUltimateProgress;

    public GameObject UltAlert;
    bool checkButtonText = true;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        endingScene.SetActive(false);
        //mapUI = GameObject.Find("MapUI");
        uicontroller = UIController.GetUIController();
        managementcontroller = ManagementController.GetManagementController();
        specialResourceView = SpecialResourceView.GetSpecialResourceView();
        StartCoroutine(updatePanelCoroutine());
    }

    // Update is called once per frame
    void Update() {
        if (!GameManager.Instance.Game.PlayerInTurn.IsAIControlled)
        {
            EndTurn.GetComponentInChildren<Button>().enabled = true;

            if (GameManager.Instance.isThereTodos)
            {
                EndTurn.GetComponentInChildren<Text>().text = "유닛이 명령을 기다리고 있습니다";
                EndTurn.GetComponentInChildren<Text>().fontSize = 30;
            }
            else if (checkButtonText)
            {
                EndTurn.GetComponentInChildren<Text>().text = "다음 턴";
                EndTurn.GetComponentInChildren<Text>().fontSize = 40;
            }
        }
    }

    IEnumerator updatePanelCoroutine()
    {
        while (true)
        {
            updatePanel();
            yield return new WaitForSeconds(1f);
        }
    }


    public void updatePanel()
    {
        GameManager.Instance.Game.PlayerInTurn.EstimateResourceInputs();

        double gold = GameManager.Instance.Game.PlayerInTurn.Gold;
        double goldTurn = GameManager.Instance.Game.PlayerInTurn.GoldNetIncome;
        if (goldTurn >= 0)
        {
            goldText.text = Math.Round(gold, 1) + "(<color=#00ff00>+" + Math.Round(goldTurn, 1) + "</color>)";
        }
        else
        {
            goldText.text = Math.Round(gold, 1) + "(<color=#ff0000>" + Math.Round(goldTurn, 1) + "</color>)";
        }

        double population = GameManager.Instance.Game.PlayerInTurn.Population;
        populationText.text = Math.Round(population, 1).ToString();

        double happiness = GameManager.Instance.Game.PlayerInTurn.Happiness;
        double happinessTurn = GameManager.Instance.Game.PlayerInTurn.HappinessIncome;
        if (happinessTurn >= 0)
        {
            happinessText.text = Math.Round(happiness, 1) + "(<color=#00ff00>+" + Math.Round(happinessTurn, 1) + "</color>)";
        }
        else
        {
            happinessText.text = Math.Round(happiness, 1) + "(<color=#ff0000>" + Math.Round(happinessTurn, 1) + "</color>)";
        }


        double research = GameManager.Instance.Game.PlayerInTurn.Research;
        double researchTurn = GameManager.Instance.Game.PlayerInTurn.ResearchIncome;
        researchText.text = Math.Round(research, 1) + "(<color=#00ff00>+" + Math.Round(researchTurn, 1) + "</color>)";

        double labor = GameManager.Instance.Game.PlayerInTurn.Labor;
        double usedLabor = GameManager.Instance.Game.PlayerInTurn.EstimatedUsedLabor;
        laborText.text = Math.Round(usedLabor, 1).ToString() + " / " + Math.Round(labor, 1).ToString();

        var game = GameManager.Instance.Game;
        var player = game.PlayerInTurn;

        Quest myVictory = game.GetPlayerHwan().Quests.OfType<QuestHwanVictory>().FirstOrDefault();
        Quest theirVictory = game.GetPlayerFinno().Quests.OfType<QuestFinnoVictory>().FirstOrDefault();
        if (player.Team == 1)
        {
            var tmp = myVictory;
            myVictory = theirVictory;
            theirVictory = myVictory;
        }

        if (myUltimateProgress != null)
            myUltimateProgress.GetComponent<Text>().text = "<color=#00ffff>" + (int)(GetProgressPercentage(myVictory) * 100) + "%</color>";
        if (oppositeUltimateProgress != null)
            oppositeUltimateProgress.GetComponent<Text>().text = "<color=#ff00ff>" + (int)(GetProgressPercentage(theirVictory) * 100) + "%</color>";
    }

    private double GetProgressPercentage(Quest quest)
    {
        return quest.Progresses
            .Where(p => p.Enabled)
            .Select(p => (double)p.Value / p.MaxValue)
            .Average();
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
            endingScene.GetComponentInChildren<Text>().text = "승리하셨습니다!";
            endingScene.SetActive(true);
			Music.transform.GetChild(GameInfo.UserPlayer).GetComponent<AudioSource>().Pause();
			Music.transform.GetChild(2).GetComponent<AudioSource>().Play();
			UIManager.Instance.MenuButton.SetActive(false);
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
            endingScene.GetComponentInChildren<Text>().text = "패배하셨습니다...";
            endingScene.SetActive(true);
			Music.transform.GetChild(GameInfo.UserPlayer).GetComponent<AudioSource>().Pause();
			Music.transform.GetChild(2).GetComponent<AudioSource>().Play();
			UIManager.Instance.MenuButton.SetActive(false);
        }
        else if (GameManager.Instance.Game.PlayerInTurn.IsDrawed)
        {
            endingScene.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Endings/Draw_ending");
            endingScene.GetComponentInChildren<Text>().text = "공멸하셨습니다...";
            endingScene.SetActive(true);
			Music.transform.GetChild(GameInfo.UserPlayer).GetComponent<AudioSource>().Pause();
			Music.transform.GetChild(2).GetComponent<AudioSource>().Play();
			UIManager.Instance.MenuButton.SetActive(false);
        }

    }

    public void onClickUltAlertExit()
    {
        UltAlert.SetActive(false);
    }

    private List<bool> prevEnemyUltimateProgress = null;
    private void NextTurn()
    {
        Game game = GameManager.Instance.Game;

        game.EndTurn();
        game.StartTurn();
        
        //Proceeds AI's Turns
        while (game.PlayerInTurn.IsAIControlled)
        {
            game.PlayerInTurn.DoAITurnAction().GetAwaiter().GetResult();
            game.EndTurn();
            game.StartTurn();
        }
        checkButtonText = true;
        EndTurn.GetComponentInChildren<Button>().interactable = true;

        Quest enemyVictoryQuest;
        if (game.PlayerInTurn.Team == 0)
            enemyVictoryQuest = game.GetPlayerFinno().Quests.OfType<CivModel.Quests.QuestFinnoVictory>().FirstOrDefault();
        else
            enemyVictoryQuest = game.GetPlayerHwan().Quests.OfType<CivModel.Quests.QuestHwanVictory>().FirstOrDefault();

        if (enemyVictoryQuest != null)
        {
            var newList = enemyVictoryQuest.Progresses.Where(p => p.Enabled).ToList();
            if (prevEnemyUltimateProgress == null)
            {
                prevEnemyUltimateProgress = newList.Select(p => p.IsFull).ToList();
            }
            else
            {
                bool notify = false;
                for (int idx = 0; idx < newList.Count; ++idx)
                {
                    if (!prevEnemyUltimateProgress[idx] && newList[idx].IsFull)
                    {
                        prevEnemyUltimateProgress[idx] = true;
                        notify = true;
                    }
                }
                if (notify)
                {
                    UltAlert.SetActive(true);
                }
            }
        }

        AlarmManager.Instance.updateAlarmQueue();

        GameManager.Instance.UpdateMap();
        GameManager.Instance.UpdateUnit();
        GameManager.Instance.UpdateMinimap();
        UIManager.Instance.ButtonInteractChange();

        managementcontroller.MakeProductionQ();
        managementcontroller.MakeDeploymentQ();
        uicontroller.MakeQuestQueue();

        GameManager.Instance.CheckNewQuest();
        GameManager.Instance.CheckCompletedProduction();

        GameManager.Instance.CheckToDo();
        if (GameManager.Instance.isThereTodos)
            GameManager.Instance.FocusOnNextActableUnit();

        GameUI.Instance.updatePanel();
		if(game.PlayerInTurn.Cities.First() != null)
		{
			if (GameManager.Instance.isThereTodos)
				GameManager.Instance.FocusOnNextActableUnit();
			else
                GameManager.Focus(game.PlayerInTurn.Cities.First());
		}

        foreach (GameObject unit in GameManager.Instance.Units)
            unit.GetComponent<Unit>().UpdateSkillCooldown();
        foreach (GameObject unit in GameManager.Instance.Additional_Units)
            unit.GetComponent<Unit>().UpdateSkillCooldown();
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

    public void BackToIntro()
    {
        LoadingSceneMgr.LoadScene("Intro");
    }

    public void RestartGame()
    {
        LoadingSceneMgr.LoadScene("Game");
    }

    private double Clamp(double a, double b, double x)
    {
        if (a > x) return a;
        else if (x > b) return b;
        else return x;
    }

    public void OnClickAutoSet()
    {
        var _plyrInTurn = GameManager.Instance.Game.PlayerInTurn;

        if (_plyrInTurn.Happiness < 0.0)
        {
            InvestmentController.I.eiSlider.value = 2f;
        }
        else if (_plyrInTurn.Happiness < 30.0)
        {
            InvestmentController.I.eiSlider.value = 1.1f;
        }
        else
        {
            InvestmentController.I.eiSlider.value = 1.0f;
        }

        _plyrInTurn.EstimateResourceInputs();



        var _gameConstants = GameManager.Instance.Game.Constants;

        double N = _gameConstants.GoldCoefficient;
        double LM = _gameConstants.EconomicRequireCoefficient;

        double t = Clamp(0.0, 1.0, (N / LM / 2.0 / InvestmentController.I.eiSlider.value));

        // Debug.Log(N / LM / 2.0 / InvestmentController.I.eiSlider.value);

        InvestmentController.I.taxSlider.value = (float) t;

        _plyrInTurn.EstimateResourceInputs();

        InvestmentController.I.tiSlider.value = 1.0f;

        InvestmentController.I.logiSlider.value = 1.0f;
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