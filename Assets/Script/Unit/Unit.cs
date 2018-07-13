using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;


public class Unit : MonoBehaviour {

	// 현재 Unit의 위치의 point class
	public CivModel.Terrain.Point point;

    // Unity에서 그려지는 벡터 포지션
    public Vector3 unityPoint;

    // 현재 Unit이 가지는 Unit class
    public CivModel.Unit unitModel;

    private bool _inMoveState = false;
    public bool MoveState { get { return _inMoveState; } }
    private bool _inAttackState = false;
    public bool AttackState { get { return _inAttackState; } }
    private bool _inSkillState = false;
    public bool SkillState { get { return _inSkillState; } }
    //private bool _inDepState = false;
    //public bool DepState { get { return _inDepState; } }
    private int _currentSkill = -1;

    private CivModel.Terrain.Point?[] _parameterPoints;
	private List<CivModel.Terrain.Point?> _skillParameterPoints = new List<CivModel.Terrain.Point?>();

	// Use this for initialization
	void Start() {
	}

    public void SetPoints(CivModel.Terrain.Point p1) {
        this.point = p1;
        this.unityPoint = GameManager.ModelPntToUnityPnt(p1, 1.25f);
        this.transform.position = this.unityPoint;
        SetMaterial();
    }

    public void SetPoints(CivModel.Terrain.Point p1, Vector3 p2)
    {
        this.point = p1;
        this.unityPoint = new Vector3(p2.x, p2.y, p2.z);
        this.transform.position = this.unityPoint;
        SetMaterial();

    }
    private void SetMaterial() {
        foreach (Material m in GameManager.Instance.materials)
        {
            if (m == GameManager.Instance.materials[(int)UnitEnum.UnitToEnum(point.Unit)])
            {
                GetComponent<Renderer>().material = m;
            }
        }
    }

    // There are enter, exit methods for move and attack states. Enter methods are public, exit methods are default.
    // NormalStateEnter() simply resets all state condition.
    public void NormalStateEnter()
    {
        if (_inMoveState) MoveStateExit();
        if (_inAttackState) MoveStateExit();
        if (_inSkillState) SkillStateExit(GameManager.Instance.selectedActor);
        //if (_inDepState) DepStateExit();
    }

    // When move state, coloring movable adjacent tiles
    // Current MoveStateEnter() shows only adjacent tiles. If moving mechanism of model changes, this should be changed.
    public void MoveStateEnter()
    {
        // State change
        if (_inMoveState) return;
        if (_inAttackState) MoveStateExit();
        if (_inSkillState) SkillStateExit(GameManager.Instance.selectedActor);
        //if (_inDepState) DepStateExit();
        _inMoveState = true;

        if (GameManager.Instance.selectedActor == null)
        {
            Debug.Log("Selected Actor is NULL");
            return;
        }

        else if (GameManager.Instance.selectedActor is CivModel.Unit)
        {
            // Select movable adjacent tiles
            _parameterPoints = GameManager.Instance.selectedActor.PlacedPoint.Value.Adjacents();
            for (int i = 0; i < _parameterPoints.Length; i++)
            {
                if (GameManager.Instance.selectedActor.MovingAttackAct != null && GameManager.Instance.selectedActor.MovingAttackAct.IsActable(_parameterPoints[i]))
                {
                    CivModel.Position pos = _parameterPoints[i].Value.Position;
                    GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
                }
                else if (GameManager.Instance.selectedActor.MoveAct != null && GameManager.Instance.selectedActor.MoveAct.IsActable(_parameterPoints[i]))
                {
                    CivModel.Position pos = _parameterPoints[i].Value.Position;
                    GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
                }
                else
                {
                    CivModel.Position pos = _parameterPoints[i].Value.Position;
                    //Debug.Log("Cannot Move to (" + pos.X + ", " + pos.Y + ")"); 
                }
            }
            IEnumerator _coroutine = MoveUnit(GameManager.Instance.selectedActor);
            StartCoroutine(_coroutine);
        }
    }

