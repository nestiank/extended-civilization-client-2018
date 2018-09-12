using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using CivModel;
using System.Linq;

using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Finno.FinnoPlayerNumber;

public class UIManager : MonoBehaviour
{

    // Get UI GameObjects from the Scene
    public GameObject mapUI;
    public GameObject managementUI;
    public GameObject diplomacyUI;
    public GameObject questUI;
    public GameObject selectedActor;
    public GameObject SpecialSpec;
	public GameObject Menu;
	public GameObject AskToQuit;
    public GameObject ActivatedUI;

    public GameObject spyPanel;
    public GameObject spyContent;

    public GameObject UnitSelTab;
    public GameObject BuildingSelTab;
    public GameObject EpicTab, HighTab, IntermediateTab, LowTab;    // Unit production
    public GameObject CityTab, CityBuildingTab, NormalBuildingTab;  // Building production

    public GameObject skillSet;
    public GameObject skillDescription;
    public GameObject unitInfo;

    public GameObject Actions;
    public GameObject moveBtn;
    public GameObject attackBtn;
    public GameObject skipBtn;

    public GameObject unitName;
    public GameObject unitAttack;
    public GameObject unitDefence;
    public GameObject unitHP;
    public GameObject unitEffect;
    public GameObject actionPoint;
    public GameObject healthPoint;
    public GameObject cityBuildingInfo;

    public GameObject MainTutorial;
    public GameObject ProductionTutorial;
    public GameObject QuestTutorial;

    public GameObject UltimateView;
    public GameObject UltimateText;

	public GameObject HwanMenuButton, FinnoMenuButton;
    public GameObject MenuButton;


    private bool isFirstProduction = true;
    private bool isFirstQuest = true;
    public bool isTutorialActivated = true;

    public Image UnitPortrait;

    public GameObject QuestComplete;
    public GameObject BGMs;
    // RayCast For Selection
    public Ray ray;
    public RaycastHit hit;

    // UIManager Class Instance Singleton
    private static UIManager _manager = null;
    public static UIManager Instance { get { return _manager; } }
    private float skillSet_x;

