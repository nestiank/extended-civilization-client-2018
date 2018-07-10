using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionFactoryTraits : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static string GetFactoryName(CivModel.IProductionFactory Factory)
    {
        char[] sep = { '.' };
        string name = Factory.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            case "PioneerProductionFactory":
                result = "개척자";
                break;
            case "JediKnightProductionFactory":
                result = "제다이 기사";
                break;
            case "FakeKnightProductionFactory":
                result = "가짜 기사(테스팅)";
                break;
            case "BrainwashedEMUKnightProductionFactory":
                result = "세뇌된 에뮤 기사";
                break;
            case "DecentralizedMilitaryProductionFactory":
                result = "탈중앙화된 군인";
                break;
            case "JackieChanProductionFactory":
                result = "재키 찬";
                break;
            case "LEOSpaceArmadaProductionFactory":
                result = "저궤도 우주 함대";
                break;
            case "ProtoNinjaProductionFactory":
                result = "프로토-닌자";
                break;
            case "UnicornOrderProductionFactory":
                result = "유니콘 기사단";
                break;
            case "SpyProductionFactory":
                result = "스파이";
                break;
            case "AncientSorcererProductionFactory":
                result = "고대 소서러";
                break;
            case "AutismBeamDroneFactory":
                result = "O-ti-ism 빔 드론";
                break;
            case "ElephantCavalryProductionFactory":
                result = "코끼리 기병";
                break;
            case "EMUHorseArcherProductionFactory":
                result = "에뮤 궁기병";
                break;
            case "GenghisKhanProductionFactory":
                result = "징기즈 칸";
                break;
            case "ArmedDivisionProductionFactory":
                result = "기갑사단";
                break;
            case "InfantryDivisionProductionFactory":
                result = "보병사단";
                break;
            case "PadawanProductionFactory":
                result = "파다완";
                break;
            case "ZapNinjaProductionFactory":
                result = "닌자";
                break;
            case "CityCenterProductionFactory":
                result = "도심부";
                break;
            case "HwanEmpireCityProductionFactory":
                result = "환 제국 도시";
                break;
            case "HwanEmpireFIRFortressProductionFactory":
                result = "환 제국 4차 산업 요새";
                break;
            case "HwanEmpireCityCentralLabProductionFactory":
                result = "환 제국 도시 연구소";
                break;
            case "HwanEmpireFIRFactoryProductionFactory":
                result = "환 제국 4차 산업 공장";
                break;
            case "HwanEmpireIbizaProductionFactory":
                result = "환 제국 이비자";
                break;
            case "HwanEmpireKimchiFactoryProductionFactory":
                result = "환 제국 김치 군수공장";
                break;
            case "HwanEmpireLatifundiumProductionFactory":
                result = "환 제국 라티푼디움";
                break;
            case "AncientFinnoFineDustFactoryProductionFactory":
                result = "고대 수오미 제국 미세먼지 공장";
                break;
            case "AncientFinnoFIRFortressProductionFactory":
                result = "고대 수오미 제국 4차 산업 요새";
                break;
            case "AncientFinnoGermaniumMineProductionFactory":
                result = "고대 수오미 제국 게르마늄 광산";
                break;
            case "AncientFinnoOctagonProductionFactory":
                result = "고대 수오미 제국 옥타곤";
                break;
            case "FinnoEmpireCityProductionFactory":
                result = "고대 수오미 제국 도시";
                break;
            case "CasinoProductionFactory":
                result = "카지노";
                break;
            case "FIRFortressProductionFactory":
                result = "4차 산업 요새";
                break;
            case "ZapFactoryBuildingProductionFactory":
                result = "공장";
                break;
            case "FactoryBuildingProductionFactory":
                result = "공장";
                break;
            case "LaboratoryBuildingProductionFactory":
                result = "연구소";
                break;
            default:
                result = "unknown : " + name;
                break;
        }
        return result;
    }
    public static string GetFacPortName(CivModel.IProductionFactory Factory)
    {
        char[] sep = { '.' };
        string name = Factory.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            case "PioneerProductionFactory":
                result = "Pioneer";
                break;
            case "JediKnightProductionFactory":
                {
                    if (Factory.ToString().Split(sep)[1].Equals("Hwan"))
                        result = "hwan_jedi";
                    else if (Factory.ToString().Split(sep)[1].Equals("Finno"))
                        result = "finno_jedi";
                    else result = "unknown : " + name;
                }
                break;
            case "FakeKnightProductionFactory":
                result = "JediKnight";
                break;
            case "BrainwashedEMUKnightProductionFactory":
                result = "hwan_emu";
                break;
            case "DecentralizedMilitaryProductionFactory":
                {
                    if (Factory.ToString().Split(sep)[1].Equals("Hwan"))
                        result = "hwan_decentralized_soldier";
                    else if (Factory.ToString().Split(sep)[1].Equals("Finno"))
                        result = "finno_decentralized_soldier";
                    else result = "unknown : " + name;
                }
                break;
            case "JackieChanProductionFactory":
                result = "hwan_jackie";
                break;
            case "LEOSpaceArmadaProductionFactory":
                result = "hwan_spaceship";
                break;
            case "ProtoNinjaProductionFactory":
                {
                    if (Factory.ToString().Split(sep)[1].Equals("Hwan"))
                        result = "hwan_ninja";
                    else if (Factory.ToString().Split(sep)[1].Equals("Finno"))
                        result = "finno_ninja";
                    else result = "unknown : " + name;
                }
                break;
            case "UnicornOrderProductionFactory":
                result = "hwan_unicorn";
                break;
            case "SpyProductionFactory":
                {
                    if (Factory.ToString().Split(sep)[1].Equals("Hwan"))
                        result = "hwan_spy";
                    else if (Factory.ToString().Split(sep)[1].Equals("Finno"))
                        result = "finno_spy";
                    else result = "unknown : " + name;
                }
                break;
            case "AncientSorcererProductionFactory":
                result = "finno_sorcerer";
                break;
            case "AutismBeamDroneFactory":
                result = "finno_autism_drone";
                break;
            case "ElephantCavalryProductionFactory":
                result = "finno_elephant";
                break;
            case "EMUHorseArcherProductionFactory":
                result = "finno_emu";
                break;
            case "GenghisKhanProductionFactory":
                result = "finno_genghis";
                break;
            case "ArmedDivisionProductionFactory":
                result = "ArmedDivision";
                break;
            case "InfantryDivisionProductionFactory":
                result = "InfantryDivision";
                break;
            case "PadawanProductionFactory":
                result = "Padawan";
                break;
            case "ZapNinjaProductionFactory":
                result = "ZapNinja";
                break;
            case "CityCenterProductionFactory":
                {
                    if (Factory.ToString().Split(sep)[1].Equals("Hwan"))
                        result = "City Buildings/hwan_city";
                    else if (Factory.ToString().Split(sep)[1].Equals("Finno"))
                        result = "City Buildings/finno_city";
                    else result = "unknown : " + name;
                }
                break;
            case "HwanEmpireCityProductionFactory":
                result = "HwanEmpireCity";
                break;
            case "HwanEmpireFIRFortressProductionFactory":
                result = "HwanEmpireFIRFortress";
                break;
            case "HwanEmpireCityCentralLabProductionFactory":
                result = "HwanEmpireCityCentralLab";
                break;
            case "HwanEmpireFIRFactoryProductionFactory":
                result = "City Buildings/hwan_factory";
                break;
            case "HwanEmpireIbizaProductionFactory":
                result = "HwanEmpireIbiza";
                break;
            case "HwanEmpireKimchiFactoryProductionFactory":
                result = "HwanEmpireKimchiFactory";
                break;
            case "HwanEmpireLatifundiumProductionFactory":
                result = "HwanEmpireLatifundium";
                break;
            case "AncientFinnoFineDustFactoryProductionFactory":
                result = "AncientFinnoFineDustFactory";
                break;
            case "AncientFinnoFIRFortressProductionFactory":
                result = "AncientFinnoFIRFortress";
                break;
            case "AncientFinnoGermaniumMineProductionFactory":
                result = "AncientFinnoGermaniumMine";
                break;
            case "AncientFinnoOctagonProductionFactory":
                result = "AncientFinnoOctagon";
                break;
            case "FinnoEmpireCityProductionFactory":
                result = "FinnoEmpireCity";
                break;
            case "CasinoProductionFactory":
                result = "Casino";
                break;
            case "FIRFortressProductionFactory":
                result = "FIRFortress";
                break;
            case "ZapFactoryBuildingProductionFactory":
                result = "ZapFactoryBuilding";
                break;
            case "FactoryBuildingProductionFactory":
                {
                    if (Factory.ToString().Split(sep)[1].Equals("Hwan"))
                        result = "City Buildings/hwan_factory";
                    else if (Factory.ToString().Split(sep)[1].Equals("Finno"))
                        result = "City Buildings/finno_factory";
                    else result = "unknown : " + name;
                }
                break;
            case "LaboratoryBuildingProductionFactory":
                {
                    if (Factory.ToString().Split(sep)[1].Equals("Hwan"))
                        result = "City Buildings/hwan_city_lab";
                    else if (Factory.ToString().Split(sep)[1].Equals("Finno"))
                        result = "City Buildings/finno_city_lab";
                    else result = "unknown : " + name;
                }
                break;
            default:
                result = "unknown : " + name;
                break;
        }
        return result;
    }
    public static string GetName(CivModel.Actor unit)
    {
        char[] sep = { '.' };
        string name = unit.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            case "Pioneer":
                result = "개척자";
                break;
            case "JediKnight":
                result = "제다이 기사";
                break;
            case "CityCenter":
                result = "도심부";
                break;
            case "FakeKnight":
                result = "가짜 기사(테스팅)";
                break;
            case "FactoryBuilding":
                result = "공장";
                break;
            case "LaboratoryBuilding":
                result = "연구소";
                break;
            case "BrainwashedEMUKnight":
                result = "세뇌된 에뮤 기사";
                break;
            case "DecentralizedMilitary":
                result = "탈중앙화된 군인";
                break;
            case "JackieChan":
                result = "재키 찬";
                break;
            case "LEOSpaceArmada":
                result = "저궤도 우주 함대";
                break;
            case "ProtoNinja":
                result = "프로토-닌자";
                break;
            case "UnicornOrder":
                result = "유니콘 기사단";
                break;
            case "Spy":
                result = "스파이";
                break;
            case "AncientSorcerer":
                result = "고대 소서러";
                break;
            case "AutismBeamDrone":
                result = "O-ti-ism 빔 드론";
                break;
            case "ElephantCavalry":
                result = "코끼리 기병";
                break;
            case "EMUHorseArcher":
                result = "에뮤 궁기병";
                break;
            case "GenghisKhan":
                result = "징기즈 칸";
                break;
            case "ArmedDivision":
                result = "기갑사단";
                break;
            case "InfantryDivision":
                result = "보병사단";
                break;
            case "Padawan":
                result = "파다완";
                break;
            case "ZapNinja":
                result = "닌자";
                break;
            default:
                result = "unknown : " + name;
                break;
        }
        return result;
    }
    public static string GetPortName(CivModel.Actor unit)
    {
        char[] sep = { '.' };
        string name = unit.ToString().Split(sep)[2];
        string result;
        switch (name)
        {
            case "Pioneer":
                result = "pioneer";
                break;
            case "JediKnight":
                {
                    if (unit.Owner == GameManager.Instance.Game.Players[CivModel.Hwan.HwanPlayerConstant.HwanPlayer])
                        result = "hwan_jedi";
                    else if (unit.Owner == GameManager.Instance.Game.Players[CivModel.Finno.FinnoPlayerConstant.FinnoPlayer])
                        result = "finno_jedi";
                    else result = "unknown : " + name;
                }
                break;
            case "FakeKnight":
                result = "hwan_jedi";
                break;
            case "BrainwashedEMUKnight":
                result = "hwan_emu";
                break;
            case "DecentralizedMilitary":
                {
                    if (unit.Owner == GameManager.Instance.Game.Players[CivModel.Hwan.HwanPlayerConstant.HwanPlayer])
                        result = "hwan_decentralized_soldier";
                    else if (unit.Owner == GameManager.Instance.Game.Players[CivModel.Finno.FinnoPlayerConstant.FinnoPlayer])
                        result = "finno_decentralized_soldier";
                    else result = "unknown : " + name;
                }
                break;
            case "JackieChan":
                result = "hwan_jackie";
                break;
            case "LEOSpaceArmada":
                result = "hwan_spaceship";
                break;
            case "ProtoNinja":
                {
                    if (unit.Owner == GameManager.Instance.Game.Players[CivModel.Hwan.HwanPlayerConstant.HwanPlayer])
                        result = "hwan_ninja";
                    else if (unit.Owner == GameManager.Instance.Game.Players[CivModel.Finno.FinnoPlayerConstant.FinnoPlayer])
                        result = "finno_ninja";
                    else result = "unknown : " + name;
                }
                break;
            case "UnicornOrder":
                result = "hwan_unicorn";
                break;
            case "Spy":
                {
                    if (unit.Owner == GameManager.Instance.Game.Players[CivModel.Hwan.HwanPlayerConstant.HwanPlayer])
                        result = "hwan_spy";
                    else if (unit.Owner == GameManager.Instance.Game.Players[CivModel.Finno.FinnoPlayerConstant.FinnoPlayer])
                        result = "finno_spy";
                    else result = "unknown : " + name;
                }
                break;
            case "AncientSorcerer":
                result = "finno_sorcerer";
                break;
            case "AutismBeamDrone":
                result = "finno_autism_drone";
                break;
            case "ElephantCavalry":
                result = "finno_elephant";
                break;
            case "EMUHorseArcher":
                result = "finno_emu";
                break;
            case "GenghisKhan":
                result = "finno_genghis";
                break;
            case "ArmedDivision":
                result = "기갑사단";
                break;
            case "InfantryDivision":
                result = "보병사단";
                break;
            case "Padawan":
                result = "파다완";
                break;
            case "ZapNinja":
                result = "닌자";
                break;
            default:
                result = "unknown : " + name;
                break;
        }
        return result;
    }
}
