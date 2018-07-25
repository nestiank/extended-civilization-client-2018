using CivModel;

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
}