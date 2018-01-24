using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;
using CivModel;
using CivModel.Common;

public class ManagementUIController : MonoBehaviour {

    public Canvas managementUI;
    public Button managementTab;

    public LinkedList<Production> mProduction;
    public LinkedList<Production> mDeployment;

    private Player mPlayer;

    private GameObject gameManagerObject;
    private CIVGameManager gameManager;
    private Presenter mPresenter;

    public GameObject[] PQlist;
    public GameObject[] DQlist;

    public GameObject proPrefab;
    public GameObject depPrefab;        // prefab templates
    public Button pioneer;              // new unit production when clicked

    public void setControlUI ()
    {
        if (managementTab == true)
            managementUI.enabled = true;//            managementUI.gameObject.SetActive(true);
        else
            managementUI.enabled = false;//            managementUI.gameObject.SetActive(false);
    }


    void Start()
    {
        gameManagerObject = CIVGameManager.GetGameManager();
        gameManager = gameManagerObject.GetComponent<CIVGameManager>();
        mPresenter = gameManager.GetPresenter();
    }

    void Update()
    {
        if (managementTab == true)
            managementUI.enabled = true;
        else
            managementUI.enabled = false;

        mProduction = mPlayer.Production;       // The list of the not-finished productions of this player
        mDeployment = mPlayer.Deployment;       // The list of the ready-to-deploy productions of this player
    }
    

    public void productionQ()
    {
        for (int i = 0; i < mProduction.Count; i++)
        {
            PQlist[i] = Instantiate(proPrefab) as GameObject;
            PQlist[i].GetComponent<Text>().text = 3 + "턴 이후 종료";        // need to calculate how many turns are left
        }
        
        
    }

    public void deploymentQ()
    {
        for (int i = 0; i < mDeployment.Count; i++)
        {
            DQlist[i] = Instantiate(depPrefab) as GameObject;
        }

    }
}