    public void AttackStateEnter()
    {
        // State change
        if (_inAttackState) return;
        if (_inMoveState) MoveStateExit();
        if (_inSkillState) SkillStateExit(GameManager.Instance.selectedActor);
        ///if (_inDepState) DepStateExit();
        _inAttackState = true;

        if (GameManager.Instance.selectedActor == null)
        {
            Debug.Log("Selected Actor is NULL");
            return;
        }
        // If GameManager.Instance.SelectedActor cannot attack
        if (GameManager.Instance.selectedActor.HoldingAttackAct == null)
        {
            Debug.Log("Selected Actor can not act holding attack");
            return;
        }

        else if (GameManager.Instance.selectedActor is CivModel.Unit)
        {
            // Select movable adjacent tiles
            _parameterPoints = GameManager.Instance.selectedActor.PlacedPoint.Value.Adjacents();
            for (int i = 0; i < _parameterPoints.Length; i++)
            {
                if (GameManager.Instance.selectedActor.HoldingAttackAct.IsActable(_parameterPoints[i]))
                {
                    CivModel.Position pos = _parameterPoints[i].Value.Position;
                    GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
                    Debug.Log(pos.X + " " + pos.Y + " Filcker Red");
                }
                else
                {
                    CivModel.Position pos = _parameterPoints[i].Value.Position;
                    //Debug.Log("Cannot Attack to (" + pos.X + ", " + pos.Y + ")");
                }
            }
            IEnumerator _coroutine = AttackUnit(GameManager.Instance.selectedActor);
            StartCoroutine(_coroutine);
        }
    }

    IEnumerator AttackUnit(CivModel.Actor unitToMove)
    {
        while (true)
        {
            CivModel.Terrain.Point destPoint = GameManager.Instance.selectedPoint;
            // 새로운 Point 을 선택했을 때
            if (unitToMove.PlacedPoint.Value != destPoint)
            {
                // Flicker하고 있는 Tile을 선택했을 때
                if (GameManager.Instance.selectedTile.isFlickering)
                {
                    if (unitToMove.HoldingAttackAct != null && unitToMove.HoldingAttackAct.IsActable(destPoint))
                    {
                        unitToMove.HoldingAttackAct.Act(destPoint);
                        AttackStateExit();
                        GameManager.Instance.UpdateUnit();
                        break;
                    }
                }
                // Flicker 하지 않는 타일 선택
                else
                {
                    AttackStateExit();
                }
            }
            yield return null;
        }
    }

    public void AttackStateExit()
    {
        _inAttackState = false;

        if (_parameterPoints == null)
            return;

        for (int i = 0; i < _parameterPoints.Length; i++)
        {
            if (_parameterPoints[i] != null)
            {
                CivModel.Position pos = _parameterPoints[i].Value.Position;
                GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
            }
        }
        _parameterPoints = null;
    }

    IEnumerator MoveUnit(CivModel.Actor unitToMove) {
		while (true) {
			CivModel.Terrain.Point destPoint = GameManager.Instance.selectedPoint;
			// 새로운 Point 을 선택했을 때
			if (unitToMove.PlacedPoint.Value != destPoint) {
				// Flicker하고 있는 Tile을 선택했을 때
				if (GameManager.Instance.selectedTile.isFlickering) {
					if (unitToMove.MovingAttackAct != null && unitToMove.MovingAttackAct.IsActable(destPoint)) {
						unitToMove.MovingAttackAct.Act(destPoint);
                        MoveStateExit();
                        GameManager.Instance.UpdateUnit();
						break;
					}
                    else if (unitToMove.MoveAct != null && unitToMove.MoveAct.IsActable(destPoint)){
						unitToMove.MoveAct.Act(destPoint);
                        MoveStateExit();
                        GameManager.Instance.UpdateUnit();
						break;
					}
                    else {
                        Debug.Log("The Unit Cannot Move");
                        MoveStateExit();
                        break;
                    }
				}
				// Flicker 하지 않는 타일 선택
				else {
					MoveStateExit();
				}
			}
			yield return null;
        }
    }

	public void MoveStateExit() {
		if (_inMoveState) _inMoveState = false;
		else if (_inAttackState) _inAttackState = false;

		if (_parameterPoints == null)
			return;

		for (int i = 0; i < _parameterPoints.Length; i++) {
			if (_parameterPoints[i] != null) {
				CivModel.Position pos = _parameterPoints[i].Value.Position;
				GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
			}
		}
		_parameterPoints = null;
	}

