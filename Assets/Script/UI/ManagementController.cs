using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using System.Linq;

public class ManagementController : MonoBehaviour {

    private static ManagementController managementcontroller;

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
    private List<List<GameObject>> ASQlist;

    public GameObject proPrefab;
    public GameObject depPrefab;
    public GameObject productablePrefab;

    public GameObject proQueue;
    public GameObject depQueue;
    public GameObject EpicQueue, HighQueue, IntermediateQueue, LowQueue;    // Unit production
    public GameObject CityQueue, CityBuildingQueue, NormalBuildingQueue;  // Building production

    //ManageMentUI 갱신 함수
    public void begin()
    {
        MakeSelectionQ();
        MakeProductionQ();
        MakeDeploymentQ();
        foreach (GameObject dq in DQlist)
        {
            dq.GetComponent<DeployPrefab>().SetButton(DQlist.IndexOf(dq));
        }

    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (managementcontroller == null)
        {
            managementcontroller = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start () {
        if (managementcontroller == this)
        {
            gameManager = GameManager.Instance;
            game = gameManager.Game;

            ASQlist = new List<List<GameObject>>();
            ASQlist.Add(EpicQlist = new List<GameObject>());
            ASQlist.Add(HighQlist = new List<GameObject>());
            ASQlist.Add(IntermediateQlist = new List<GameObject>());
            ASQlist.Add(LowQlist = new List<GameObject>());
            ASQlist.Add(CityQlist = new List<GameObject>());
            ASQlist.Add(CityBuildingQlist = new List<GameObject>());
            ASQlist.Add(NormalBuildingQlist = new List<GameObject>());

            PQlist = new List<GameObject>();
            DQlist = new List<GameObject>();
        }
        else
        {
            Destroy(this);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        mProduction = game.PlayerInTurn.Production;
        mDeployment = game.PlayerInTurn.Deployment;
    }

    //type: 0 = all, 1 = unit, 2 = city, 3 = NormalBuliding, 4 = citybuilding, 5 = BulidingAll (2~4 All)
    private void MakeSelectionQ()//선택 큐 프리팹 생성 함수
    {
        facList = GameManager.Instance.Game.PlayerInTurn.AvailableProduction.ToList(); //전체 선택 목록 받아오기
        //facList의 변경으로 Epic-High-intermediate-Low 변경 가능. 하지만 지금은 설정되지 않았음(Epic에 생성)

        //Debug.Log("facList: " + facList.Count);
        //Debug.Log("ALL SelectList Updated");

        DeleteAllSQ();
        game.PlayerInTurn.EstimateResourceInputs();
        foreach (IProductionFactory fac in facList)
        {
            //여기서 분리 
            if (typeof(CivModel.Unit).IsAssignableFrom(fac.ResultType))
            {
                var f = (IActorProductionFactory)fac;
                switch (f.ActorConstants.BattleClassLevel)
                {
                    case 4:
                        PartSelectionQ(EpicQlist, EpicQueue, fac);
                        break;
                    case 3:
                        PartSelectionQ(HighQlist, HighQueue, fac);
                        break;
                    case 2:
                        PartSelectionQ(IntermediateQlist, IntermediateQueue, fac);
                        break;
                    case 1:
                        PartSelectionQ(LowQlist, LowQueue, fac);
                        break;
                    case 0:
                        PartSelectionQ(LowQlist, LowQueue, fac);
                        break;
                    default:
                        PartSelectionQ(LowQlist, LowQueue, fac);
                        break;
                }
            }
            else if (typeof(TileObject).IsAssignableFrom(fac.ResultType))
            {
                //Debug.Log(fac.ToString());
                if (typeof(CityBase).IsAssignableFrom(fac.ResultType))
                {
                    PartSelectionQ(CityQlist, CityQueue, fac);
                }
                else if (typeof(TileBuilding).IsAssignableFrom(fac.ResultType))
                {
                    PartSelectionQ(NormalBuildingQlist, NormalBuildingQueue, fac);
                }
                else
                {
                    throw new System.Exception("Undefined Factory");
                }
            }
            else if (typeof(InteriorBuilding).IsAssignableFrom(fac.ResultType))
            {
                PartSelectionQ(CityBuildingQlist, CityBuildingQueue, fac);
            }
            else
            {
                throw new System.Exception("Undefined Factory");
            }
        }
        //내용물 없을 때 빈칸 채우기
        foreach (var qlist in ASQlist)
        {
            if (qlist.Count == 0)
            {
                GameObject productableQueue;
                switch (ASQlist.IndexOf(qlist))
                {
                    case 0:
                        productableQueue = EpicQueue;
                        break;
                    case 1:
                        productableQueue = HighQueue;
                        break;
                    case 2:
                        productableQueue = IntermediateQueue;
                        break;
                    case 3:
                        productableQueue = LowQueue;
                        break;
                    case 4:
                        productableQueue = CityQueue;
                        break;
                    case 5:
                        productableQueue = CityBuildingQueue;
                        break;
                    case 6:
                        productableQueue = NormalBuildingQueue;
                        break;
                    default:
                        productableQueue = null;
                        Debug.Log("Error: qlist = " + qlist);
                        throw new MissingComponentException();
                }
                //Debug.Log("SelectionList: " + ASQlist.IndexOf(qlist) + "null");
                var SPrefab = Instantiate(productablePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                SPrefab.transform.SetParent(productableQueue.transform);
                SPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                SPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
                SPrefab.GetComponent<ProductablePrefab>().MakeItem();
                qlist.Add(SPrefab);
            }
        }
    }

    //각 Factory의 분야를 읽어서 해당하는 Queue에 집어넣는 역할 
    private GameObject PartSelectionQ(List<GameObject> SQlist, GameObject productableQueue, IProductionFactory fac)
    {
        /*if (fac.ProductionResultType == null)
        {
            return null;
        }*/
        var SPrefab = Instantiate(productablePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        SPrefab.transform.SetParent(productableQueue.transform);
        SPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        SPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
        SPrefab.GetComponent<ProductablePrefab>().MakeItem(fac);
        SPrefab.GetComponent<ProductablePrefab>().SetButton(fac);
        SQlist.Add(SPrefab);
        return SPrefab;
    }

    //선택 큐 초기화(GameObject)
    private void DeleteAllSQ()
    {
        DeleteSQ(EpicQlist);
        DeleteSQ(HighQlist);
        DeleteSQ(IntermediateQlist);
        DeleteSQ(LowQlist);
        DeleteSQ(NormalBuildingQlist);
        DeleteSQ(CityQlist);
        DeleteSQ(CityBuildingQlist);
        ASQlist.Clear();
        ASQlist.Add(EpicQlist = new List<GameObject>());
        ASQlist.Add(HighQlist = new List<GameObject>());
        ASQlist.Add(IntermediateQlist = new List<GameObject>());
        ASQlist.Add(LowQlist = new List<GameObject>());
        ASQlist.Add(CityQlist = new List<GameObject>());
        ASQlist.Add(CityBuildingQlist = new List<GameObject>());
        ASQlist.Add(NormalBuildingQlist = new List<GameObject>());
    }

    //선택 큐 초기화에 쓰이는 함수
    private void DeleteSQ(List<GameObject> SQlist)
    {
        foreach (GameObject sq in SQlist)
        {
            Destroy(sq);
        }
        SQlist.Clear();
    }
    
    public void MakeProductionQ()
    {
        List<GameObject> tempList = new List<GameObject>();
        //Debug.Log("ProductionList startMaking");
        foreach (GameObject pq in PQlist)
        {
            Destroy(pq);
        }
        PQlist.Clear();
        mProduction = GameManager.Instance.Game.PlayerInTurn.Production;
        //Debug.Log("ProList : " + mProduction.Count);
        //Debug.Log("ProductionList Updated");
        foreach (Production prod in mProduction)
        {
            var PPrefab = Instantiate(proPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            PPrefab.transform.SetParent(proQueue.transform);
            PPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            PPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(PPrefab.GetComponent<ProductionPrefab>().MakeItem(prod));
        }
        if (mProduction.Count == 0)
        {
            //Debug.Log("ProductionList null");
            var PPrefab = Instantiate(proPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            PPrefab.transform.SetParent(proQueue.transform);
            PPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            PPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            PPrefab.GetComponent<ProductionPrefab>().MakeItem();
            tempList.Add(PPrefab);
        }
        PQlist = tempList;
        foreach (GameObject pq in PQlist)
        {
            pq.GetComponent<ProductionPrefab>().SetButton(PQlist.IndexOf(pq));
        }
    }

    public void MakeDeploymentQ()
    {
        List<GameObject> tempList = new List<GameObject>();
        //Debug.Log("DeploymentList startMaking");
        foreach (GameObject dq in DQlist)
        {
            Destroy(dq);
        }
        DQlist.Clear();
        mDeployment = GameManager.Instance.Game.PlayerInTurn.Deployment;
        //Debug.Log("DepList : " + mDeployment.Count);
        //Debug.Log("DeploymentList Updated");
        foreach (Production prod in mDeployment)
        {
            var DPrefab = Instantiate(depPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            DPrefab.transform.SetParent(depQueue.transform);
            DPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            DPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(DPrefab.GetComponent<DeployPrefab>().MakeItem(prod));
        }
        if (mDeployment.Count == 0)
        {
            //Debug.Log("DeploymentList null");
            var DPrefab = Instantiate(depPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            DPrefab.transform.SetParent(depQueue.transform);
            DPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            DPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(DPrefab.GetComponent<DeployPrefab>().MakeItem());
            tempList.Add(DPrefab);
        }
        DQlist = tempList;
        foreach (GameObject dq in DQlist)
        {
            dq.GetComponent<DeployPrefab>().SetButton(DQlist.IndexOf(dq));
        }
    }

    public static ManagementController GetManagementController()
    {
        if (managementcontroller == null)
        {
            Debug.Log("ManagementController not made");
            throw new MissingComponentException();
        }
        return managementcontroller;
    }
}
