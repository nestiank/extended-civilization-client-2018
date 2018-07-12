using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CivModel;
using CivModel.Common;

public class UIManager : MonoBehaviour
{
    public GameObject mapUI;
    public GameObject managementUI;
    public GameObject questUI;
    public GameObject selectedActor;
    public GameObject SpecialSpec;

    public GameObject UnitSelTab;
    public GameObject BuildingSelTab;
    public GameObject EpicTab, HighTab, IntermediateTab, LowTab;    // Unit production
    public GameObject CityTab, CityBuildingTab, NormalBuildingTab;  // Building production

    GameObject skillSet;
    GameObject unitInfo;

    GameObject Actions;
    GameObject moveBtn;
    GameObject attackBtn;
    GameObject skipBtn;
    GameObject skillBtn;

    public Image UnitPortrait;

    public Ray ray;
    public RaycastHit hit;

    private static UIManager _manager = null;
    public static UIManager Instance { get { return _manager; } }

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
        mapUI = GameObject.Find("MapUI");
        managementUI = GameObject.Find("ManagementUI");
        questUI = GameObject.Find("QuestUI");
        SpecialSpec = GameObject.Find("SpecialSpec");
        skillSet = GameObject.Find("Skill Set");
        unitInfo = GameObject.Find("UnitInfo");
        UnitPortrait = GameObject.Find("Portrait").GetComponent<Image>();

        Actions = GameObject.Find("Actions");
        moveBtn = GameObject.Find("Move");
        attackBtn = GameObject.Find("Attact");
        skipBtn = GameObject.Find("Wait");
        skillBtn = GameObject.Find("Skill");

        Actions.SetActive(false);
        managementUI.SetActive(false);
        questUI.SetActive(false);
        SpecialSpec.SetActive(false);
        skillSet.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                selectedActor = hit.transform.gameObject;
                //Debug.Log(selectedActor.name);
                HexTile tile = selectedActor.GetComponent<HexTile>();
                GameManager.Instance.selectedTile = tile;
                GameManager.Instance.selectedPoint = tile.point;