    private void Awake()
    {
        if (_manager != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _manager = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        UnitPortrait = GameObject.Find("Portrait").GetComponent<Image>();

        Actions.SetActive(false);
        managementUI.SetActive(false);
        diplomacyUI.SetActive(false);
        questUI.SetActive(false);
        SpecialSpec.SetActive(false);
        //skillSet.SetActive(false);
        cityBuildingInfo.SetActive(false);
        mapUI.transform.GetChild(1).gameObject.SetActive(false);
        QuestComplete.SetActive(false);
        spyPanel.SetActive(false);
        skillSet_x = skillSet.GetComponent<RectTransform>().sizeDelta.x;
        ActivatedUI = mapUI;
        TutorialManagement(ActivatedUI);
        UltimateView.SetActive(false);

		HwanMenuButton.SetActive(false);
		FinnoMenuButton.SetActive(false);
		if (GameInfo.UserPlayer == 0)
		{
			HwanMenuButton.SetActive(true);
			MenuButton = HwanMenuButton;
		}
		else
		{
			FinnoMenuButton.SetActive(true);
			MenuButton = FinnoMenuButton;
		}
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Selecting Actor(Tile, Unit) of the Game
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0) && !isTutorialActivated)
        {
            // layer mask to index-11~14 layers
            int layermask = (1 << 11) + (1 << 12) + (1 << 13) + (1 << 14);
            layermask = ~layermask;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask))
            {
                selectedActor = hit.transform.gameObject;
                HexTile tile = selectedActor.GetComponent<HexTile>();
                if (tile == null) return;
                // Update selectedTile
                GameManager.Instance.selectedTile = tile;
                // Update selectedPoint using tile information
                GameManager.Instance.selectedPoint = tile.point;
                
                if (tile.point.Unit != null && tile.point.TileBuilding != null)
                {
                    
                    if (tile.isFirstClick)
                    {
                        GameManager.Instance.SelectActor(tile.point.Unit);
                        tile.isFirstClick = false;
                        GameManager.Instance.selectedGameObject = GameManager.GetUnitGameObject(tile.point);
                    }
                    else
                    {
                        GameManager.Instance.SelectActor(tile.point.TileBuilding);
                        tile.isFirstClick = true;
                        GameManager.Instance.selectedGameObject = selectedActor;
                    }
                }
                else if (tile.point.TileBuilding != null)
                {
                    GameManager.Instance.SelectActor(tile.point.TileBuilding);
                    tile.isFirstClick = true;
                    GameManager.Instance.selectedGameObject = selectedActor;
                }
                else if (tile.point.Unit != null)
                {
                    GameManager.Instance.SelectActor(tile.point.Unit);
                    tile.isFirstClick = false;
                    GameManager.Instance.selectedGameObject = GameManager.GetUnitGameObject(tile.point);
                }
                // If neither Unit nor TileBuilding exists on the selected tile
                else
                {
                    GameManager.Instance.selectedActor = null;
                }

               
                //if (GameManager.Instance.selectedActor != null)
                //Debug.Log(GameManager.Instance.selectedActor.ToString());

                // Set Unit Information
                UpdateUnitInfo();

                // Change Button Interaction correponds to the selected Actor
                ButtonInteractChange();

                // Filcker Selected Tile with Cyan
                // IEnumerator _tileCoroutine = FlickerSelectedTile(tile);
                // StartCoroutine(_tileCoroutine);
            }


           
        }
        if(SpecialSpec.activeSelf == true)
            SpecialSpec.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            managementUI.SetActive(false);
            diplomacyUI.SetActive(false);
            questUI.SetActive(false);
            mapUI.SetActive(true);
        }
        skillDescription.transform.position = Input.mousePosition;

        if (isTutorialActivated)
            if (Input.GetMouseButtonDown(0))
            {
                MainTutorial.SetActive(false);
                ProductionTutorial.SetActive(false);
                QuestTutorial.SetActive(false);
                isTutorialActivated = false;
            }
    }
    // Set Unit Information
    public void UpdateUnitInfo()
    {
        if (questUI.activeSelf == false && managementUI.activeSelf == false && diplomacyUI.activeSelf == false)
        {
            if (GameManager.Instance.selectedActor != null)
            {
                unitInfo.SetActive(true);
                actionPoint.SetActive(true);
                skillSet.GetComponent<RectTransform>().anchoredPosition = new Vector2(140, 192.5f);
                if (GameManager.Instance.selectedActor is CivModel.Actor)
                {
                    UnitPortrait.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetPortName(GameManager.Instance.selectedActor)).ToLower()), typeof(Sprite)) as Sprite;
                    unitName.GetComponent<Text>().text = GameManager.Instance.selectedActor.TextName; //ProductionFactoryTraits.GetName(GameManager.Instance.selectedActor);
                }

                unitAttack.GetComponent<Text>().text = "공격력: " + GameManager.Instance.selectedActor.AttackPower.ToString();
                unitDefence.GetComponent<Text>().text = "방어력: " + GameManager.Instance.selectedActor.DefencePower.ToString();
                unitHP.GetComponent<Text>().text = GameManager.Instance.selectedActor.RemainHP.ToString() + "/" + GameManager.Instance.selectedActor.MaxHP;
                actionPoint.GetComponent<Text>().text = "행동력: " + GameManager.Instance.selectedActor.RemainAP.ToString() + "/" + GameManager.Instance.selectedActor.MaxAP;
                healthPoint.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 280 * (float)GameManager.Instance.selectedActor.RemainHP / (float)GameManager.Instance.selectedActor.MaxHP);
                if (GameManager.Instance.selectedActor.RemainHP / GameManager.Instance.selectedActor.MaxHP * 100 > 66)
                    healthPoint.GetComponent<Image>().color = Color.green;
                else if (GameManager.Instance.selectedActor.RemainHP / GameManager.Instance.selectedActor.MaxHP * 100 > 33)
                    healthPoint.GetComponent<Image>().color = Color.yellow;
                else
                    healthPoint.GetComponent<Image>().color = Color.red;
                if (GameManager.Instance.selectedActor.PassiveSkills == null)
                    unitEffect.SetActive(false);
                else
                {
                    string _passive = "";
                    for (int i = 0; i < GameManager.Instance.selectedActor.PassiveSkills.Count; i++)
                    {
                        _passive += GameManager.Instance.selectedActor.PassiveSkills[i].SkillName + "\n";
                    }
                    unitEffect.GetComponent<Text>().text = _passive;
                }

                
                if(GameManager.Instance.selectedActor is CivModel.TileBuilding)
                {
                    // CityBase 한정
                    if (GameManager.Instance.selectedActor is CivModel.CityBase)
                    {
                        unitName.GetComponent<Text>().text = ((CityBase)GameManager.Instance.selectedActor).CityName;
                        UnitPortrait.sprite = CityBuilding.GetPortraiteImage((CivModel.CityBase)GameManager.Instance.selectedActor);
                        cityBuildingInfo.SetActive(true);
                        cityBuildingInfo.GetComponentInChildren<Text>().text = CityBuilding.ListCityBuildings(((CityBase)GameManager.Instance.selectedActor).InteriorBuildings);
                        actionPoint.SetActive(false);
                        skillSet.GetComponent<RectTransform>().anchoredPosition = new Vector2(1, 192.5f);
                        string _providing = "";
                        if (((CityBase)GameManager.Instance.selectedActor).ProvidedGold != 0) _providing += "골드 생산량: " + ((CityBase)GameManager.Instance.selectedActor).ProvidedGold + "\n";
                        if (((CityBase)GameManager.Instance.selectedActor).ProvidedHappiness != 0) _providing += "행복도 생산량: " + ((CityBase)GameManager.Instance.selectedActor).ProvidedHappiness + "\n";
                        if (((CityBase)GameManager.Instance.selectedActor).ProvidedLabor != 0) _providing += "노동력 생산량: " + ((CityBase)GameManager.Instance.selectedActor).ProvidedLabor;
                        unitEffect.GetComponent<Text>().text += _providing;
                    }
                    else
                    {
                        cityBuildingInfo.SetActive(false);
                        UnitPortrait.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetPortName(GameManager.Instance.selectedActor)).ToLower()), typeof(Sprite)) as Sprite;
                        unitName.GetComponent<Text>().text = GameManager.Instance.selectedActor.TextName; //ProductionFactoryTraits.GetName(GameManager.Instance.selectedActor);
                    }
                }

            }
            else unitInfo.SetActive(false);
        }
    }

    // Flicker Selected Tile With Cyan
    IEnumerator FlickerSelectedTile(HexTile prevTile)
    {
        while (true)
        {
            if (prevTile != GameManager.Instance.selectedTile)
            {
                prevTile.StopFlickering();
                if(GameManager.Instance.selectedActor == null)
                {
                    GameManager.Instance.selectedTile.FlickerWhite();
                }
                else
                {
                    GameManager.Instance.selectedTile.FlickerCyan();
                }
                break;
            }
            yield return null; 
        }
    }

    // On Click UI Button
    public void onClick(GameObject go)
    {
        if (GameManager.Instance.selectedGameObject != null)
        {
            if (GameManager.Instance.selectedGameObject.GetComponent<Unit>() != null)
            {
                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateExit();

                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().AttackState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateExit();

                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillStateExit();
            }
        }
        if (go.activeSelf == false)
        {
            go.SetActive(true);
            if (go != mapUI) mapUI.SetActive(false);
            if (go != managementUI) managementUI.SetActive(false);
            if (go != diplomacyUI) diplomacyUI.SetActive(false);
            if (go != questUI) questUI.SetActive(false);
            ActivatedUI = go;
            if (go == questUI && isFirstQuest)
            {
                TutorialManagement(ActivatedUI);
                isFirstQuest = false;
            }
            if (go == managementUI && isFirstProduction)
            {
                TutorialManagement(ActivatedUI);
                isFirstProduction = false;
            }
        }
        else
            if (go != mapUI)
        {
            go.SetActive(false);
            mapUI.SetActive(true);
            ActivatedUI = mapUI;
        }
    }

	public void onClickMenu()
	{
		if (Menu.activeSelf)
			Menu.SetActive(false);
		else
			Menu.SetActive(true);
	}

	//only exclusive to game quit button
	public void onClickYesNo(GameObject but)
	{
		if (but.name == "Yes")
		{
			Application.Quit();
		}
		else if (but.name == "No")
			AskToQuit.SetActive(false);
	}

	public void onClickMenuButton(GameObject but)
	{
		if (but.name == "QuitGame")
		{
			AskToQuit.SetActive(true);
		}
	}

    // On Click Move Button
    public void onClickMove()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateEnter();
        }
    }
    // On Click Attack Button
    public void onClickAttack()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().AttackStateEnter();
        }
    }
    // On Click Skill Button
    public void onClickSkill()
    {
        if (skillSet.activeSelf == false)
            skillSet.SetActive(true);
        else skillSet.SetActive(false);
    }
    // On Click a Specific Skill
    public void onClickSkillBtn(int idx)
    {
        if (GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            // Unit Skill
            if (GameManager.Instance.selectedActor is CivModel.Unit)
                GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillStateEnter(idx);
            // Tile Building Skill
            else if (GameManager.Instance.selectedActor is CivModel.TileBuilding)
            {
                GameManager.Instance.selectedTile.SkillStateEnter(idx);
            }
            else Debug.Log("Cannot Use Skill");
        }
    }
    // On Click Skip Button
    public void onClickSkip()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedActor.SleepFlag = true;
        }
        ButtonInteractChange();
        GameManager.Instance.CheckToDo();
        GameManager.Instance.FocusOnNextActableUnit();
       if(GameManager.Instance.selectedActor == null)
        {
            unitInfo.SetActive(false);
            Actions.SetActive(false);
        }
    }

    public void SpecialMouseOver()
    {
        SpecialSpec.SetActive(true);
    }
    public void SpecialMouseExit()
    {
        SpecialSpec.SetActive(false);
    }

    public void MyUltimateMouseOver()
    {
        UltimateView.SetActive(true);
        var component = UltimateText.GetComponent<Text>();

        var player = GameManager.Instance.Game.PlayerInTurn;

        CivModel.Quest quest = null;

        // Player가 환인경우
        if (player == GameManager.Instance.Game.GetPlayerHwan())
        {
            quest = player.Quests.OfType<CivModel.Quests.QuestHwanVictory>().FirstOrDefault();
        }
        // Player가 피노인경우
        else
        {
            quest = player.Quests.OfType<CivModel.Quests.QuestFinnoVictory>().FirstOrDefault();
        }

        if (quest != null)
        {
            var str = quest.Progresses
                .Where(p => p.Enabled)
                .Aggregate("", (s, p) => s + p.Description + " [" + p.Value + "/" + p.MaxValue + "]\n")
                .TrimEnd();

            component.text = str;
        }
    }

    public void OppositeUltimateMouseOver()
    {
        UltimateView.SetActive(true);
        var component = UltimateText.GetComponent<Text>();

        var player = GameManager.Instance.Game.PlayerInTurn;

        CivModel.Quest quest = null;

        // Player가 환인경우
        if (player == GameManager.Instance.Game.GetPlayerHwan())
        {
            player = GameManager.Instance.Game.GetPlayerFinno();
            quest = player.Quests.OfType<CivModel.Quests.QuestFinnoVictory>().FirstOrDefault();
        }
        // Player가 피노인경우
        else
        {
            player = GameManager.Instance.Game.GetPlayerHwan();
            quest = player.Quests.OfType<CivModel.Quests.QuestHwanVictory>().FirstOrDefault();
        }

        if (quest != null)
        {
            var str = quest.Progresses
                .Where(p => p.Enabled)
                .Aggregate("", (s, p) => s + p.Description + " [" + p.Value + "/" + p.MaxValue + "]\n")
                .TrimEnd();

            component.text = str;
        }
    }

    public void UltimateMouseOver()
    {
        UltimateView.SetActive(true);
        var component = UltimateText.GetComponent<Text>();

        var player = GameManager.Instance.Game.GetPlayerHwan();
        var quest = player.Quests.OfType<CivModel.Quests.QuestHwanVictory>().FirstOrDefault();
        if (quest != null)
        {
            var str = quest.Progresses
                .Where(p => p.Enabled)
                .Aggregate("", (s, p) => s + p.Description + " [" + p.Value + "/" + p.MaxValue + "]\n")
                .TrimEnd();

            component.text = str;
        }
    }

    public void UltimateMouseExit()
    {
        UltimateView.SetActive(false);
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

    // Change Button Interaction according to selectedActor
    public void ButtonInteractChange()
    {
        // Hide Actions Tab
        if(GameManager.Instance.selectedActor == null || GameManager.Instance.selectedActor.Owner != GameManager.Instance.Game.PlayerInTurn || GameManager.Instance.selectedActor is CityBase)
        {
            Actions.SetActive(false);
            skillSet.SetActive(false);
        } else
        {
            Actions.SetActive(true);
            skillSet.SetActive(true);
        }
        // Move Button
        if(GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn && GameManager.Instance.selectedActor.RemainAP != 0)
        {
            moveBtn.GetComponent<Button>().interactable = true;
        } else
        {
            moveBtn.GetComponent<Button>().interactable = false;
        }

        // Attack Button
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn && GameManager.Instance.selectedActor.RemainAP != 0)
        {
            attackBtn.GetComponent<Button>().interactable = true;
        }
        else
        {
            attackBtn.GetComponent<Button>().interactable = false;
        }

        // SkipButton
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            if (GameManager.Instance.selectedActor.SkipFlag == false)
                skipBtn.GetComponent<Button>().interactable = true;
            else
                skipBtn.GetComponent<Button>().interactable = false;
        } else
        {
            skipBtn.GetComponent<Button>().interactable = false;
        }

        // SkillButton
        if (GameManager.Instance.selectedActor is CivModel.Actor && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            if (GameManager.Instance.selectedActor.SpecialActs == null)
                skillSet.SetActive(false);
            else
            {
                skillSet.GetComponent<RectTransform>().sizeDelta = new Vector2(skillSet_x * GameManager.Instance.selectedActor.SpecialActs.Count / 3
                    , skillSet.GetComponent<RectTransform>().sizeDelta.y);
                Button[] skillsBtn = skillSet.GetComponentsInChildren<Button>();
                foreach (var skill in skillsBtn)
                {
                    skill.gameObject.SetActive(true);
                    skill.interactable = true;
                }
                int skillIdx;
                for (skillIdx = 0; skillIdx < GameManager.Instance.selectedActor.SpecialActs.Count; ++skillIdx)
                {
                    SkillInfo si;
                    if (skillIdx < GameManager.Instance.selectedActor.ActiveSkills.Count)
                        si = GameManager.Instance.selectedActor.ActiveSkills[skillIdx];
                    
                    else si = new SkillInfo { SkillName = "null", SkillDescription = "" };
                    skillsBtn[skillIdx].GetComponentsInChildren<Image>().Last().sprite = SkillButton.GetSkillIcon(GameManager.Instance.selectedActor, skillIdx);
                }

                foreach (var skill in skillsBtn.Skip(skillIdx))
                {
                    skill.gameObject.SetActive(false);
                }

                if (GameManager.Instance.selectedActor is CivModel.Unit)
                {
                    if(GameManager.Instance.selectedActor.RemainAP.CompareTo(0) == 0)
                    {
                        Button[] skillsBtnNoAP = skillSet.GetComponentsInChildren<Button>();
                        foreach (var skill in skillsBtnNoAP)
                        {
                            skill.interactable = false;
                        }
                    }
                    GameObject unitGameObject = GameManager.GetUnitGameObject(GameManager.Instance.selectedPoint);
                    if (unitGameObject != null)
                    {
                        Unit unit = unitGameObject.GetComponent<Unit>();
                        if (unit.skillCooldown > 0)
                        {
                            Button[] skillsBtnNoAP = skillSet.GetComponentsInChildren<Button>();
                            foreach (var skill in skillsBtnNoAP)
                            {
                                skill.interactable = false;
                            }
                        }
                    }
                    
                }
            }
        }

        if (GameManager.Instance.selectedActor is CivModel.CityBase && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            skillSet.SetActive(true);

            skillSet.GetComponent<RectTransform>().sizeDelta = new Vector2(skillSet_x * GameManager.Instance.selectedActor.SpecialActs.Count / 3
                , skillSet.GetComponent<RectTransform>().sizeDelta.y);
            Button[] skillsBtn = skillSet.GetComponentsInChildren<Button>();
            foreach (var skill in skillsBtn)
            {
                skill.gameObject.SetActive(true);
                skill.interactable = true;
            }
            int skillIdx;
            for (skillIdx = 0; skillIdx < GameManager.Instance.selectedActor.SpecialActs.Count; ++skillIdx)
            {
                SkillInfo si;
                if (skillIdx < GameManager.Instance.selectedActor.ActiveSkills.Count)
                    si = GameManager.Instance.selectedActor.ActiveSkills[skillIdx];
                else si = new SkillInfo { SkillName = "null", SkillDescription = "" };
                skillsBtn[skillIdx].GetComponentsInChildren<Image>().Last().sprite = SkillButton.GetSkillIcon(GameManager.Instance.selectedActor, skillIdx);
            }

            foreach (var skill in skillsBtn.Skip(skillIdx))
            {
                skill.gameObject.SetActive(false);
            }
        }
    }

    // Update Unit Info & selectedActor information according to the given actor
    public void updateSelectedInfo(CivModel.Actor actor)
    {
        GameManager.Instance.SelectActor(actor);
        var pt = actor.PlacedPoint.Value;
        GameManager.Instance.selectedPoint = pt;
        foreach (GameObject unit in GameManager.Instance.Units)
        {
            if (unit.GetComponent<Unit>().point == pt)
            {
                GameManager.Instance.selectedGameObject = unit;
                break;
            }
        }
        GameManager.Instance.selectedTile = GameManager.Instance.Tiles[pt.Position.X, pt.Position.Y].GetComponent<HexTile>();
        Instance.Actions.SetActive(true);
        UpdateUnitInfo();
        ButtonInteractChange();
    }

    //set what to show on QuestComplete Scene
    public void SetQuestComplete(Quest qst)
    {
        Image Qstimage = QuestComplete.GetComponentInChildren<Image>();
        Text text = QuestComplete.GetComponentInChildren<Text>();
        Qstimage.sprite = QuestInfo.GetPortraitImage(qst);
        text.text = qst.CompleteNotice;
        MenuButton.SetActive(false);
    }

    public void QuestCompleteExit()
    {
        QuestComplete.SetActive(false);
        MenuButton.SetActive(true);
        GameManager.Instance.StopQuestVoice();
        if (GameManager.Instance.HolySound.GetComponent<AudioSource>().isPlaying)
            GameManager.Instance.HolySound.GetComponent<AudioSource>().Stop();
            if (GameInfo.UserPlayer == 0)
            BGMs.transform.GetChild(0).GetComponent<AudioSource>().UnPause();
        if (GameInfo.UserPlayer == 1)
            BGMs.transform.GetChild(1).GetComponent<AudioSource>().UnPause();
    }

    public void OnClickBack()
    {
        spyPanel.SetActive(false);
    }

    //activate tutorial based on input tab
    public void TutorialManagement(GameObject ActivatedUI)
    {
        if(ActivatedUI == mapUI)
        {
            MainTutorial.SetActive(true);
            ProductionTutorial.SetActive(false);
            questUI.SetActive(false);
        }
        else if (ActivatedUI == managementUI)
        {
            MainTutorial.SetActive(false);
            ProductionTutorial.SetActive(true);
            QuestTutorial.SetActive(false);
        }
        else
        {
            MainTutorial.SetActive(false);
            ProductionTutorial.SetActive(false);
            QuestTutorial.SetActive(true);
        }
        isTutorialActivated = true;
    }
    public void onTutotialButtonClicked()
    {
        if(!isTutorialActivated)
            TutorialManagement(ActivatedUI);
        else
        {
            MainTutorial.SetActive(false);
            ProductionTutorial.SetActive(false);
            QuestTutorial.SetActive(false);
        }
    }
}