	public void SkillStateEnter(int index) {
        // State change
        if (_inSkillState && _currentSkill == index) return;
        if (_inMoveState) MoveStateExit();
        if (_inAttackState) MoveStateExit();
        //if (_inDepState) DepStateExit();
        _inSkillState = true;
        _currentSkill = index;

        // If SpecialActs[_currentSkill] is not parametered skill, this skill is immediately activated.
        if (!GameManager.Instance.selectedActor.SpecialActs[_currentSkill].IsParametered) {
            GameManager.Instance.selectedActor.SpecialActs[_currentSkill].Act(null);
            GameManager.Instance.UpdateUnit();
            return;
        }
        else {
            for (int i = 0; i < GameManager.Instance.Game.Terrain.Width; i++) {
                for (int j = 0; j < GameManager.Instance.Game.Terrain.Height; j++) {
                    CivModel.Terrain.Point? pnt = GameManager.Instance.Game.Terrain.GetPoint(i, j);
                    if (GameManager.Instance.selectedActor.SpecialActs[_currentSkill].IsActable(pnt)) {
                        CivModel.Position pos = pnt.Value.Position;
                        GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
						_skillParameterPoints.Add(pnt);
                    }
                }
            }
            IEnumerator _coroutine = SkillUnit(GameManager.Instance.selectedActor);
            StartCoroutine(_coroutine);
        }
    }

    IEnumerator SkillUnit(CivModel.Actor unitToSkill) {
        while (true) {
            CivModel.Terrain.Point destPoint = GameManager.Instance.selectedPoint;
            // 새로운 Point 을 선택했을 때
            if (unitToSkill.PlacedPoint.Value != destPoint)
            {
                // Flicker하고 있는 Tile을 선택했을 때
                if (GameManager.Instance.selectedTile.isFlickering)
                {
                    if (unitToSkill.SpecialActs[_currentSkill].IsActable(destPoint)) {
                        unitToSkill.SpecialActs[_currentSkill].Act(destPoint);
                        SkillStateExit(unitToSkill);
                        GameManager.Instance.UpdateUnit();
                        break;
                    }
					else {
						SkillStateExit(unitToSkill);
					}
                }
                // Flicker 하지 않는 타일 선택
                else
                {
                    SkillStateExit(unitToSkill);
                }
            }
            yield return null;
        }
    }


    public void SkillStateExit(CivModel.Actor unitToSkill) {
		foreach (CivModel.Terrain.Point pnt in _skillParameterPoints) {
			CivModel.Position pos = pnt.Position;
			GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
		}
        _skillParameterPoints.Clear();
    }

    //public void DepStateEnter(Production dep)
    //{
    //    // State change
    //    if (dep == null || _inDepState) return;
    //    if (_inMoveState) MoveStateExit();
    //    if (_inAttackState) AttackStateExit();
    //    if (_inSkillState) SkillStateExit();
    //    _inDepState = true;
    //    _deployment = dep;
    //    // Select deploy tile
    //    CivModel.Terrain terrain = GameManager.I.Game.Terrain;
    //    for (int i = 0; i < terrain.Width; i++)
    //    {
    //        for (int j = 0; j < terrain.Height; j++)
    //        {
    //            CivModel.Terrain.Point point = terrain.GetPoint(i, j);
    //            if (dep.IsPlacable(point))
    //            {
    //                GameManager.I.Cells[point.Position.X, point.Position.Y].GetComponent<HexTile>().FlickerBlue();
    //            }
    //        }
    //    }
    //}
    //void DepStateExit()
    //{
    //    _inDepState = false;
    //    _deployment = null;
    //    CivModel.Terrain terrain = GameManager.I.Game.Terrain;
    //    for (int i = 0; i < terrain.Width; i++)
    //    {
    //        for (int j = 0; j < terrain.Height; j++)
    //        {
    //            CivModel.Terrain.Point point = terrain.GetPoint(i, j);
    //            GameManager.I.Cells[point.Position.X, point.Position.Y].GetComponent<HexTile>().StopFlickering();
    //        }
    //    }
    //}


}
