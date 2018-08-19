using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;

public class ResourceInfo : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    static public string GetResourceName(Quest qst) {
        switch (qst.TextName) {
            case "불가사의 - 오티즘 빔 반사 어레이":
                return "[특수 자원 : 오티즘빔 반사 어레이]";
            case "첩보 - 크툴루 계획":
                return "[특수 자원 : 크툴루 계획 기밀 정보]";
            case "불가사의- 이집트 캉덤":
                return "[특수 자원 : 가자 피라미드 외계 통신 기구]";
            case "[전쟁 동맹] - 에뮤 연방":
                return "[특수 자원: 오티즘 빔 증폭 크리스탈]";
            case "[불가사의] - 아틀란티스":
                return "[특수 자원: 네크로노미콘]";
            case "[불가사의] - R'̧l̨̜y͎͎̜̺̬e͕͇͇͚͓̹h̢̳͎̗͇͇̙":
                return "[특수 자원: G̼͈͉̖a̙͉͔͍̙t͍̞͕e̺̹̼̬s̷̘̯ ͉̪͙̯ͅo̮̝͔̩̖f͚͚͖̳̻͇ ̮̻̮͎͇͇R̝'̥̬͝l͚̼y҉̫e͏̜̲͔͈̲͖ͅh́]";
            case "군사 동맹 - 궤도 장악권":
                return "[특수 자원 : 궤도 장악권]";
            case "건물 기증 - 모아이 포스 필드":
                return "[특수 자원 : 모아이 포스 필드]";
            case "불가사의 - 성간 에너지":
                return "[특수 자원 : 성간 에너지 추출기]";
            case "불가사의 - 유전 연구학":
                return "[특수 자원 : Ubermensch]";
            default:
                return "Resource Unknown";
        }
    }

    static public Sprite GetResourceSprite(Quest qst) {
        string resourceSpriteName = "";
        switch (qst.TextName) {
            case "불가사의 - 오티즘 빔 반사 어레이":
                resourceSpriteName = "hwan_autism_ray_reflection";
                break;
            case "첩보 - 크툴루 계획":
                resourceSpriteName = "hwan_cthulhu_info";
                break;
            case "불가사의- 이집트 캉덤":
                resourceSpriteName = "hwan_pyramid";
                break;
            case "[전쟁 동맹] - 에뮤 연방":
                resourceSpriteName = "finno_crystal";
                break;
            case "[불가사의] - 아틀란티스":
                resourceSpriteName = "finno_necronimicon";
                break;
            case "[불가사의] - R'̧l̨̜y͎͎̜̺̬e͕͇͇͚͓̹h̢̳͎̗͇͇̙":
                resourceSpriteName = "finno_gate";
                break;
            case "군사 동맹 - 궤도 장악권":
                resourceSpriteName = "hwan_spacetrack";
                break;
            case "건물 기증 - 모아이 포스 필드":
                resourceSpriteName = "hwan_moai";
                break;
            case "불가사의 - 성간 에너지":
                resourceSpriteName = "finno_energy";
                break;
            case "불가사의 - 유전 연구학":
                resourceSpriteName = "finno_ubermensch";
                break;
            default:
                resourceSpriteName = "hwan_spacetrack";
                break;
        }
        return Resources.Load<Sprite>("SpecialResource/" + resourceSpriteName);
    }

}
