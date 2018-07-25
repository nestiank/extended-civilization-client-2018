using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;

public class QuestInfo : MonoBehaviour {

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	static public string GetRequesterCountry(Quest qst) {
		switch (qst.Name) {
			case "불가사의 - 오티즘 빔 반사 어레이":
				return "어류 공화국";
			case "첩보 - 크툴루 계획":
				return "아틀란티스";
			case "불가사의- 이집트 캉덤":
				return "이집트";
			case "[전쟁 동맹] - 에뮤 연방":
				return "에뮤 연방";
			case "[불가사의] - 아틀란티스":
				return "아틀란티스";
			case "[불가사의] - R'̧l̨̜y͎͎̜̺̬e͕͇͇͚͓̹h̢̳͎̗͇͇̙":
				return "R'̧l̨̜y͎͎̜̺̬e͕͇͇͚͓̹h̢̳͎̗͇͇̙";
			case "군사 동맹 - 궤도 장악권":
				return "쉬드 캉덤";
			case "건물 기증 - 모아이 포스 필드":
				return "이스터 왕국";
			case "불가사의 - 성간 에너지":
				return "에뮤 왕국";
			case "불가사의 - 유전 연구학":
				return "아틀란티스";
			default:
				return "Requester Unknown";
		}
	}

	static public Sprite GetPortraitImage(Quest qst) {
		string questPortraitName = "";
		switch (qst.Name) {
			case "불가사의 - 오티즘 빔 반사 어레이":
				questPortraitName = "hwan_main1";
				break;
			case "첩보 - 크툴루 계획":
				questPortraitName = "hwan_main2";
				break;
			case "불가사의- 이집트 캉덤":
				questPortraitName = "hwan_main3";
				break;
			case "[전쟁 동맹] - 에뮤 연방":
				questPortraitName = "finno_main1";
				break;
			case "[불가사의] - 아틀란티스":
				questPortraitName = "finno_main2";
				break;
			case "[불가사의] - R'̧l̨̜y͎͎̜̺̬e͕͇͇͚͓̹h̢̳͎̗͇͇̙":
				questPortraitName = "finno_main3";
				break;
			case "군사 동맹 - 궤도 장악권":
				questPortraitName = "hwan_sub1";
				break;
			case "건물 기증 - 모아이 포스 필드":
				questPortraitName = "hwan_sub2";
				break;
			case "불가사의 - 성간 에너지":
				questPortraitName = "finno_sub1";
				break;
			case "불가사의 - 유전 연구학":
				questPortraitName = "finno_sub2";
				break;
			default:
				questPortraitName = "hwan_sub1";
				break;
		}
		return Resources.Load<Sprite>("Quests/" + questPortraitName);
	}

	static public string GetResourceImage(Quest qst) {
		return "";
	}

}
