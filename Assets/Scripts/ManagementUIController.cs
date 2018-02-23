using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class ManagementUIController : MonoBehaviour {

    private static ManagementUIController managementUIController;

    public Canvas managementUI;

    private LinkedList<Production> mProduction;
    private LinkedList<Production> mDeployment;
    private IReadOnlyList<IProductionFactory> facList;

    private GameObject gameManagerObject;
    private GameManager gameManager;
    private Game game;

    private List<GameObject> PQlist;
    private List<GameObject> DQlist;
    private List<GameObject> SQlist;

    public GameObject proPrefab;
    public GameObject depPrefab;
    public GameObject productablePrefab;            // prefab templates

    public GameObject proQueue;
    public GameObject depQueue;
    public GameObject productableQueue;

    public void ManageFunction()                                      // Management tab on/off button -> ManageMentUIActive
    {
        List<GameObject> tempList = new List<GameObject>();
        Debug.Log("SelectList startMaking");
        foreach (GameObject sq in SQlist)
        {
            Destroy(sq);
        }
        SQlist.Clear();
        facList = game.PlayerInTurn.GetAvailableProduction();
        Debug.Log(facList + " " + facList.Count);
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
        if (facList.Count == 0)
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
        MakeProductionQ();
        MakeDeploymentQ();
        foreach (GameObject sq in SQlist)
        {
            sq.GetComponent<SelPrefab>().SetButton(SQlist.IndexOf(sq));
            Debug.Log(SQlist.IndexOf(sq));
        }
        foreach (GameObject dq in DQlist)
        {
            dq.GetComponent<DepPrefab>().SetButton(DQlist.IndexOf(dq));
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
            gameManager = GameManager.I;
            game = gameManager.Game;
            SQlist = new List<GameObject>();
            PQlist = new List<GameObject>();
            DQlist = new List<GameObject>();
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        Debug.Log(game);
        mProduction = game.PlayerInTurn.Production;
        mDeployment = game.PlayerInTurn.Deployment;
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
        mProduction = game.PlayerInTurn.Production;
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
        mDeployment = game.PlayerInTurn.Deployment;
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