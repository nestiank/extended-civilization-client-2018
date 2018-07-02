using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexTile : MonoBehaviour
{
    // 현재 tile의 위치의 point class
    public CivModel.Terrain.Point point;

	// prefab의 자식 gameobject들 
	Transform terrains;
	Transform buildings;

	CivModel.TileBuilding building;


	// Use this for initialization
	void Start() {
		terrains = transform.GetChild(0).transform;
		SetTerrain();

		buildings = transform.GetChild(1).transform;
		building = point.TileBuilding;
		SetBuilding();
	}

	// Render tile terrain
	public void SetTerrain() {
		if (terrains != null) {
			terrains.GetChild((int)point.Type).gameObject.SetActive(true);
		}
	}

	public void SetBuilding() {
		Transform side;

		if (building is CivModel.CityBase) {
			buildings.GetChild(2).gameObject.SetActive(true);
			side = buildings.GetChild((building.Owner.Team + 1) % 2);
			side.GetChild(0).gameObject.SetActive(true);
		}
		else if (building is CivModel.Hwan.HwanEmpireIbiza) {
			side = buildings.GetChild(0);
			side.GetChild(1).gameObject.SetActive(true);
		}
		else if (building is CivModel.Finno.AncientFinnoOctagon) {
			side = buildings.GetChild(1);
			side.GetChild(1).gameObject.SetActive(true);
		}
		else if (building is CivModel.Hwan.HwanEmpireLatifundium) {
			side = buildings.GetChild(0);
			side.GetChild(2).gameObject.SetActive(true);
		}
		else if (building is CivModel.Finno.AncientFinnoGermaniumMine) {
			side = buildings.GetChild(1);
			side.GetChild(2).gameObject.SetActive(true);
		}
		else if (building is CivModel.Hwan.HwanEmpireFIRFortress) {
			side = buildings.GetChild(0);
			side.GetChild(3).gameObject.SetActive(true);
		}
		else if (building is CivModel.Finno.AncientFinnoFIRFortress) {
			side = buildings.GetChild(1);
			side.GetChild(3).gameObject.SetActive(true);
		}
		else if (building is CivModel.Hwan.HwanEmpireKimchiFactory) {
			side = buildings.GetChild(0);
			side.GetChild(4).gameObject.SetActive(true);
		}
		else if (building is CivModel.Finno.AncientFinnoFineDustFactory) {
			side = buildings.GetChild(1);
			side.GetChild(4).gameObject.SetActive(true);
		}
	}



}
