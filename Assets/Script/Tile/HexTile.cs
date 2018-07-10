using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;


public class HexTile : MonoBehaviour
{
    // 현재 tile의 위치의 point class
    public CivModel.Terrain.Point point;

    // Unity에서 그려지는 벡터 포지션
    public Vector3 unityPoint;

	// prefab의 자식 gameobject들 
	Transform terrains;
	Transform buildings;

	CivModel.TileBuilding building;

	public bool isFirstClick = true;

    public bool isFlickering;
    private IEnumerator _coroutine;

	// Use this for initialization
	void Start() {
        SetTerrain();
		SetBuilding();
	}

    public void SetPoints(CivModel.Terrain.Point p1) {
        this.point = p1;
        this.unityPoint = GameManager.ModelPntToUnityPnt(p1, -0.05f);
    }

    public void SetPoints(CivModel.Terrain.Point p1, Vector3 p2) {
        this.point = p1;
        this.unityPoint = new Vector3(p2.x, p2.y, p2.z);
    }

	// Render tile terrain
	public void SetTerrain() {
        terrains = transform.GetChild(0).transform;

		if (terrains != null) {
            foreach (Transform child in terrains) {
                child.gameObject.SetActive(false);
            }

			terrains.GetChild((int)point.Type).gameObject.SetActive(true);
		}
	}

	public void SetBuilding() {
        buildings = transform.GetChild(1).transform;
        building = point.TileBuilding;

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

	public void FlickerCyan() {
		isFlickering = true;
		Debug.Log(gameObject.name + " is flickering with cyan");
		if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
			return;
		_coroutine = Flicker(Color.cyan);
		StartCoroutine(_coroutine);
	}

	// Flicker with blue color. This is used for parametered move and skill.
	public void FlickerBlue()
    {
        isFlickering = true;
        Debug.Log(gameObject.name + " is flickering with blue");
        if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.blue);
        StartCoroutine(_coroutine);
    }

    // Blink with red color. This is used for attack.
    public void FlickerRed()
    {
        isFlickering = true;
        Debug.Log(gameObject.name + " is flickering with red");
        if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.red);
        StartCoroutine(_coroutine);
    }

    public void StopFlickering()
    {
        isFlickering = false;
        Debug.Log(gameObject.name + " stopped flickering");
        if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
            return;
        if (_coroutine == null)
            return;
        StopCoroutine(_coroutine);
        Material mat = terrains.GetChild((int)point.Type).GetComponent<Renderer>().material;
        mat.SetColor("_Color", Color.white);
    }

    // Make tile flicker with color c. Don't need to read this method.
    IEnumerator Flicker(Color c)
    {
        Material mat = terrains.GetChild((int)point.Type).GetComponent<Renderer>().material;
        Color delta = Color.white - c;

        while (true)
        {
            // From white to c
            for (float i = 0; i <= 1f; i += 1.5f * Time.deltaTime)
            {
                mat.SetColor("_Color", Color.white - delta * (1 - Mathf.Cos(Mathf.PI * i)) / 2);
                yield return null;
            }
            mat.SetColor("_Color", c);
            // From c to white
            for (float i = 0; i <= 1f; i += 1.5f * Time.deltaTime)
            {
                mat.SetColor("_Color", c + delta * (1 - Mathf.Cos(Mathf.PI * i)) / 2);
                yield return null;
            }
            mat.SetColor("_Color", Color.white);

            if (!isFlickering)
            {
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }



}
