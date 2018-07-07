using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;


public class Unit : MonoBehaviour {

	// 현재 Unit의 위치의 point class
	public CivModel.Terrain.Point point;

    // Unity에서 그려지는 벡터 포지션
    public Vector3 unityPoint;

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
        if (_inAttackState) AttackStateExit();
        if (_inSkillState) SkillStateExit();
        //if (_inDepState) DepStateExit();
    }

    // When move state, coloring movable adjacent tiles
    // Current MoveStateEnter() shows only adjacent tiles. If moving mechanism of model changes, this should be changed.
    public void MoveStateEnter()
    {
        // State change
        if (_inMoveState) return;
        if (_inAttackState) AttackStateExit();
        if (_inSkillState) SkillStateExit();
        //if (_inDepState) DepStateExit();
        _inMoveState = true;

        if (GameManager.Instance.selectedUnit == null)
        {
            return;
        }

        // Select movable adjacent tiles
        _parameterPoints = GameManager.Instance.selectedUnit.PlacedPoint.Value.Adjacents();
        for (int i = 0; i < _parameterPoints.Length; i++)
        {
            if (GameManager.Instance.selectedUnit.MovingAttackAct.IsActable(_parameterPoints[i]))
            {
                CivModel.Position pos = _parameterPoints[i].Value.Position;
                GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
            }
            else if (GameManager.Instance.selectedUnit.MoveAct.IsActable(_parameterPoints[i]))
            {
                CivModel.Position pos = _parameterPoints[i].Value.Position;
                GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
            }
        }
    }
    void MoveStateExit()
    {
        _inMoveState = false;

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

    public void AttackStateEnter()
    {
        // State change
        if (_inAttackState) return;
        if (_inMoveState) MoveStateExit();
        if (_inSkillState) SkillStateExit();
        ///if (_inDepState) DepStateExit();
        _inAttackState = true;

        if (GameManager.Instance.selectedUnit == null)
            return;
        // If GameManager.I.SelectedActor cannot attack
        if (GameManager.Instance.selectedUnit.MovingAttackAct == null)
            return;
        
        // Select attackable adjacent tiles
        _parameterPoints = GameManager.Instance.selectedUnit.PlacedPoint.Value.Adjacents();
        for (int i = 0; i < _parameterPoints.Length; i++)
        {
            if (GameManager.Instance.selectedUnit.MovingAttackAct.IsActable(_parameterPoints[i]))
            {
                CivModel.Position pos = _parameterPoints[i].Value.Position;
                GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
            }
        }
    }
    void AttackStateExit()
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

    public void SkillStateEnter(int index)
    {
        // State change
        if (_inSkillState && _currentSkill == index) return;
        if (_inMoveState) MoveStateExit();
        if (_inAttackState) AttackStateExit();
        //if (_inDepState) DepStateExit();
        _inSkillState = true;
        _currentSkill = index;

        // If SpecialActs[_currentSkill] is not parametered skill, this skill is immediately activated.
        if (!GameManager.Instance.selectedUnit.SpecialActs[_currentSkill].IsParametered)
        {
            GameManager.Instance.selectedUnit.SpecialActs[_currentSkill].Act(null);
            SkillStateExit();
            return;
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.Game.Terrain.Width; i++)
            {
                for (int j = 0; j < GameManager.Instance.Game.Terrain.Height; j++)
                {
                    CivModel.Terrain.Point? pnt = GameManager.Instance.Game.Terrain.GetPoint(i, j);
                    if (GameManager.Instance.selectedUnit.SpecialActs[_currentSkill].IsActable(pnt))
                    {
                        CivModel.Position pos = pnt.Value.Position;
                        GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
                    }
                }
            }
        }
    }
    void SkillStateExit()
    {
        int index = _currentSkill;
        _inSkillState = false;
        _currentSkill = -1;
        if (GameManager.Instance.selectedUnit == null || !GameManager.Instance.selectedUnit.SpecialActs[index].IsParametered)
        {
            return;
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.Game.Terrain.Width; i++)
            {
                for (int j = 0; j < GameManager.Instance.Game.Terrain.Height; j++)
                {
                    CivModel.Terrain.Point? pnt = GameManager.Instance.Game.Terrain.GetPoint(i, j);
                    if (GameManager.Instance.selectedUnit.SpecialActs[index].IsActable(pnt))
                    {
                        CivModel.Position pos = pnt.Value.Position;
                        GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
                    }
                }
            }
        }
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
