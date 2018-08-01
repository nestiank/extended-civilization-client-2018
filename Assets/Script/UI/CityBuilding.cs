using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CivModel;
using CivModel.Common;
using System;

public class CityBuilding : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public static string ListCityBuildings(IReadOnlyList<InteriorBuilding> interiorBuildings)
    {
        Dictionary<string, int> cityBuildingDic = new Dictionary<string, int>();

        string text = "";

        foreach (CivModel.InteriorBuilding cityBuilding in interiorBuildings)
        {
            string cityBuildingName = GetName(cityBuilding);
            if (cityBuildingDic.ContainsKey(cityBuildingName))
            {
                cityBuildingDic[cityBuildingName]++;
            }
            else
            {
                cityBuildingDic[cityBuildingName] = 1;
            }
        }

        foreach (string key in cityBuildingDic.Keys)
        {
            string cityBuildingTxt = key + " X" + cityBuildingDic[key] + "\n";
            text += cityBuildingTxt;
        }

        return text;
    }

    private static string GetName(CivModel.InteriorBuilding cityBuilding)
    {
        char[] sep = { '.' };
        string name = cityBuilding.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            // DEPRECATED
            case "CityLab": // 연구소
                result = "연구소";
                break;

            case "FIRFactory": // 공장
                result = "공장";
                break;

            // 1. City Buildings

            case "HwanEmpireCityCentralLab": // 환제국도시연구소
                result = "환제국도시연구소";
                break;

            case "HwanEmpireFIRFactory": // 5차산업혁명공장
                result = "5차산업혁명공장";
                break;

            // 오타 실화냐;;
            case "AncientFinnoLabortory": // 피노연구소
                result = "고대핀란드도시연구소";
                break;

            case "AncientFinnoFIRFactory": // 5차산업혁명공장
                result = "고대핀란드5차산업혁명공장";
                break;

            case "HwanEmpireSungsimdang": // 성심당
                result = "성심당";
                break;
            case "AncientFinnoXylitolProductionRegion": // 자일리톨
                result = "자일리톨 생산지";
                break;
            case "HwanEmpireVigilant": // 환 자경단
                result = "환 자경단";
                break;
            case "AncientFinnoVigilant": //피노 자경단
                result = "고대 핀란드 자경단";
                break;
            default:
                result = "unknown : " + name;
                Debug.Log(result);
                break;
        }
        return result;
    }
}
