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

    private GameObject gameManagerObject;
    private CIVGameManager gameManager;
    private Presenter mPresenter;

    public Transform newProduction;
    public Transform newPlacement;
    public Button pioneer;

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
        mProduction = 
    }
}
