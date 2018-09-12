using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using CivModel.Quests;

public class QuestInfo : MonoBehaviour {

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	static public string GetRequesterCountry(Quest qst) {
        return qst.Requester?.PlayerName ?? "(없음)";
	}

    static public string GetQuestName(Quest qst)
    {
        string questName = "";

        if (qst is QuestAtlantis)
        {
            questName = "finno_main2";
        }
        else if (qst is QuestAutismBeamReflex)
        {
            questName = "hwan_main1";
        }
        else if (qst is QuestEgyptKingdom)
        {
            questName = "hwan_main3";
        }
        else if (qst is QuestPorjectCthulhu)
        {
            questName = "hwan_main2";
        }
        else if (qst is QuestRlyeh)
        {
            questName = "finno_main3";
        }
        else if (qst is QuestSubAirspaceDomination)
        {
            questName = "hwan_sub1";
        }
        else if (qst is QuestSubGeneticEngineering)
        {
            questName = "finno_sub2";
        }
        else if (qst is QuestSubInterstellarEnergy)
        {
            questName = "finno_sub1";
        }
        else if (qst is QuestSubMoaiForceField)
        {
            questName = "hwan_sub2";
        }
        else if (qst is QuestWarAliance)
        {
            questName = "finno_main1";
        }
        else if(qst is QuestHwanTuto1)
        {
            questName = "hwan_tuto1";
        }
        else if(qst is QuestHwanTuto2)
        {
            questName = "hwan_tuto2";
        }
        else if(qst is QuestHwanTuto3)
        {
            questName = "hwan_tuto3";
        }
        else if(qst is QuestHwanTuto4)
        {
            questName = "hwan_tuto4";
        }
        else if(qst is QuestHwanTuto5)
        {
            questName = "hwan_tuto5";
        }
        else if(qst is QuestFinnoTuto1)
        {
            questName = "finno_tuto1";
        }
        else if (qst is QuestFinnoTuto2)
        {
            questName = "finno_tuto2";
        }
        else if (qst is QuestFinnoTuto3)
        {
            questName = "finno_tuto3";
        }
        else if (qst is QuestFinnoTuto4)
        {
            questName = "finno_tuto4";
        }
        else if (qst is QuestFinnoTuto5)
        {
            questName = "finno_tuto5";
        }
        else
        {
            questName = "invisible";
            Debug.Log("Unknown Quest: " + qst.TextName);
        }

        return questName;
    }

	static public Sprite GetPortraitImage(Quest qst) {
        string questName = GetQuestName(qst);
		return Resources.Load<Sprite>("Quests/" + questName);
	}

    static public Sprite GetRequesterPortraitImage(Quest qst) {
        if (qst.Requester == null)
            return Resources.Load<Sprite>("Quests/lemuria");

        int requesterNumber = qst.Requester.PlayerNumber;
        string requesterName = "";

        switch (requesterNumber)
        {
            case 0:
                requesterName = "hwan"; break;
            case 1:
                requesterName = "finno"; break;
            case 2:
                requesterName = "egypt"; break;
            case 3:
                requesterName = "atlantis"; break;
            case 4:
                requesterName = "fish"; break;
            case 5:
                requesterName = "emu"; break;
            case 6:
                requesterName = "shied"; break;
            case 7:
                requesterName = "lemuria"; break;
            case 8:
                requesterName = "easter"; break;
            default:
                requesterName = GetQuestName(qst);
                Debug.Log("Unknown Requester");
                break;
        }
        return Resources.Load<Sprite>("Quests/" + requesterName);
    }
    

    static public Sprite GetExplainImage(Quest qst)
    {
        string questName = GetQuestName(qst);
        return Resources.Load<Sprite>("Quests/Description/" + questName);
    }

	static public string GetResourceImage(Quest qst) {
		return "";
	}

    

}
