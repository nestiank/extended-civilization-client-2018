using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using System.Linq;

public class SpecialResourceView : MonoBehaviour
{
    private static SpecialResourceView specialResourceView;

    private GameManager gameManager;
    private Game game;

    public GameObject SRPrefab;
    private List<GameObject> SRQlist;
    public GameObject SRQueue;

    public IDictionary<ISpecialResource, int> mSRlist;

    public void begin()
    {
        MakeSpecialResourseQ();
    }

    void Awake()
    {
        // DontDestroyOnLoad(this);
        if (specialResourceView == null)
        {
            specialResourceView = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start ()
    {
        gameManager = GameManager.Instance;
        game = gameManager.Game;
        SRQlist = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        mSRlist = game.PlayerInTurn.SpecialResource;
	}

    public void MakeSpecialResourseQ()
    {
        List<GameObject> tempList = new List<GameObject>();
        //Debug.Log("ProductionList startMaking");
        foreach (GameObject SRq in SRQlist)
        {
            Destroy(SRq);
        }
        SRQlist.Clear();
        mSRlist = game.PlayerInTurn.SpecialResource;

        if(mSRlist[CivModel.Quests.AutismBeamAmplificationCrystal.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.AutismBeamAmplificationCrystal.Instance));
        }

        if (mSRlist[CivModel.Quests.GatesOfRlyeh.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.GatesOfRlyeh.Instance));
        }

        if (mSRlist[CivModel.Quests.InterstellarEnergyExtractor.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.InterstellarEnergyExtractor.Instance));
        }

        if (mSRlist[CivModel.Quests.Necronomicon.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.Necronomicon.Instance));
        }

        if (mSRlist[CivModel.Quests.SpecialResourceAirspaceDomination.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.SpecialResourceAirspaceDomination.Instance));
        }

        if (mSRlist[CivModel.Quests.SpecialResourceAlienCommunication.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.SpecialResourceAlienCommunication.Instance));
        }

        if (mSRlist[CivModel.Quests.SpecialResourceAutismBeamReflex.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.SpecialResourceAutismBeamReflex.Instance));
        }

        if (mSRlist[CivModel.Quests.SpecialResourceCthulhuProjectInfo.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.SpecialResourceCthulhuProjectInfo.Instance));
        }

        if (mSRlist[CivModel.Quests.SpecialResourceMoaiForceField.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.SpecialResourceMoaiForceField.Instance));
        }

        if (mSRlist[CivModel.Quests.Ubermensch.Instance] != 0)
        {
            var SRPre = Instantiate(SRPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            SRPre.transform.SetParent(SRQueue.transform);
            SRPre.transform.localScale = new Vector3(1f, 1f, 1f);
            SRPre.transform.localPosition = new Vector3(0f, 0f, 0f);
            tempList.Add(SRPre.GetComponent<SpecialResourcePrefab>().MakeItem(CivModel.Quests.Ubermensch.Instance));
        }

        SRQlist = tempList;
    }

    public static SpecialResourceView GetSpecialResourceView()
    {
        if (specialResourceView == null)
        {
            Debug.Log("SpecialResourceView not made");
            throw new MissingComponentException();
        }
        return specialResourceView;
    }
}
