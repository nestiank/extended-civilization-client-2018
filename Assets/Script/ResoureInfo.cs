using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using CivModel.Quests;

public class ResourceInfo : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    static public string GetResourceName(Quest qst) {
        if (qst is QuestAtlantis)
        {
            return "[특수 자원: 네크로노미콘]";
        }
        else if (qst is QuestAutismBeamReflex)
        {
            return "[특수 자원 : 오티즘빔 반사 어레이]";
        }
        else if (qst is QuestEgyptKingdom)
        {
            return "[특수 자원 : 가자 피라미드 외계 통신 기구]";
        }
        else if (qst is QuestPorjectCthulhu)
        {
            return "[특수 자원 : 크툴루 계획 기밀 정보]";
        }
        else if (qst is QuestRlyeh)
        {
            return "[특수 자원: G̼͈͉̖a̙͉͔͍̙t͍̞͕e̺̹̼̬s̷̘̯ ͉̪͙̯ͅo̮̝͔̩̖f͚͚͖̳̻͇ ̮̻̮͎͇͇R̝'̥̬͝l͚̼y҉̫e͏̜̲͔͈̲͖ͅh́]";
        }
        else if (qst is QuestSubAirspaceDomination)
        {
            return "[특수 자원 : 궤도 장악권]";
        }
        else if (qst is QuestSubGeneticEngineering)
        {
            return "[특수 자원 : Ubermensch]";
        }
        else if (qst is QuestSubInterstellarEnergy)
        {
            return "[특수 자원 : 성간 에너지 추출기]";
        }
        else if (qst is QuestSubMoaiForceField)
        {
            return "[특수 자원 : 모아이 포스 필드]";
        }
        else if (qst is QuestWarAliance)
        {
            return "[특수 자원: 오티즘 빔 증폭 크리스탈]";
        }
        else
        {
            return "Resoure Unknown";
        }
    }

    static public Sprite GetResourceSprite(Quest qst) {
        string resourceSpriteName = "";

        if (qst is QuestAtlantis)
        {
            resourceSpriteName = "finno_necronimicon";
        }
        else if (qst is QuestAutismBeamReflex)
        {
            resourceSpriteName = "hwan_autism_ray_reflection";
        }
        else if (qst is QuestEgyptKingdom)
        {
            resourceSpriteName = "hwan_pyramid";
        }
        else if (qst is QuestPorjectCthulhu)
        {
            resourceSpriteName = "hwan_cthulhu_info";
        }
        else if (qst is QuestRlyeh)
        {
            resourceSpriteName = "finno_gate";
        }
        else if (qst is QuestSubAirspaceDomination)
        {
            resourceSpriteName = "hwan_spacetrack";
        }
        else if (qst is QuestSubGeneticEngineering)
        {
            resourceSpriteName = "finno_ubermensch";
        }
        else if (qst is QuestSubInterstellarEnergy)
        {
            resourceSpriteName = "finno_energy";
        }
        else if (qst is QuestSubMoaiForceField)
        {
            resourceSpriteName = "hwan_moai";
        }
        else if (qst is QuestWarAliance)
        {
            resourceSpriteName = "finno_crystal";
        }
        else
        {
            resourceSpriteName = "finno_crystal";
            Debug.Log("Unknown Quest: " + qst.TextName);
        }
        return Resources.Load<Sprite>("SpecialResource/" + resourceSpriteName);
    }

}
