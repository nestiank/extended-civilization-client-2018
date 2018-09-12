using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;

public class Quests : MonoBehaviour {

	public Quest quest;

	private Text[] textarguments;
	private Image[] images;
	private Button[] buttons;

	// Use this for initialization
	void Start() {
		textarguments = gameObject.GetComponentsInChildren<Text>();
		images = gameObject.GetComponentsInChildren<Image>();
		buttons = gameObject.GetComponentsInChildren<Button>();
	}

	// Update is called once per frame
	void Update() {

	}

	public void AcceptItem(Quest qst) {
		qst.Accept();
		UIController.GetUIController().MakeQuestQueue();
	}
	public void CompleteItem(Quest quest) {
		quest.Complete();
		UIController.GetUIController().MakeQuestQueue();
	}

	public GameObject MakeDItem(Quest quest) {
		textarguments = gameObject.GetComponentsInChildren<Text>();
		images = gameObject.GetComponentsInChildren<Image>();
		buttons = gameObject.GetComponentsInChildren<Button>();
		this.quest = quest;
		string leftturn;
		if (quest.LeftTurn == -1) {
			leftturn = "영구적";
		}
		else {
			leftturn = quest.LeftTurn.ToString() + "턴 남음";
		}

		// Image 추가 부분
		foreach (Image img in images) {
			switch (img.name) {
				case "Portrait":
					img.sprite = QuestInfo.GetRequesterPortraitImage(quest);
					img.enabled = true;
					break;
				case "ResourceImage":
                    img.sprite = ResourceInfo.GetResourceSprite(quest);
					img.enabled = true;
					break;
			}
		}
		// Text 수정 부분
		foreach (Text txt in textarguments) {
			switch (txt.name) {
				case "Country":
					txt.text = QuestInfo.GetRequesterCountry(quest);
					break;
				case "NumberOfUnits":
					txt.text = "X 1";
					break;
				case "TurnsLeft":
					txt.text = leftturn;
					break;
			}
		}
		return this.gameObject;
	}
	public GameObject MakeDItem() {
		textarguments = gameObject.GetComponentsInChildren<Text>();
		images = gameObject.GetComponentsInChildren<Image>();
		buttons = gameObject.GetComponentsInChildren<Button>();
		quest = null;
		foreach (Image img in images) {
			switch (img.name) {
				case "Portrait":
					img.enabled = false;
					break;
				case "ResourceImage":
					img.enabled = false;
					break;
			}
		}

		foreach (Text txt in textarguments) {
			switch (txt.name) {
				case "Country":
					txt.text = "비었음";
					break;
				case "NumberOfUnits":
					txt.text = "";
					break;
				case "TurnsLeft":
					txt.text = "";
					break;
			}
		}

		foreach (Button but in buttons) {
			but.enabled = false;
		}
		return this.gameObject;
	}
	public void SetDButton() {
		if (quest == null) {
			foreach (Button but in buttons) {
				switch (but.name) {
					case "DQuest_null":
						break;
					default:
						but.gameObject.SetActive(false);
						break;
				}
			}
		}
		else {
			foreach (Button but in buttons) {
				switch (but.name) {
					case "Take":
						but.onClick.AddListener(delegate () { AcceptItem(quest); });
						break;
					case "DQuest":
						but.onClick.AddListener(delegate () { ShowInfoofQuest(quest, 1); UIController.GetUIController().SetQstExplain(quest); });
						break;
					default:
						Debug.Log("Undefined Button: " + but.name);
						break;
				}
			}
		}
	}

