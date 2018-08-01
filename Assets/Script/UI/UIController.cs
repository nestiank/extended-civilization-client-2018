using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;

public class UIController : MonoBehaviour {

	private static UIController uicontroller;

	private IReadOnlyList<Quest> questList;
	private List<GameObject> DQQlist; //Offered Quest Queue list
	private List<GameObject> AQQlist; //Taken Quest Queue list
	private List<GameObject> CQQlist; //Completed Quset Queue list
	public GameObject DQPrefab;
	public GameObject AQPrefab;
	public GameObject CQPrefab;
	public GameObject DQQueue;
	public GameObject AQQueue;
	public GameObject CQQueue;
	public GameObject QstInfo;
	private Text[] questInfotexts;

	void Awake() {
		DontDestroyOnLoad(this);
		if (uicontroller == null) {
			uicontroller = this;
		}
		else {
			Destroy(this);
		}
	}

	// Use this for initialization
	void Start() {
		if (uicontroller == this) {
			DQQlist = new List<GameObject>();
			AQQlist = new List<GameObject>();
			CQQlist = new List<GameObject>();
			questInfotexts = QstInfo.GetComponentsInChildren<Text>();
		}
		else {
			Destroy(this);
		}
	}

	// Update is called once per frame
	void Update() {

	}

	// Initialize Quest Queue from PlayerInTurn.Quests
	public void MakeQuestQueue() {
		List<GameObject> tempDList = new List<GameObject>();
		List<GameObject> tempAList = new List<GameObject>();
		List<GameObject> tempCList = new List<GameObject>();

		// Debug.Log("QuestQueue making");
		foreach (GameObject item in DQQlist) {
			Destroy(item);
		}
		DQQlist.Clear();
		foreach (GameObject item in AQQlist) {
			Destroy(item);
		}
		AQQlist.Clear();
		foreach (GameObject item in CQQlist) {
			Destroy(item);
		}
		CQQlist.Clear();

		questList = GameManager.Instance.Game.PlayerInTurn.Quests;
		Debug.Log("Quest: " + questList.Count);
		foreach (Quest qst in questList) {
			switch (qst.Status) {
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
					Debug.Log("Undefined Status");
					throw new System.Exception("Undefined Status");
			}
		}

		if (tempDList.Count == 0) {
			var dqPrefab = Instantiate(DQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			dqPrefab.transform.SetParent(DQQueue.transform);
			dqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
			dqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
			dqPrefab.name = "DQuest_null";
			tempDList.Add(dqPrefab.GetComponent<Quests>().MakeDItem());
		}
		DQQlist = tempDList;
		if (tempAList.Count == 0) {
			var aqPrefab = Instantiate(AQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			aqPrefab.transform.SetParent(AQQueue.transform);
			aqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
			aqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
			aqPrefab.name = "AQuest_null";
			tempAList.Add(aqPrefab.GetComponent<Quests>().MakeAItem());
		}
		AQQlist = tempAList;
		if (tempCList.Count == 0) {
			var cqPrefab = Instantiate(CQPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			cqPrefab.transform.SetParent(CQQueue.transform);
			cqPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
			cqPrefab.transform.localPosition = new Vector3(0f, 0f, 0f);
			cqPrefab.name = "CQuest_null";
			tempCList.Add(cqPrefab.GetComponent<Quests>().MakeCItem());
		}
		CQQlist = tempCList;

		foreach (GameObject dq in DQQlist) {
			dq.GetComponent<Quests>().SetDButton();
		}
		foreach (GameObject aq in AQQlist) {
			aq.GetComponent<Quests>().SetAButton();
		}
		foreach (GameObject cq in CQQlist) {
			cq.GetComponent<Quests>().SetCButton();
		}
	}

	public void SetQuestInfo(Quest qst, int type) {
		if (qst == null) {
			foreach (Text txt in questInfotexts) {
				switch (txt.name) {
					default:
						txt.text = "";
						break;
				}
			}
			QstInfo.SetActive(false);
		}
		else {
			foreach (Text txt in questInfotexts) {
				switch (txt.name) {
					case "QuestNameText":
						txt.text = qst.Name;
						break;
					case "OfferedTurnText":
						txt.text = "게시된 턴: 턴 " + qst.PostingTurn; // qst에서 불러올 수 없음
						if (qst.PostingTurn == -1)
							txt.text = "게시된 턴: 턴 1";
						break;
					case "AvailableTurnText":
						if (type == 1) txt.text = "게시 기한: " + qst.LeftTurn + "턴 동안";
						else txt.text = "남은 기간: " + qst.LeftTurn + "턴 동안";
						if (qst.LimitTurn == -1)
							txt.text = "게시 기한 : 영구히";
						break;
					case "DeadlineText":
						txt.text = "제한 기한: " + qst.LimitTurn + "턴 이내";
						if (qst.LimitTurn == -1)
							txt.text = "제한 기한: 없음";
						break;
					case "CountryText":
						txt.text = "게시 국가: " + QuestInfo.GetRequesterCountry(qst);
						break;
					case "ConditionText":
						txt.text = qst.GoalNotice;
						break;
					case "RewardText":
						txt.text = qst.RewardNotice;
						break;
					default:
						txt.text = "";
						break;
				}
			}
			QstInfo.SetActive(true);
		}
	}

	public static UIController GetUIController() {
		if (uicontroller == null) {
			Debug.Log("UIController not made");
			throw new MissingComponentException();
		}
		return uicontroller;
	}
}