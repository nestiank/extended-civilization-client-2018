using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;


public class HexTile : MonoBehaviour
{
    // CivModel.Terrain.Point Attributes of the Tile.
    // Associated With the Model, But as it's a pointer, Access is Required.
    public CivModel.Terrain.Point point;

    // Unity Transform Position of the Unit
    public Vector3 unityPoint;

	// Child GameObjects of the Tile
	Transform terrains;
	Transform buildings;
    Color color;

    // Tile Building of the Tile
	CivModel.TileBuilding building;

	public bool isFirstClick = true;

    public bool isFlickering;
    public bool isFlickerForSelect;
    private IEnumerator _coroutine;

	// Use this for initialization
	void Start() {
        SetTerrain();
		SetBuilding();
    }
    
    // Change Tile position to given CivModel.Terrain.Point value
    // Default y position is -.0.05f
    public void SetPoints(CivModel.Terrain.Point p1) {
        this.point = p1;
        this.unityPoint = GameManager.ModelPntToUnityPnt(p1, -0.05f);
    }
    // Change Tile position to given CivModel.Terrain.Point value
    public void SetPoints(CivModel.Terrain.Point p1, Vector3 p2) {
        this.point = p1;
        this.unityPoint = new Vector3(p2.x, p2.y, p2.z);
    }

	// Render Tile Terrain
	public void SetTerrain() {
        int type = (int)point.Type;
        int owner = GetPlayerNumber();
        for(int i = 0; i < 8; i++)
            for(int j = 0; j < 10; j++)
                transform.GetChild(0).GetChild(i).GetChild(j).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(type).GetChild(owner).gameObject.SetActive(true);
	}

