using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using CivModel.Common;

public class PseudoFSM : MonoBehaviour {
    private static PseudoFSM _fsm = null;
    public static PseudoFSM I { get { return _fsm; } }

    // For pseudo-FSM
    private bool _inMoveState = false;
    public bool MoveState { get { return _inMoveState; } }
    private bool _inAttackState = false;
    public bool AttackState { get { return _inAttackState; } }
    private bool _inSkillState = false;
    public bool SkillState { get { return _inSkillState; } }
    private bool _inDepState = false;
    public bool DepState { get { return _inDepState; } }
    private int _currentSkill = -1;
    private Production _deployment;
    public Production Deployment { get { return _deployment; } }

    private CivModel.Terrain.Point?[] _parameterPoints;

    // Use this for initialization
    void Awake () {
        // Singleton
        if (_fsm != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _fsm = this;
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // For state change of pseudo-FSM
    // There are enter, exit methods for move and attack states. Enter methods are public, exit methods are default.
    // NormalStateEnter() simply resets all state condition.
    public void NormalStateEnter()
    {
        if (_inMoveState) MoveStateExit();
        if (_inAttackState) AttackStateExit();
        if (_inSkillState) SkillStateExit();
        if (_inDepState) DepStateExit();
    }

    // When move state, coloring movable adjacent tiles
    // Current MoveStateEnter() shows only adjacent tiles. If moving mechanism of model changes, this should be changed.
    public void MoveStateEnter()
    {
        // State change
        if (_inMoveState) return;
        if (_inAttackState) AttackStateExit();
        if (_inSkillState) SkillStateExit();
        if (_inDepState) DepStateExit();
        _inMoveState = true;

        // Select movable adjacent tiles
        _parameterPoints = GameManager.I.SelectedActor.PlacedPoint.Value.Adjacents();
        for (int i = 0; i < _parameterPoints.Length; i++)
        {
            if (GameManager.I.SelectedActor.MoveAct.IsActable(_parameterPoints[i]))
            {
                CivModel.Position pos = _parameterPoints[i].Value.Position;
                GameManager.I.Cells[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
            }
            else if (GameManager.I.SelectedActor.MovingAttackAct.IsActable(_parameterPoints[i]))
            {
                CivModel.Position pos = _parameterPoints[i].Value.Position;
                GameManager.I.Cells[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
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
                GameManager.I.Cells[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
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
        if (_inDepState) DepStateExit();
        _inAttackState = true;

        if (GameManager.I.SelectedActor == null)
            return;
        // If GameManager.I.SelectedActor cannot attack
        if (GameManager.I.SelectedActor.MovingAttackAct == null)
            return;

        // Select attackable adjacent tiles
        _parameterPoints = GameManager.I.SelectedActor.PlacedPoint.Value.Adjacents();
        for (int i = 0; i < _parameterPoints.Length; i++)
        {
            if (GameManager.I.SelectedActor.MovingAttackAct.IsActable(_parameterPoints[i]))
            {
                CivModel.Position pos = _parameterPoints[i].Value.Position;
                GameManager.I.Cells[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
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
                GameManager.I.Cells[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
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
        if (_inDepState) DepStateExit();
        _inSkillState = true;
        _currentSkill = index;

        // If SpecialActs[_currentSkill] is not parametered skill, this skill is immediately activated.
        if (!GameManager.I.SelectedActor.SpecialActs[_currentSkill].IsParametered)
        {
            GameManager.I.SelectedActor.SpecialActs[_currentSkill].Act(null);
            SkillStateExit();
            return;
        }
        else
        {
            for (int i = 0; i < GameInfo.mapWidth; i++)
            {
                for (int j = 0; j < GameInfo.mapHeight; j++)
                {
                    CivModel.Terrain.Point? point = GameManager.I.Game.Terrain.GetPoint(i, j);
                    if (GameManager.I.SelectedActor.SpecialActs[_currentSkill].IsActable(point))
                    {
                        CivModel.Position pos = point.Value.Position;
                        GameManager.I.Cells[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
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
        if (GameManager.I.SelectedActor == null || !GameManager.I.SelectedActor.SpecialActs[index].IsParametered)
        {
            return;
        }
        else
        {
            for (int i = 0; i < GameInfo.mapWidth; i++)
            {
                for (int j = 0; j < GameInfo.mapHeight; j++)
                {
                    CivModel.Terrain.Point? point = GameManager.I.Game.Terrain.GetPoint(i, j);
                    if (GameManager.I.SelectedActor.SpecialActs[index].IsActable(point))
                    {
                        CivModel.Position pos = point.Value.Position;
                        GameManager.I.Cells[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
                    }
                }
            }
        }
    }
    public void DepStateEnter(Production dep)
    {
        // State change
        if (dep == null || _inDepState) return;
        if (_inMoveState) MoveStateExit();
        if (_inAttackState) AttackStateExit();
        if (_inSkillState) SkillStateExit();
        _inDepState = true;
        _deployment = dep;
        // Select deploy tile
        CivModel.Terrain terrain = GameManager.I.Game.Terrain;
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                if (dep.IsPlacable(point))
                {
                    GameManager.I.Cells[point.Position.X, point.Position.Y].GetComponent<HexTile>().FlickerBlue();
                }
            }
        }
    }
    void DepStateExit()
    {
        _inDepState = false;
        _deployment = null;
        CivModel.Terrain terrain = GameManager.I.Game.Terrain;
        for (int i = 0; i < terrain.Width; i++)
        {
            for (int j = 0; j < terrain.Height; j++)
            {
                CivModel.Terrain.Point point = terrain.GetPoint(i, j);
                GameManager.I.Cells[point.Position.X, point.Position.Y].GetComponent<HexTile>().StopFlickering();
            }
        }
    }
}