	public GameObject MakeAItem(Quest quest) {
		textarguments = gameObject.GetComponentsInChildren<Text>();
		images = gameObject.GetComponentsInChildren<Image>();
		buttons = gameObject.GetComponentsInChildren<Button>();
		this.quest = quest;
		string leftturn;
		if (quest.LeftTurn == -1) {
			leftturn = "영구적";
		}
		else {
			leftturn = quest.LeftTurn.ToString() + "턴 남음";
		}
		// Image 추가 부분
		foreach (Image img in images) {
			switch (img.name) {
				case "Portrait":
					img.sprite = QuestInfo.GetRequesterPortraitImage(quest);
					img.enabled = true;
					break;
				case "ResourceImage":
                    img.sprite = ResourceInfo.GetResourceSprite(quest);
					img.enabled = true;
					break;
			}
		}
		foreach (Text txt in textarguments) {
			switch (txt.name) {
				case "Country":
					txt.text = QuestInfo.GetRequesterCountry(quest);
					break;
				case "NumberOfUnits":
					txt.text = "X 1";
					break;
				case "TurnsLeft":
					txt.text = leftturn;
					break;
			}
		}
		return this.gameObject;
	}
	public GameObject MakeAItem() {
		textarguments = gameObject.GetComponentsInChildren<Text>();
		images = gameObject.GetComponentsInChildren<Image>();
		buttons = gameObject.GetComponentsInChildren<Button>();
		quest = null;
		foreach (Image img in images) {
			switch (img.name) {
				case "Portrait":
					img.enabled = false;
					break;
				case "ResourceImage":
					img.enabled = false;
					break;
			}
		}

		foreach (Text txt in textarguments) {
			switch (txt.name) {
				case "Country":
					txt.text = "비었음";
					break;
				case "NumberOfUnits":
					txt.text = "";
					break;
				case "TurnsLeft":
					txt.text = "";
					break;
			}
		}

		foreach (Button but in buttons) {
			but.enabled = false;
		}
		return this.gameObject;
	}
	public void SetAButton() {
		if (quest == null) {
			foreach (Button but in buttons) {
				switch (but.name) {
					case "AQuest_null":
						break;
					default:
						but.gameObject.SetActive(false);
						break;
				}
			}
		}
		else {
			foreach (Button but in buttons) {
				switch (but.name) {
					case "AQuest":
						but.onClick.AddListener(delegate () { ShowInfoofQuest(quest, 2); UIController.GetUIController().SetQstExplain(quest); });
						break;
					default:
						Debug.Log("Undefined Button: " + but.name);
						break;
				}
			}
		}
	}

	public GameObject MakeCItem(Quest quest) {
		textarguments = gameObject.GetComponentsInChildren<Text>();
		images = gameObject.GetComponentsInChildren<Image>();
		buttons = gameObject.GetComponentsInChildren<Button>();
		this.quest = quest;
		// Image 추가 부분
		foreach (Image img in images) {
			switch (img.name) {
				case "Portrait":
					img.sprite = QuestInfo.GetRequesterPortraitImage(quest);
					img.enabled = true;
					break;
				case "ResourceImage":
                    img.sprite = ResourceInfo.GetResourceSprite(quest);
					img.enabled = true;
					break;
			}
		}
		foreach (Text txt in textarguments) {
			switch (txt.name) {
				case "Country":
					txt.text = QuestInfo.GetRequesterCountry(quest);
					break;
				case "NumberOfUnits":
					txt.text = "X 1";
					break;
				case "TurnsLeft":
					txt.text = "종료됨";
					//몇 턴에 끝났는지 읽어올 방법 없음.
					break;
			}
		}
		return this.gameObject;
	}
	public GameObject MakeCItem() {
		textarguments = gameObject.GetComponentsInChildren<Text>();
		images = gameObject.GetComponentsInChildren<Image>();
		buttons = gameObject.GetComponentsInChildren<Button>();
		quest = null;
		foreach (Image img in images) {
			switch (img.name) {
				case "Portrait":
					img.enabled = false;
					break;
				case "ResourceImage":
					img.enabled = false;
					break;
			}
		}

		foreach (Text txt in textarguments) {
			switch (txt.name) {
				case "Country":
					txt.text = "비었음";
					break;
				case "NumberOfUnits":
					txt.text = "";
					break;
				case "TurnsLeft":
					txt.text = "";
					break;
			}
		}

		foreach (Button but in buttons) {
			but.enabled = false;
		}
		return this.gameObject;
	}
	public void SetCButton() {
		if (quest == null) {
			foreach (Button but in buttons) {
				switch (but.name) {
					case "CQuest_null":
						break;
					default:
						but.gameObject.SetActive(false);
						break;
				}
			}
		}
		else {
			foreach (Button but in buttons) {
				switch (but.name) {
					case "CQuest":
						but.onClick.AddListener(delegate () { ShowInfoofQuest(quest, 3); UIController.GetUIController().SetQstExplain(quest); });
						break;
					default:
						Debug.Log("Undefined Button: " + but.name);
						break;
				}
			}
		}
	}

	public void ShowInfoofQuest(Quest quest, int type) {
		UIController.GetUIController().SetQuestInfo(quest, type);
		return;
	}
}