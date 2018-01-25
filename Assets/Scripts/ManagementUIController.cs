using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;
using CivModel;
using CivModel.Common;

public class ManagementUIController : MonoBehaviour {

    private static ManagementUIController managementUIController;

    public Canvas managementUI;
    public Button managementTab;

    private LinkedList<Production> mProduction;
    private LinkedList<Production> mDeployment;
    private IReadOnlyList<IProductionFactory> facList;

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


    


    public void SetManagementUI(bool val)
    {
        Debug.Log("manUI : " + val);
        managementUI.enabled = val;
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
                MakeProductionQ();
                MakeDeploymentQ();
                foreach(GameObject sq in SQlist)
                {
                    sq.GetComponent<SelPrefab>().SetButton(SQlist.IndexOf(sq));
                }
            }
        }

        else if (mPresenter.State == Presenter.States.ProductUI)
        {
            mPresenter.CommandCancel();
        }
    }
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (managementUIController == null)
        {
            managementUIController = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        if (managementUIController == this)
        {
            gameManagerObject = CIVGameManager.GetGameManager();
            gameManager = gameManagerObject.GetComponent<CIVGameManager>();
            mPresenter = gameManager.GetPresenter();
            mGame = mPresenter.Game;
            mPlayers = mGame.Players;
        }
        else
        {
            Destroy(this);
        }
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

    public void MakeProductionQ()
    {
        List<GameObject> tempList = new List<GameObject>();
        Debug.Log("ProductionList startMaking");
        foreach (GameObject pq in PQlist)
        {
            Destroy(pq);
        }
        PQlist.Clear();
        mProduction = mGame.PlayerInTurn.Production;
        Debug.Log("ProList : " + mProduction.Count);
        Debug.Log("ProductionList Updated");
        foreach (Production prod in mProduction)
        {
            var PPrefab = Instantiate(proPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            PPrefab.transform.SetParent(proQueue.transform);
            PPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            PPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(PPrefab.GetComponent<ProPrefab>().MakeItem(prod));
        }
        if (mProduction.Count == 0)
        {
            Debug.Log("ProductionList null");
            var PPrefab = Instantiate(proPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            PPrefab.transform.SetParent(proQueue.transform);
            PPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            PPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            PPrefab.GetComponent<ProPrefab>().MakeItem();
            tempList.Add(PPrefab);
        }
        PQlist = tempList;
        
    }

    public void MakeDeploymentQ()
    {
        List<GameObject> tempList = new List<GameObject>();
        Debug.Log("DeploymentList startMaking");
        foreach (GameObject dq in DQlist)
        {
            Destroy(dq);
        }
        DQlist.Clear();
        mDeployment = mGame.PlayerInTurn.Deployment;
        Debug.Log("DepList : " + mDeployment.Count);
        Debug.Log("DeploymentList Updated");
        foreach (Production prod in mDeployment)
        {
            var DPrefab = Instantiate(depPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            DPrefab.transform.SetParent(depQueue.transform);
            DPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            DPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(DPrefab.GetComponent<DepPrefab>().MakeItem(prod));
        }
        if (mDeployment.Count == 0)
        {
            Debug.Log("DeploymentList null");
            var DPrefab = Instantiate(depPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            DPrefab.transform.SetParent(depQueue.transform);
            DPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            DPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(DPrefab.GetComponent<DepPrefab>().MakeItem());
            tempList.Add(DPrefab);
        }
        DQlist = tempList;
    }
    public static void PrefabsSetting()
    {
        ProPrefab.SetPresenter();
        DepPrefab.SetPresenter();
        SelPrefab.SetPresenter();
    }
    public static ManagementUIController GetManagementUIController()
    {
        if(managementUIController == null)
        {
            Debug.Log("managementUIController not made");
            throw new MissingComponentException();
        }
        return managementUIController;
    }
}