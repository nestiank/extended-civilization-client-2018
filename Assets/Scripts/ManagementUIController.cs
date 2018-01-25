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
    public List<GameObject> SQlist;

    public GameObject proPrefab;
    public GameObject depPrefab;
    public GameObject productablePrefab;            // prefab templates

    public GameObject proQueue;
    public GameObject depQueue;
    public GameObject productableQueue;

    private IReadOnlyList<IProductionFactory> facList;

    


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
    public GameObject MakeProductItem(GameObject prefab, Unit unit)
    {
        GameObject item = Instantiate(prefab);
        item.GetComponents<Text>()[1].text = unit.GetType().ToString();
        return item;
    }
    public GameObject MakeSelectionItem(GameObject prefab, Unit unit)
    {
        GameObject item = Instantiate(prefab);
        item.GetComponents<Text>()[1].text = unit.GetType().ToString();
        return item;
    }
    public void InitiateSelectionTap(Player player)
    {
        mPresenter.CommandApply();
        var factoryList = mPresenter.AvailableProduction;

    }


    public void ManageButton()                                      // Management tab on/off button
    {
        if (mPresenter.State == Presenter.States.Normal)
        {
            mPresenter.CommandProductUI();
            if(mPresenter.State == Presenter.States.ProductUI)
            {
                List<GameObject> tempList = new List<GameObject>();
                Debug.Log("SelectList startMaking");
                mPresenter.CommandApply();
                foreach(GameObject sq in SQlist)
                {
                    Destroy(sq);
                }
                SQlist.Clear();
                facList = mPresenter.AvailableProduction;
                Debug.Log("facList : " + facList.Count);
                Debug.Log("SelectList Updated");
                foreach (IProductionFactory fac in facList)
                {
                    var SPrefab = Instantiate(productablePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    SPrefab.transform.SetParent(productableQueue.transform);
                    SPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                    SPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
                    tempList.Add(SPrefab.GetComponent<SelPrefab>().MakeItem(fac));
                }
                if(facList.Count == 0)
                {
                    Debug.Log("SelectList null");
                    var SPrefab = Instantiate(productablePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    SPrefab.transform.SetParent(productableQueue.transform);
                    SPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                    SPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
                    SPrefab.GetComponent<SelPrefab>().MakeItem();
                    tempList.Add(SPrefab);
                }
                SQlist = tempList;
                mPresenter.CommandCancel();
            }
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
        SQlist = new List<GameObject>();

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
                    break;
                }
            case CivPresenter.Presenter.States.ProductUI:
                {
                    SetManagementUI(true);
                    break;
                }
            case CivPresenter.Presenter.States.ProductAdd:
                {
                    SetManagementUI(true);
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