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
    private List<GameObject> EpicQlist, HighQlist, IntermediateQlist, LowQlist;    // Unit production
    private List<GameObject> CityQlist, CityBuildingQlist, NormalBuildingQlist;

    public GameObject proPrefab;
    public GameObject depPrefab;
    public GameObject productablePrefab;            // prefab templates

    public GameObject proQueue;
    public GameObject depQueue;
    public GameObject EpicQueue, HighQueue, IntermediateQueue, LowQueue;    // Unit production
    public GameObject CityQueue, CityBuildingQueue, NormalBuildingQueue;  // Building production


    private List<GameObject> MakeSelectionQ(List<GameObject> SQlist, GameObject productableQueue)
    {
        List<GameObject> tempList = new List<GameObject>();
        Debug.Log("SelectList startMaking");
        foreach (GameObject sq in SQlist)
        {
            Destroy(sq);
        }
        SQlist.Clear();
        facList = game.PlayerInTurn.GetAvailableProduction();
        //facList의 변경으로 Epic-High-intermediate-Low 변경 가능. 하지만 지금은 설정되지 않았음(Epic에 생성)
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
        return tempList;
    }
    public void ManageFunction()                                      // Management tab on/off button -> ManageMentUIActive
    {
        EpicQlist = MakeSelectionQ(EpicQlist, EpicQueue);
        MakeProductionQ();
        MakeDeploymentQ();
        foreach (GameObject sq in EpicQlist)
        {
            sq.GetComponent<SelPrefab>().SetButton(EpicQlist.IndexOf(sq));
            Debug.Log(EpicQlist.IndexOf(sq));
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

            EpicQlist = new List<GameObject>();
            HighQlist = new List<GameObject>();
            IntermediateQlist = new List<GameObject>();
            LowQlist = new List<GameObject>();
            CityQlist = new List<GameObject>();
            CityBuildingQlist = new List<GameObject>();
            NormalBuildingQlist = new List<GameObject>();

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