                if (tile.point.Unit != null && tile.point.TileBuilding != null)
                {
                    if (tile.isFirstClick)
                    {
                        GameManager.Instance.selectedActor = tile.point.Unit;
                        tile.isFirstClick = false;
                        GameManager.Instance.selectedGameObject = GameManager.GetUnitGameObject(tile.point);
                    }
                    else
                    {
                        GameManager.Instance.selectedActor = tile.point.TileBuilding;
                        tile.isFirstClick = true;
                        GameManager.Instance.selectedGameObject = selectedActor;
                    }
                }
                else if (tile.point.TileBuilding != null)
                {
                    GameManager.Instance.selectedActor = tile.point.TileBuilding;
                    tile.isFirstClick = true;
                    GameManager.Instance.selectedGameObject = selectedActor;
                }
                else if (tile.point.Unit != null)
                {
                    GameManager.Instance.selectedActor = tile.point.Unit;
                    tile.isFirstClick = false;
                    GameManager.Instance.selectedGameObject = GameManager.GetUnitGameObject(tile.point);
                }
                else
                {
                    GameManager.Instance.selectedActor = null;
                }
                ButtonInteractChange();
                IEnumerator _tileCoroutine = FlickerSelectedTile(tile);
                StartCoroutine(_tileCoroutine);
            }
        }

        if (GameManager.Instance.selectedActor != null)
        {
            unitInfo.SetActive(true);
            UnitPortrait.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetPortName(GameManager.Instance.selectedActor)).ToLower()), typeof(Sprite)) as Sprite;
            if(GameManager.Instance.selectedActor is CivModel.Unit) {
                GameObject.Find("UnitName").GetComponent<Text>().text = ProductionFactoryTraits.GetName(GameManager.Instance.selectedActor);
            } else if(GameManager.Instance.selectedActor is CivModel.CityBase) {
                GameObject.Find("UnitName").GetComponent<Text>().text = GameManager.Instance.selectedActor.Name;
            }
            GameObject.Find("UnitAttack").GetComponent<Text>().text = GameManager.Instance.selectedActor.AttackPower.ToString();
            GameObject.Find("UnitDefence").GetComponent<Text>().text = GameManager.Instance.selectedActor.DefencePower.ToString();
            GameObject.Find("UnitEffect").GetComponent<Text>().text = GameManager.Instance.selectedActor.RemainHP.ToString() + "/" + GameManager.Instance.selectedActor.MaxHP;
            GameObject.Find("ActionPoint").GetComponent<Text>().text = GameManager.Instance.selectedActor.RemainAP.ToString() + "/" + GameManager.Instance.selectedActor.MaxAP;
            GameObject.Find("HealthPoint").GetComponent<RectTransform>().sizeDelta = new Vector2(30, 280 * (float)GameManager.Instance.selectedActor.RemainHP / (float)GameManager.Instance.selectedActor.MaxHP);
        }
        else unitInfo.SetActive(false);

        if (SpecialSpec.activeSelf == true)
        {
            string specialText = "";
            GameObject.Find("SpecialSpecText").GetComponent<Text>().text = specialText;
        }
    }

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

    public void onClick(GameObject go)
    {
        if (GameManager.Instance.selectedGameObject != null)
        {
            if (GameManager.Instance.selectedGameObject.GetComponent<Unit>() != null)
            {
                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateExit();

                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().AttackState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().AttackStateExit();

                if (GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillState)
                    GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillStateExit(GameManager.Instance.selectedActor);
            }
        }
        if (go.activeSelf == false)
        {
            go.SetActive(true);
            if (go != mapUI) mapUI.SetActive(false);
            if (go != managementUI) managementUI.SetActive(false);
            if (go != questUI) questUI.SetActive(false);
        }
        else
        {
            go.SetActive(false);
            if (go != mapUI) mapUI.SetActive(true);
        }
    }



    public void onClickMove()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateEnter();
        }
    }
    public void onClickAttack()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().AttackStateEnter();
        }
    }
    public void onClickSkill()
    {
        if (skillSet.activeSelf == false)
            skillSet.SetActive(true);
        else skillSet.SetActive(false);
    }
    public void onClickSkillBtn(int idx)
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillStateEnter(idx);
        }
    }

    public void onClickSkip()
    {
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            GameManager.Instance.selectedActor.SkipFlag = true;
        }
        ButtonInteractChange();
    }

    public void SpecialMouseOver()
    {
        SpecialSpec.SetActive(true);
    }
    public void SpecialMouseExit()
    {
        SpecialSpec.SetActive(false);
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



    private void ButtonInteractChange()
    {
        // Hide Actions Tab
        if(GameManager.Instance.selectedActor == null || GameManager.Instance.selectedActor.Owner != GameManager.Instance.Game.PlayerInTurn)
        {
            Actions.SetActive(false);
        } else
        {
            Actions.SetActive(true);
        }
        // Move Button
        if(GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            moveBtn.GetComponent<Button>().interactable = true;
        } else
        {
            moveBtn.GetComponent<Button>().interactable = false;
        }

        // Attack Button
        if (GameManager.Instance.selectedActor is CivModel.Unit && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
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
            {
                skipBtn.GetComponent<Button>().interactable = true;
            } else
            {
                skipBtn.GetComponent<Button>().interactable = false;
            }
        } else
        {
            skipBtn.GetComponent<Button>().interactable = false;
        }

        // SkillButton
        if (GameManager.Instance.selectedActor is CivModel.Actor && GameManager.Instance.selectedActor.Owner == GameManager.Instance.Game.PlayerInTurn)
        {
            if (GameManager.Instance.selectedActor.SpecialActs != null)
            {
                skillBtn.GetComponent<Button>().interactable = true;

                Button[] skillsBtn = skillSet.GetComponentsInChildren<Button>();
                foreach (var skill in skillsBtn) {
                    skill.interactable = false;
                }
                int skillIdx = 0;
                foreach(var skill in GameManager.Instance.selectedActor.SpecialActs) {
                    skillsBtn[skillIdx].interactable = true;
                    skillsBtn[skillIdx].GetComponentInChildren<Text>().text = GameManager.Instance.selectedActor.SpecialActs[skillIdx].Owner.Name;
                    skillIdx++;
                }

            }
            else
            {
                skillBtn.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            skillBtn.GetComponent<Button>().interactable = false;
        }
    }
}