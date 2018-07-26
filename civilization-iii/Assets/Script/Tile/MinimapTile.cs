using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;


public class MinimapTile : MonoBehaviour {

	public CivModel.Terrain.Point point;
	public Vector3 unity_point;

	Transform owner_color;
	Transform city_active;
	Player owner;
	TileBuilding City;

	// Use this for initialization
	void Start () {
		SetOwner();
		SetCity();
	}
	public void SetPoints(CivModel.Terrain.Point p1) {
		this.point = p1;
		this.unity_point = GameManager.ModelPntToUnityPnt(p1, -200f);
	}

	public void SetPoints(CivModel.Terrain.Point p1, Vector3 p2) {
		this.point = p1;
		this.unity_point = new Vector3(p2.x, p2.y, p2.z);
	}

	public void SetOwner() {
		
		owner_color = transform.GetChild(0).transform;
		owner = point.TileOwner;
		if (owner == null) {
			owner_color.GetChild(9).gameObject.SetActive(true);
			if ((int)point.Type == 1) {
				owner_color.GetChild(9).gameObject.SetActive(false);
				owner_color.GetChild(10).gameObject.SetActive(true);
			}
		}
		
		else if (owner == GameManager.Instance.Game.Players[0]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(0).gameObject.SetActive(true);
		}
		else if (owner == GameManager.Instance.Game.Players[1]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(1).gameObject.SetActive(true);
		}
		else if (owner == GameManager.Instance.Game.Players[2]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(2).gameObject.SetActive(true);
		}
		else if (owner == GameManager.Instance.Game.Players[3]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(3).gameObject.SetActive(true);
		}
		else if (owner == GameManager.Instance.Game.Players[4]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(4).gameObject.SetActive(true);
		}
		else if (owner == GameManager.Instance.Game.Players[5]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(5).gameObject.SetActive(true);
		}
		else if (owner == GameManager.Instance.Game.Players[6]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(6).gameObject.SetActive(true);
		}
		else if (owner == GameManager.Instance.Game.Players[7]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(7).gameObject.SetActive(true);
		}
		else if (owner == GameManager.Instance.Game.Players[8]) {
            owner_color.GetChild(9).gameObject.SetActive(false);
            owner_color.GetChild(10).gameObject.SetActive(false);
            owner_color.GetChild(8).gameObject.SetActive(true);
		}
		
	}

	public void SetCity() {
		City = point.TileBuilding;
		city_active = transform.GetChild(1).transform;
		if(City is CityBase) {
			city_active.GetChild(0).gameObject.SetActive(true);
		}
	}
}
