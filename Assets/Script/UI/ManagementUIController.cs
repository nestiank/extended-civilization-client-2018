using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using System.Linq;

public class ManagementUIController : MonoBehaviour
{

    private static ManagementUIController managementUIController;

    public Canvas managementUI;

    private LinkedList<Production> mProduction;
    private LinkedList<Production> mDeployment;
    private IReadOnlyList<IProductionFactory> facList;

    private GameObject gameManagerObject;
    private GameManager gameManager;
    private Game game;

    private List<GameObject> PQlist; //생산 큐
    private List<GameObject> DQlist; //배치 큐
    private List<GameObject> EpicQlist, HighQlist, IntermediateQlist, LowQlist;    // Unit production
    private List<GameObject> CityQlist, CityBuildingQlist, NormalBuildingQlist;    // Building production
    private List<List<GameObject>> ASQlist;

    public GameObject EpicQueue, HighQueue, IntermediateQueue, LowQueue;    // Unit production
    public GameObject CityQueue, CityBuildingQueue, NormalBuildingQueue;  // Building production

    public static ManagementUIController GetManagementUIController()
    {
        if (managementUIController == null)
        {
            throw new MissingComponentException();
        }
        return managementUIController;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}