    // Render Tile Building
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
        else if(building is CivModel.Hwan.Preternaturality) {
            side = buildings.GetChild(0);
            side.GetChild(5).gameObject.SetActive(true);
        }
        else if(building is CivModel.Finno.Preternaturality) {
            side = buildings.GetChild(0);
            side.GetChild(5).gameObject.SetActive(true);
        }
	}

    // Set player number which model didn't implemented
    public int GetPlayerNumber()
    {
        var owner = point.TileOwner;
        if (owner == null) return 9;
        else
            for (int i = 0; i < 9; i++)
                if (owner == GameManager.Instance.Game.Players[i])
                    return i;
        return 9;
    }

    // Flicker Tile with White Color
    public void FlickerWhite()
    {
        isFlickerForSelect = true;
        if (transform.GetChild(0).GetChild((int)point.Type).GetChild(GetPlayerNumber()).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.white);
        StartCoroutine(_coroutine);
    }
    // Flicker Tile with Cyan Color
    public void FlickerCyan() {
        isFlickerForSelect = true;
        //Debug.Log(gameObject.name + " is flickering with cyan");
        if (transform.GetChild(0).GetChild((int)point.Type).GetChild(GetPlayerNumber()).GetComponent<Renderer>() == null)
            return;
		_coroutine = Flicker(Color.cyan);
		StartCoroutine(_coroutine);
	}
	// Flicker Tile with Blue Color. This is used for parametered move and skill.
	public void FlickerBlue()
    {
        isFlickering = true;
        //Debug.Log(gameObject.name + " is flickering with blue");
        if (transform.GetChild(0).GetChild((int)point.Type).GetChild(GetPlayerNumber()).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.blue);
        StartCoroutine(_coroutine);
    }

    // Flicker Tile with Red Color. This is used for attack.
    public void FlickerRed()
    {
        isFlickering = true;
        //Debug.Log(gameObject.name + " is flickering with red");
        if (transform.GetChild(0).GetChild((int)point.Type).GetChild(GetPlayerNumber()).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.red);
        StartCoroutine(_coroutine);
    }

    // Stop Flickering Tile
    public void StopFlickering()
    {
        isFlickering = false;
        isFlickerForSelect = false;
        //Debug.Log(gameObject.name + " stopped flickering");
        if (transform.GetChild(0).GetChild((int)point.Type).GetChild(GetPlayerNumber()).GetComponent<Renderer>() == null)
            return;
        if (_coroutine == null)
            return;
        StopCoroutine(_coroutine);
        transform.GetChild(0).GetChild((int)point.Type).GetChild(GetPlayerNumber()).GetComponent<Renderer>().material.SetColor("_Color", Color.white);
    }

    // Make tile flicker with color c.
    IEnumerator Flicker(Color c)
    {
        Material mat = transform.GetChild(0).GetChild((int)point.Type).GetChild(GetPlayerNumber()).GetComponent<Renderer>().material;
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


    private bool _inSkillState = false;
    public bool SkillState { get { return _inSkillState; } }

    // Initial Skill Index is -1
    private int _currentSkill = -1;


    // ParameterPoints which are target of the Skill
    private List<CivModel.Terrain.Point?> _skillParameterPoints = new List<CivModel.Terrain.Point?>();


    public void SkillStateEnter(int index)
    {
        // State change
        if (_inSkillState && _currentSkill == index) return;
        _inSkillState = true;
        _currentSkill = index;

        // If SpecialActs does not exist, exit skill state.
        if (GameManager.Instance.selectedActor.SpecialActs == null)
        {
            Debug.Log("Special Acts Does not Exist");
            SkillStateExit();
            return;
        }

        // If SpecialActs[_currentSkill] does not exist, exit skill state.
        if (GameManager.Instance.selectedActor.SpecialActs[_currentSkill] == null)
        {
            Debug.Log("Special Acts of Current Skill Does not Exist");
            SkillStateExit();
            return;
        }

        // If SpecialActs[_currentSkill] is not parametered skill, this skill is immediately activated.
        if (!GameManager.Instance.selectedActor.SpecialActs[_currentSkill].IsParametered)
        {
            Debug.Log("HexTile Using Unparametered Skill!");
            GameManager.Instance.selectedActor.SpecialActs[_currentSkill].Act(null);
            GameManager.Instance.UpdateUnit();
            return;
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.Game.Terrain.Width; i++)
            {
                for (int j = 0; j < GameManager.Instance.Game.Terrain.Height; j++)
                {
                    CivModel.Terrain.Point? pnt = GameManager.Instance.Game.Terrain.GetPoint(i, j);
                    if (GameManager.Instance.selectedActor.SpecialActs[_currentSkill].IsActable(pnt))
                    {
                        CivModel.Position pos = pnt.Value.Position;
                        GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
                        _skillParameterPoints.Add(pnt);
                    }
                }
            }

            if (_skillParameterPoints.Count == 0)
            {
                Debug.Log("NoWhere To Use Skill");
                SkillStateExit();
                return;
            }

            IEnumerator skillCoroutine = SkillUnit(GameManager.Instance.selectedActor);
            StartCoroutine(skillCoroutine);
        }
    }

    IEnumerator SkillUnit(CivModel.Actor actorToSkill)
    {
        while (true)
        {
            CivModel.Terrain.Point destPoint = GameManager.Instance.selectedPoint;
            // 새로운 Point 을 선택했을 때
            if (actorToSkill.PlacedPoint.Value != destPoint)
            {
                // Flicker하고 있는 Tile을 선택했을 때
                if (GameManager.Instance.selectedTile.isFlickering)
                {
                    if (actorToSkill.SpecialActs[_currentSkill].IsActable(destPoint))
                    {
                        actorToSkill.SpecialActs[_currentSkill].Act(destPoint);
                        SkillStateExit();
                        GameManager.Instance.UpdateUnit();
                        break;
                    }
                    else
                    {
                        SkillStateExit();
                    }
                }
                // Flicker 하지 않는 타일 선택
                else
                {
                    SkillStateExit();
                }
            }
            yield return null;
        }
    }


    public void SkillStateExit()
    { 
        if (_inSkillState) _inSkillState = false;
        _currentSkill = -1;

        if (_skillParameterPoints.Count == 0)
            return;

        foreach (CivModel.Terrain.Point pnt in _skillParameterPoints)
        {
            CivModel.Position pos = pnt.Position;
            GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
        }
        _skillParameterPoints.Clear();
    }
}