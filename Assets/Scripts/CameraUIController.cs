using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;
using CivModel;

public class CameraUIController : MonoBehaviour {

    public Canvas CameraCanvas;
    public Canvas tempManagementUI;
    public Button MoveButton;
    public Button AttackButton;
    public Button SkillButton;
    public Button WaitButton;


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

    }
    public void WaitButtonMethod()
    {

    }
    public void AttackButtonMethod()
    {

    }
    // Use this for initialization
	void Start () {
        gameManagerObject = CIVGameManager.GetGameManager();
        gameManager = gameManagerObject.GetComponent<CIVGameManager>();
        mPresenter = gameManager.GetPresenter();

        tempManagementUI.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
		if(!gameManager.pointSelected.HasValue)
        {
            MoveButton.enabled = false;
        }
        else
        {
            MoveButton.enabled = true;
        }
        
	}
}
