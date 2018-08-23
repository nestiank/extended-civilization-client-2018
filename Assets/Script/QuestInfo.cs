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
        return qst.Requester.PlayerName;
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
        else
        {
            questName = "hwan_main1";
            Debug.Log("Unknown Quest: " + qst.TextName);
        }

        return questName;
    }

	static public Sprite GetPortraitImage(Quest qst) {
        string questName = GetQuestName(qst);
		return Resources.Load<Sprite>("Quests/" + questName);
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
