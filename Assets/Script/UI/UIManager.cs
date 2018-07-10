using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CivModel;
using CivModel.Common;

public class UIManager : MonoBehaviour
{
    GameObject mapUI;
	GameObject managementUI;
	GameObject questUI;
    GameObject selectedActor;
    GameObject SpecialSpec;
    GameObject skillSet;
    GameObject unitInfo;

    public Image UnitPortrait;

    public Ray ray;
    public RaycastHit hit;

	// Use this for initialization
	void Start() {
        mapUI = GameObject.Find("MapUI");
		managementUI = GameObject.Find("ManagementUI");
		questUI = GameObject.Find("QuestUI");
        SpecialSpec = GameObject.Find("SpecialSpec");
        skillSet = GameObject.Find("Skill Set");
        unitInfo = GameObject.Find("UnitInfo");
        UnitPortrait = GameObject.Find("Portrait").GetComponent<Image>();

        managementUI.SetActive(false);
        questUI.SetActive(false);
        SpecialSpec.SetActive(false);
        skillSet.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) {
            if(Physics.Raycast(ray, out hit)){
                selectedActor = hit.transform.gameObject;
				Debug.Log(selectedActor.name);
				HexTile tile = selectedActor.GetComponent<HexTile>();
				GameManager.Instance.selectedTile = tile;
				GameManager.Instance.selectedPoint = tile.point;

				IEnumerator _tileCoroutine = FlickerSelectedTile(tile);
				StartCoroutine(_tileCoroutine);

                if (tile.point.Unit != null && tile.point.TileBuilding != null) {
                    if(tile.isFirstClick) {
                        GameManager.Instance.selectedActor = tile.point.Unit;
                        tile.isFirstClick = false;
                        GameManager.Instance.selectedGameObject = GameManager.GetUnitGameObject(tile.point);
                    } else {
                        GameManager.Instance.selectedActor = tile.point.TileBuilding;
                        tile.isFirstClick = true;
                        GameManager.Instance.selectedGameObject = selectedActor;
                    }
                } else if (tile.point.TileBuilding != null) {
                    GameManager.Instance.selectedActor = tile.point.TileBuilding;
                    tile.isFirstClick = true;
                    GameManager.Instance.selectedGameObject = selectedActor;
                } else if (tile.point.Unit != null) {
                    GameManager.Instance.selectedActor = tile.point.Unit;
                    tile.isFirstClick = false;
                    GameManager.Instance.selectedGameObject = GameManager.GetUnitGameObject(tile.point);
                }
                else
                {
                    GameManager.Instance.selectedActor = null;
                }
            }
        }
        
        if (GameManager.Instance.selectedActor != null)
        {
            unitInfo.SetActive(true);
            UnitPortrait.sprite = Resources.Load(("Portraits/" + (ProductionFactoryTraits.GetPortName(GameManager.Instance.selectedActor)).ToLower()), typeof(Sprite)) as Sprite;
            GameObject.Find("UnitName").GetComponent<Text>().text = ProductionFactoryTraits.GetName(GameManager.Instance.selectedActor);
            GameObject.Find("UnitAttack").GetComponent<Text>().text = GameManager.Instance.selectedActor.AttackPower.ToString();
            GameObject.Find("UnitDefence").GetComponent<Text>().text = GameManager.Instance.selectedActor.DefencePower.ToString();
            GameObject.Find("UnitEffect").GetComponent<Text>().text = GameManager.Instance.selectedActor.RemainHP.ToString() + "/" + GameManager.Instance.selectedActor.MaxHP;
            GameObject.Find("ActionPoint").GetComponent<Text>().text = GameManager.Instance.selectedActor.RemainAP.ToString() + "/" + GameManager.Instance.selectedActor.MaxAP;
            GameObject.Find("HealthPoint").GetComponent<RectTransform>().sizeDelta = new Vector2(30, 280 * (float)GameManager.Instance.selectedActor.RemainHP / (float)GameManager.Instance.selectedActor.MaxHP);
        }
        else unitInfo.SetActive(false);

        if (SpecialSpec.activeSelf == true)
        {
            string specialText = "Sample";
            // Queue about specialities needed
            GameObject.Find("SpecialSpecText").GetComponent<Text>().text = specialText;
        }
    }

	IEnumerator FlickerSelectedTile(HexTile prevTile) {
		while (true) {
			if (prevTile != GameManager.Instance.selectedTile) {
				prevTile.StopFlickering();
				GameManager.Instance.selectedTile.FlickerCyan();
				break;
			}
			yield return null;
		}
	}

    public void onClick(GameObject go)
    {
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

    public void onClickMove() {
        if (GameManager.Instance.selectedActor is CivModel.Unit) {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().MoveStateEnter();
        }
    }
    public void onClickAttack() {
        if (GameManager.Instance.selectedActor is CivModel.Unit) {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().AttackStateEnter();
        }
    }
    public void onClickSkill() {
        if (skillSet.activeSelf == false)
            skillSet.SetActive(true);
        else skillSet.SetActive(false);
    }
    public void onClickSkillBtn(int idx) {
        if (GameManager.Instance.selectedActor is CivModel.Unit) {
            GameManager.Instance.selectedGameObject.GetComponent<Unit>().SkillStateEnter(idx);
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
}