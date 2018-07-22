using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialResourceTraits : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public static string GetSpecialResourceName(CivModel.ISpecialResource specialResource)
    {
        char[] sep = { '.' };
        string name = specialResource.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            case "AutismBeamAmplificationCrystal":
                result = "오티즘 빔 증폭 크리스탈";
                break;
            case "GatesOfRlyeh":
                result = "GatesOfRlyeh";
                break;
            case "InterstellarEnergyExtractor":
                result = "성간 에너지 추출기";
                break;
            case "Necronomicon":
                result = "네크로노미콘";
                break;
            case "SpecialResourceAirspaceDomination":
                result = "궤도 장악권";
                break;
            case "SpecialResourceAlienCommunication":
                result = "가자 피라미드 외계 통신 기구";
                break;
            case "SpecialResourceAutismBeamReflex":
                result = "오티즘빔 반사 어레이";
                break;
            case "SpecialResourceCthulhuProjectInfo":
                result = "크툴루 계획 기밀 정보";
                break;
            case "SpecialResourceMoaiForceField":
                result = "모아이 포스 필드";
                break;
            case "Ubermensch":
                result = "Ubermensch";
                break;
            default:
                result = "Unknown: " + name;
                break;
        }

        return result;
    }
}
