using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using System.Linq;

public class ManagementUIController : MonoBehaviour {

    private static ManagementUIController managementUIController;

    public Canvas managementUI;

    private LinkedList<Production> mProduction;
    private LinkedList<Production> mDeployment;
    //private IReadOnlyList<IProductionFactory> facList;
    private IReadOnlyList<IProductionFactory> facList;
    private IReadOnlyList<Quest> questList;

    private GameObject gameManagerObject;
    private GameManager gameManager;
    private Game game;

    private List<GameObject> PQlist;
    private List<GameObject> DQlist;
    private List<GameObject> EpicQlist, HighQlist, IntermediateQlist, LowQlist;    // Unit production
    private List<GameObject> CityQlist, CityBuildingQlist, NormalBuildingQlist;
    private List<List<GameObject>> ASQlist;
    private List<GameObject> DQQlist; //Deploied Quest Queue list
    private List<GameObject> AQQlist; //Accepted Quest Queue list
    private List<GameObject> CQQlist; //Completed Quset Queue list

    public GameObject DQPrefab;
    public GameObject AQPrefab;
    public GameObject CQPrefab;
    public GameObject proPrefab;
    public GameObject depPrefab;
    public GameObject productablePrefab;            // prefab templates

    public GameObject DQQueue;
    public GameObject AQQueue;
    public GameObject CQQueue;
    public GameObject proQueue;
    public GameObject depQueue;
    public GameObject EpicQueue, HighQueue, IntermediateQueue, LowQueue;    // Unit production
    public GameObject CityQueue, CityBuildingQueue, NormalBuildingQueue;  // Building production

    public GameObject QuestInfo;
    
    private Text[] questInfotexts;

    //ManageMentUI 갱신 함수
    public void ManageFunction()                                      // Management tab on/off button -> ManageMentUIActive
    {
        MakeSelectionQ();
        MakeProductionQ();
        MakeDeploymentQ();
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

            DQQlist = new List<GameObject>();
            AQQlist = new List<GameObject>();
            CQQlist = new List<GameObject>();
            questInfotexts = QuestInfo.GetComponentsInChildren<Text>();
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        //Debug.Log(game);
        mProduction = game.PlayerInTurn.Production;
        mDeployment = game.PlayerInTurn.Deployment;
    }

