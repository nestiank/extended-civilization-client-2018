using CivModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Units {
	HwanPioneer,
	HwanBrainwashedEmuKnight,
	HwanDecentralizedMilitary,
	HwanSpy,
	HwanUnicornOrder,
	HwanLEO,
	HwanJediKnight,
	HwanProtoNinja,
	HwanJackieChan,
	FinnoPioneer,
	FinnoEmuHorseArcher,
	FinnoDecentralizedMilitary,
	FinnoSpy,
	FinnoElephantCavarly,
	FinnoAncientSorcerer,
	FinnoJediKnight,
	FinnoAutismBeamDrone,
	FinnoGenghisKhan
}

public class UnitEnum {
	public static Units UnitToEnum(CivModel.Unit unit) {
		if (unit is CivModel.Hwan.Pioneer) {
			return Units.HwanPioneer;
		}
		else if (unit is CivModel.Hwan.BrainwashedEMUKnight) {
			return Units.HwanBrainwashedEmuKnight;
		}
		else if (unit is CivModel.Hwan.DecentralizedMilitary) {
			return Units.HwanDecentralizedMilitary;
		}
		else if (unit is CivModel.Hwan.Spy) {
			return Units.HwanSpy;
		}
		else if (unit is CivModel.Hwan.UnicornOrder) {
			return Units.HwanUnicornOrder;
		}
		else if (unit is CivModel.Hwan.LEOSpaceArmada) {
			return Units.HwanLEO;
		}
		else if (unit is CivModel.Hwan.JediKnight) {
			return Units.HwanJediKnight;
		}
		else if (unit is CivModel.Hwan.ProtoNinja) {
			return Units.HwanProtoNinja;
		}
		else if (unit is CivModel.Hwan.JackieChan) {
			return Units.HwanJackieChan;
		}
		else if (unit is CivModel.Finno.Pioneer) {
			return Units.FinnoPioneer;
		}
		else if (unit is CivModel.Finno.EMUHorseArcher) {
			return Units.FinnoEmuHorseArcher;
		}
		else if (unit is CivModel.Finno.DecentralizedMilitary) {
			return Units.FinnoDecentralizedMilitary;
		}
		else if (unit is CivModel.Finno.Spy) {
			return Units.FinnoSpy;
		}
		else if (unit is CivModel.Finno.ElephantCavalry) {
			return Units.FinnoElephantCavarly;
		}
		else if (unit is CivModel.Finno.AncientSorcerer) {
			return Units.FinnoAncientSorcerer;
		}
		else if (unit is CivModel.Finno.JediKnight) {
			return Units.FinnoJediKnight;
		}
		else if (unit is CivModel.Finno.AutismBeamDrone) {
			return Units.FinnoAutismBeamDrone;
		}
		else {
			return Units.HwanBrainwashedEmuKnight;
		}
	}

    public static GameObject GetUnitGameObject(CivModel.Unit unit) {
        
        string unitResourcePath = "";

        if (unit is CivModel.Hwan.Pioneer)
        {
            unitResourcePath = "hwan_burrow";
        }
        else if (unit is CivModel.Hwan.BrainwashedEMUKnight)
        {
            unitResourcePath =  "hwan_emu_knight";
        }
        else if (unit is CivModel.Hwan.DecentralizedMilitary)
        {
            unitResourcePath =  "hwan_decentralized_soldier";
        }
        else if (unit is CivModel.Hwan.Spy)
        {
            unitResourcePath = "hwan_secret_inspector";
        }
        else if (unit is CivModel.Hwan.UnicornOrder)
        {
            unitResourcePath =  "hwan_unicorn";
        }
        else if (unit is CivModel.Hwan.LEOSpaceArmada)
        {
            unitResourcePath =  "hwan_spaceship";
        }
        else if (unit is CivModel.Hwan.JediKnight)
        {
            unitResourcePath =  "hwan_jedi";
        }
        else if (unit is CivModel.Hwan.ProtoNinja)
        {
            unitResourcePath =  "hwan_ninja";
        }
        else if (unit is CivModel.Hwan.JackieChan)
        {
            unitResourcePath =  "hwan_jackie_chan";
        }
        else if (unit is CivModel.Finno.Pioneer)
        {
            unitResourcePath = "finno_burrow";
        }
        else if (unit is CivModel.Finno.EMUHorseArcher)
        {
            unitResourcePath = "finno_emu_archer";
        }
        else if (unit is CivModel.Finno.DecentralizedMilitary)
        {
            unitResourcePath = "finno_decentralized_soldier";
        }
        else if (unit is CivModel.Finno.Spy)
        {
            unitResourcePath = "finno_spy";
        }
        else if (unit is CivModel.Finno.ElephantCavalry)
        {
            unitResourcePath = "finno_elephant";
        }
        else if (unit is CivModel.Finno.AncientSorcerer)
        {
            unitResourcePath = "finno_sorcerer";
        }
        else if (unit is CivModel.Finno.JediKnight)
        {
            unitResourcePath =  "finno_jedi";
        }
        else if (unit is CivModel.Finno.AutismBeamDrone)
        {
            unitResourcePath = "finno_autism_drone";
        }
        else if (unit is CivModel.Finno.GenghisKhan)
        {
            unitResourcePath = "finno_genghis_khan";
        }
        else
        {
            unitResourcePath = "hwan_spaceship";
        }

        return Resources.Load<GameObject>("UnitModels/" + unitResourcePath);
    }

}