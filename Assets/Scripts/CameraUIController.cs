using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;
using CivModel;
using CivModel.Common;

public class CameraUIController : MonoBehaviour {

    public Canvas CameraCanvas;
    public Canvas tempManagementUI;


    public Button MoveButton;
    public Button AttackButton;
    public Button SkillButton;
    public Button WaitButton;

    public GameObject UnitInfo;

    public Image Portrait;
    public Text UnitName;
    public Text UnitAttack;
    public Text UnitDefence;
    public Text UnitAP;
    public Text UnitHP;

    private GameObject gameManagerObject;
    private CIVGameManager gameManager;
    private Presenter mPresenter;

    public void MoveButtonMethod()
    {
        Debug.Log("MoveButton");
        if (gameManager.pointSelected.HasValue)
            mPresenter.CommandMove();
    }

    public void SkillButtonMethod()
    {
        Debug.Log("SkillButton");
        if (mPresenter.SelectedActor.GetType() == typeof(Pioneer))
        {
            mPresenter.SelectedActor.SpecialActs[0].Act(mPresenter.SelectedActor.PlacedPoint);
            Debug.Log("Pioneer set City");
        }
    }
    public void WaitButtonMethod()
    {
        Debug.Log("WaitButton");
        mPresenter.CommandSkip();
    }
    public void AttackButtonMethod()
    {
        Debug.Log("AttackButton");
        /*if(caseA)
            mPresenter.CommandMovingAttack();
           else(caseA)
            mPresenter.CommandHoldingAttack
        */
    }
    // Use this for initialization
    void Start ()
    {
        gameManagerObject = CIVGameManager.GetGameManager();
        gameManager = gameManagerObject.GetComponent<CIVGameManager>();
        mPresenter = gameManager.GetPresenter();

        tempManagementUI.enabled = false;
    }
	// Update is called once per frame
	void Update ()
    {
		if(mPresenter.SelectedActor == null)
        {
            MoveButton.enabled = false;
        }
        else
        {
            if (mPresenter.SelectedActor.RemainAP == 0)
            {
                MoveButton.enabled = false;
            }
            else
            {
                MoveButton.enabled = true;
            }
        }

        if(mPresenter.SelectedActor == null)
        {
            UnitInfo.SetActive(false);
            UnitName.text = "공허";
            UnitAttack.text = "공격력 : 무한";
            UnitDefence.text = "방어력 : 무한";
            UnitAP.text = "어디든지";
        }
        else
        {
            UnitInfo.SetActive(true);

            UnitName.text = mPresenter.SelectedActor.GetType().ToString().Replace("CivModel.Common.","");
            UnitAttack.text = "공격력 : " + mPresenter.SelectedActor.AttackPower.ToString();
            UnitDefence.text = "방어력 : " + mPresenter.SelectedActor.DefencePower.ToString();
            UnitAP.text = mPresenter.SelectedActor.RemainAP + "/" + mPresenter.SelectedActor.MaxAP;
            UnitHP.text = mPresenter.SelectedActor.RemainHP + "/" + mPresenter.SelectedActor.MaxHP;
        }
    }
}