    private void MakeSelectionQ()//선택 큐 프리팹 생성 함수
    {
        //Debug.Log("ALL SelectList startMaking");
        facList = game.PlayerInTurn.AvailableProduction.ToList(); //전체 선택 목록 받아오기
        //facList의 변경으로 Epic-High-intermediate-Low 변경 가능. 하지만 지금은 설정되지 않았음(Epic에 생성)
        //Debug.Log(facList + " " + facList.Count);
        //Debug.Log("facList : " + facList.Count);
        //Debug.Log("ALL SelectList Updated");
        DeleteAllSQ();
        foreach (IProductionFactory fac in facList)
        {
            //여기서 분리 
            if(typeof(Unit).IsAssignableFrom(fac.ResultType))
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
            else if(typeof(TileObject).IsAssignableFrom(fac.ResultType))
            {
                Debug.Log(fac.ToString());
                if (typeof(CityBase).IsAssignableFrom(fac.ResultType))
                {
                    PartSelectionQ(CityQlist, CityQueue, fac);
                }
                else if(typeof(TileBuilding).IsAssignableFrom(fac.ResultType))
                {
                    PartSelectionQ(NormalBuildingQlist, NormalBuildingQueue, fac);
                }
                else
                {
                    throw new System.Exception("Undefined Factory");
                }
            }
            else if(typeof(InteriorBuilding).IsAssignableFrom(fac.ResultType))
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
                        //Debug.Log("Error : qlist = " + qlist);
                        throw new MissingComponentException();
                }
                //Debug.Log("SelectionList : " + ASQlist.IndexOf(qlist) + "null");
                var SPrefab = Instantiate(productablePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                SPrefab.transform.SetParent(productableQueue.transform);
                SPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                SPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
                SPrefab.GetComponent<SelPrefab>().MakeItem();
                qlist.Add(SPrefab);
            }
        }
        foreach (GameObject sq in EpicQlist)
        {
            sq.GetComponent<SelPrefab>().SetButton(EpicQlist.IndexOf(sq));
            //Debug.Log(EpicQlist.IndexOf(sq));
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
        SPrefab.GetComponent<SelPrefab>().MakeItem(fac);
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
        mProduction = game.PlayerInTurn.Production;
        //Debug.Log("ProList : " + mProduction.Count);
        //Debug.Log("ProductionList Updated");
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
            //Debug.Log("ProductionList null");
            var PPrefab = Instantiate(proPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            PPrefab.transform.SetParent(proQueue.transform);
            PPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            PPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            PPrefab.GetComponent<ProPrefab>().MakeItem();
            tempList.Add(PPrefab);
        }
        PQlist = tempList;
        foreach (GameObject pq in PQlist)
        {
            pq.GetComponent<ProPrefab>().SetButton(PQlist.IndexOf(pq));
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
        mDeployment = game.PlayerInTurn.Deployment;
        //Debug.Log("DepList : " + mDeployment.Count);
        //Debug.Log("DeploymentList Updated");
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
            //Debug.Log("DeploymentList null");
            var DPrefab = Instantiate(depPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            DPrefab.transform.SetParent(depQueue.transform);
            DPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            DPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(DPrefab.GetComponent<DepPrefab>().MakeItem());
            tempList.Add(DPrefab);
        }
        DQlist = tempList;
        foreach (GameObject dq in DQlist)
        {
            dq.GetComponent<DepPrefab>().SetButton(DQlist.IndexOf(dq));
        }
    }

    public void MakeQuestQueue()
    {
        List<GameObject> tempDList = new List<GameObject>();
        List<GameObject> tempAList = new List<GameObject>();
        List<GameObject> tempCList = new List<GameObject>();
        Debug.Log("QuestQueue making");
        foreach (GameObject item in DQQlist)
        {
            Destroy(item);
        }
        DQQlist.Clear();
        foreach (GameObject item in AQQlist)
        {
            Destroy(item);
        }
        AQQlist.Clear();
        foreach (GameObject item in CQQlist)
        {
            Destroy(item);
        }
        CQQlist.Clear();
        questList = game.PlayerInTurn.Quests;
        Debug.Log("Quest : " + questList.Count);
        foreach (Quest qst in questList)
        {
            switch (qst.Status)
            {
                case QuestStatus.Deployed:
                    var dqPrefab = Instantiate(DQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    dqPrefab.transform.SetParent(DQQueue.transform);
                    dqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                    dqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
                    dqPrefab.name = "DQuest";
                    tempDList.Add(dqPrefab.GetComponent<Quests>().MakeDItem(qst));
                    break;
                case QuestStatus.Accepted:
                    var aqPrefab = Instantiate(AQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    aqPrefab.transform.SetParent(AQQueue.transform);
                    aqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                    aqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
                    aqPrefab.name = "AQuest";
                    tempAList.Add(aqPrefab.GetComponent<Quests>().MakeAItem(qst));
                    break;
                case QuestStatus.Completed:
                    var cqPrefab = Instantiate(CQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    cqPrefab.transform.SetParent(CQQueue.transform);
                    cqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
                    cqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
                    cqPrefab.name = "CQuest";
                    tempCList.Add(cqPrefab.GetComponent<Quests>().MakeCItem(qst));
                    break;
                case QuestStatus.Disabled:
                    break;

                default:
                    Debug.Log("Undifined Status");
                    throw new System.Exception("Undifined Status");
            }
        }
        if (tempDList.Count == 0)
        {
            var dqPrefab = Instantiate(DQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            dqPrefab.transform.SetParent(DQQueue.transform);
            dqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            dqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            dqPrefab.name = "DQuest_null";
            tempDList.Add(dqPrefab.GetComponent<Quests>().MakeDItem());
        }
        DQQlist = tempDList;

        if (tempAList.Count == 0)
        {
            var aqPrefab = Instantiate(AQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            aqPrefab.transform.SetParent(AQQueue.transform);
            aqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            aqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            aqPrefab.name = "AQuest_null";
            tempAList.Add(aqPrefab.GetComponent<Quests>().MakeAItem());
        }
        AQQlist = tempAList;

        if (tempCList.Count == 0)
        {
            var cqPrefab = Instantiate(CQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            cqPrefab.transform.SetParent(CQQueue.transform);
            cqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            cqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
            cqPrefab.name = "CQuest_null";
            tempCList.Add(cqPrefab.GetComponent<Quests>().MakeCItem());
        }
        CQQlist = tempCList;

        foreach (GameObject dq in DQQlist)
        {
            dq.GetComponent<Quests>().SetDButton();
        }
        foreach (GameObject aq in AQQlist)
        {
            aq.GetComponent<Quests>().SetAButton();
        }
        foreach (GameObject cq in CQQlist)
        {
            cq.GetComponent<Quests>().SetCButton();
        }

    }
    public void SetQuestInfo(Quest qst)
    {
        if (qst == null)
        {
            foreach (Text txt in questInfotexts)
            {
                switch (txt.name)
                {
                    default: txt.text = "";
                        break;
                }
            }
            QuestInfo.SetActive(false);
        }
        else
        {

            foreach (Text txt in questInfotexts)
            {
                switch (txt.name)
                {
                    case "QuestName":
                        txt.text = qst.Name;
                        break;
                    case "Offered Turn":
                        txt.text = "게시된 턴 : 턴 " + qst.PostingTurn; // qst에서 불러올 수 없음
                        if (qst.PostingTurn == -1)
                            txt.text = "게시된 턴 : 턴 1";
                        break;
                    case "Available Turns":
                        txt.text = "게시 기한 : " + qst.LeftTurn + "턴 동안";
                        if (qst.LimitTurn == -1)
                            txt.text = "게시 기한 : 영구히";
                        break;
                    case "Deadline":
                        txt.text = "제한 기한 : " + qst.LimitTurn + "까지";
                        if (qst.LimitTurn == -1)
                            txt.text = "제한 기한 : 없음";
                        break;
                    case "Country Name":
                        //txt.text = "게시 국가 : " + qst.Requester.ToString();
                        break;
                    case "Conditions":
                        txt.text = qst.GoalNotice;
                        break;
                    case "Rewards":
                        txt.text = qst.RewardNotice;
                        break;
                    default:
                        txt.text = "";
                        break;
                }
            }
            QuestInfo.SetActive(true);
        }
    }
    public static ManagementUIController GetManagementUIController()
    {
        if(managementUIController == null)
        {
            //Debug.Log("managementUIController not made");
            throw new MissingComponentException();
        }
        return managementUIController;
    }
}