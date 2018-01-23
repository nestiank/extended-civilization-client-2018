using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;
using CivModel;
using CivModel.Common;

public class ManagementUIController : MonoBehaviour {

    public GameObject managementUI;
    public Button managementTab;

    public LinkedList<Production> mProduction;
    public LinkedList<Production> mDeployment;

    private Player mPlayer;

    private GameObject gameManagerObject;
    private CIVGameManager gameManager;
    private Presenter mPresenter;

    public Transform newProduction;
    public Transform newPlacement;      // prefabs
    public Button pioneer;              // new unit production when clicked

    public void setControlUI ()
    {
        if (managementTab == true)
            managementUI.gameObject.SetActive(true);
        else
            managementUI.gameObject.SetActive(false);
    }

    public void productionQ (CivModel.Production produce)
    {
        if (pioneer == true)
        {
            Instantiate(newProduction);
        }

        if (produce.Completed == true)
            Destroy(newProduction);
    }

    public void placementQ (CivModel.Production produce)
    {
        if (produce.Completed == true)
            Instantiate(newPlacement);

    }

    void Start()
    {
        gameManagerObject = CIVGameManager.GetGameManager();
        gameManager = gameManagerObject.GetComponent<CIVGameManager>();
        mPresenter = gameManager.GetPresenter();
    }

        void Update()
    {
        mProduction = mPlayer.Production;       // The list of the not-finished productions of this player
        mDeployment = mPlayer.Deployment;       // The list of the ready-to-deploy productions of this player
    }
}