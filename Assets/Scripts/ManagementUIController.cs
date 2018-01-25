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

    private LinkedList<Production> mProduction;
    private LinkedList<Production> mDeployment;

    private IReadOnlyList<Player> mPlayers;

    private GameObject gameManagerObject;
    private CIVGameManager gameManager;
    private Presenter mPresenter;
    private Game mGame;

    public List<GameObject> PQlist;
    public List<GameObject> DQlist;

    public GameObject proPrefab;
    public GameObject depPrefab;            // prefab templates
    public Button newPioneer;              // new unit production when clicked

    public void SetManagementUI(bool val)
    {
        Debug.Log("manUI : " + val);
        managementUI.enabled = val;
    }

    public GameObject MakeDeploymentItem(GameObject prefab, Unit unit)
    {
        GameObject item = Instantiate(prefab);
        item.GetComponents<Text>()[1].text = unit.GetType().ToString();
        return item;
    }


                
    public void ManageButton()                                      // Management tab on/off button
    {
        if (mPresenter.State == Presenter.States.Normal)
        {
            mPresenter.CommandProductUI();
        }
        else if (mPresenter.State == Presenter.States.ProductUI)
        {
            mPresenter.CommandCancel();
        }
    }

    void Start()
    {
        gameManagerObject = CIVGameManager.GetGameManager();
        gameManager = gameManagerObject.GetComponent<CIVGameManager>();
        mPresenter = gameManager.GetPresenter();

        mPlayers = mPresenter.Game.Players;
    }

    void Update()
    {
        mProduction = mPresenter.Game.PlayerInTurn.Production;
        mDeployment = mPresenter.Game.PlayerInTurn.Deployment;

        switch (mPresenter.State)//for debug
        {
            case CivPresenter.Presenter.States.Deploy:
                {
                    SetManagementUI(true);
                    Debug.Log("State : Deploy");
                    break;
                }
            case CivPresenter.Presenter.States.ProductUI:
                {
                    SetManagementUI(true);
                    Debug.Log("State : ProductUI");
                    break;
                }
            case CivPresenter.Presenter.States.ProductAdd:
                {
                    SetManagementUI(true);
                    Debug.Log("State : ProductAdd");
                    break;
                }
            default:
                SetManagementUI(false);
                break;
        }
    }
    

    public void AddNewProduction(bool val)
    {
        Debug.Log("new production in queue");

    }

    public void MakeProductionQ()
    {
        for (int i = 0; i < mProduction.Count; i++)
        {
            PQlist[i] = Instantiate(proPrefab);
            PQlist[i].GetComponent<Text>().text = 3 + "턴 이후 종료";        // need to calculate how many turns are left
        }
        
        
    }

    public void MakeDeploymentQ()
    {
        for (int i = 0; i < mDeployment.Count; i++)
        {
            DQlist[i] = Instantiate(depPrefab);
        }

    }
}