using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using CivModel;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    // CivModel.Terrain.Point Attributes of the Unit.
    // Associated With the Model, But as it's a pointer, Access is Required.
    public CivModel.Terrain.Point point;

    // Unity Transform Position of the Unit.
    public Vector3 unityPoint;

    // CivModel.Unit Attributes of the Unit.
    // Associated With the Model, But as it's a pointer, Access is Required.
    public CivModel.Unit unitModel;

    public int unitOwnerNumber;

    // Unit States. Move, Attack and Skill
    private bool _inMoveState = false;
    public bool MoveState { get { return _inMoveState; } }
    private bool _inAttackState = false;
    public bool AttackState { get { return _inAttackState; } }
    private bool _inSkillState = false;
    public bool SkillState { get { return _inSkillState; } }

    // Initial Skill Index is -1
    private int _currentSkill = -1;

    // ParameterPoints which are target of the Skill
    private CivModel.Terrain.Point?[] _parameterPoints;
    private List<CivModel.Terrain.Point> _skillParameterPoints = new List<CivModel.Terrain.Point>();

    // Change Unit position to given CivModel.Terrain.Point value
    // Default y position is 1.25f
    public void SetPoints(CivModel.Terrain.Point p1)
    {
        this.point = p1;
        this.unityPoint = GameManager.ModelPntToUnityPnt(p1, 0);
        this.transform.position = this.unityPoint;
        this.unitOwnerNumber = this.unitModel.Owner.PlayerNumber;
        // SetMaterial();
    }
    // Change Unit position to given CivModel.Terrain.Point value
    public void SetPoints(CivModel.Terrain.Point p1, Vector3 p2)
    {
        this.point = p1;
        this.unityPoint = new Vector3(p2.x, p2.y, p2.z);
        this.transform.position = this.unityPoint;
        // SetMaterial();
    }
    // Set Material of the Unit
    // Materials are stored in the GameManager Class of the Unity Editor
    private void SetMaterial()
    {
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
        if (_inSkillState) SkillStateExit();
    }

    // When move state, coloring movable adjacent tiles
    // Current MoveStateEnter() shows only adjacent tiles. If moving mechanism of model changes, this should be changed.
    public void MoveStateEnter()
    {
        // State change
        if (_inMoveState) return;
        if (_inAttackState) MoveStateExit();
        if (_inSkillState) SkillStateExit();
        _inMoveState = true;

        Actor selectedActor = GameManager.Instance.selectedActor;
        if (selectedActor == null)
        {
            Debug.Log("Selected Actor is NULL");
            return;
        }

        else if (selectedActor is CivModel.Unit)
        {
            var player = GameManager.Instance.Game.Players[GameInfo.UserPlayer];
            bool checkMovingAttack = false;
            // Select movable tiles
            if (selectedActor.MovingAttackAct != null)
                checkMovingAttack = true;
            _parameterPoints = CivModel.Path.ActorMovePath.GetReachablePoint(selectedActor, checkMovingAttack)
                .Where(x => x != selectedActor.PlacedPoint)
                .Select(x => (CivModel.Terrain.Point?)x).ToArray();
            for (int i = 0; i < _parameterPoints.Length; i++)
            {
                if (_parameterPoints[i].Value.Unit == null && _parameterPoints[i].Value.TileBuilding == null)
                {
                    CivModel.Position pos = _parameterPoints[i].Value.Position;
                    GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
                    GameManager.Instance.AdditionalTiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
                }
                //if the tile has an enemy unit or an enemy tilebuilding.
                else
                {
                    if (IsEnemyActor(player, _parameterPoints[i].Value) && selectedActor.MovingAttackAct != null)
                    {
                        CivModel.Position pos = _parameterPoints[i].Value.Position;
                        GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
                        GameManager.Instance.AdditionalTiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
                    }
                }
            }
            IEnumerator _coroutine = MoveUnit(selectedActor);
            StartCoroutine(_coroutine);
        }
    }

    private bool IsEnemyActor(Player player, CivModel.Terrain.Point point)
    {
        return (point.Unit != null && !point.Unit.Owner.IsAlliedWith(player))
            || (point.TileBuilding != null && !point.TileBuilding.Owner.IsAlliedWith(player));
    }

    IEnumerator MoveUnit(CivModel.Actor unitToMove)
    {
        while (true)
        {
            CivModel.Terrain.Point destPoint = GameManager.Instance.selectedPoint;
            var player = GameManager.Instance.Game.Players[GameInfo.UserPlayer];

            // 새로운 Point 을 선택했을 때
            if (unitToMove.PlacedPoint.Value != destPoint)
            {
                // Flicker하고 있는 Tile을 선택했을 때
                if (GameManager.Instance.selectedTile.isFlickering)
                {
                    IMovePath MovePath;
                    UIManager.Instance.updateSelectedInfo(unitToMove);
                    //적 Actor일때
                    if (unitToMove.MovingAttackAct != null && IsEnemyActor(player,destPoint))
                    {
                        MovePath = MoveOrMoveAttack(unitToMove, destPoint, unitToMove.MovingAttackAct);
                        //unitToMove.MovingAttackAct.Act(destPoint);
                        MoveStateExit();
                        if (MovePath != null && !MovePath.IsInvalid)
                        {
                            yield return MoveorMovingAttackAnimation(MovePath, 0.2f, unitToMove.MovingAttackAct);
                            yield return AttackAnimation(unitToMove, destPoint);
                            unitToMove.MovePath = MovePath;
                            MovePath.ActFullWalkForRemainAP();
                        }
                        UIManager.Instance.ButtonInteractChange();
                        GameManager.Instance.UpdateUnit();
                        break;
                    }
                    else if (unitToMove.MoveAct != null)
                    {
                        MovePath = MoveOrMoveAttack(unitToMove, destPoint, unitToMove.MoveAct);
                        //unitToMove.MoveAct.Act(destPoint);
                        MoveStateExit();
                        if (MovePath != null && !MovePath.IsInvalid)
                        {
                            yield return MoveorMovingAttackAnimation(MovePath, 0.2f, unitToMove.MoveAct);
                            unitToMove.MovePath = MovePath;
                            MovePath.ActFullWalkForRemainAP();
                        }
                        GameManager.Instance.selectedPoint = destPoint;
                        UIManager.Instance.ButtonInteractChange();
                        GameManager.Instance.UpdateUnit();
                        break;
                    }
                    else
                    {
                        Debug.Log("The Unit Cannot Move");
                        MoveStateExit();
                        break;
                    }
                }
                // Flicker 하지 않는 타일 선택
                else
                {
                    UIManager.Instance.updateSelectedInfo(unitToMove);
                    MoveStateExit();
                    break;
                }
            }
            yield return null;
        }
    }

    private static IMovePath MoveOrMoveAttack(Actor unitToMove, CivModel.Terrain.Point destPoint, CivModel.IActorAction finalAction)
    {
        IMovePath MovePath;
        IMovePath path = new CivModel.Path.ActorMovePath(
                  unitToMove, destPoint, finalAction);
        if (!path.IsInvalid)
        {
            MovePath = path;
        }
        else
        {
            MovePath = null;
        }
        /*
        if (MovePath != null && !MovePath.IsInvalid)
        {
            unitToMove.MovePath = MovePath;
            MovePath.ActFullWalkForRemainAP();
        }*/
        return MovePath;
    }

    IEnumerator MoveorMovingAttackAnimation(IMovePath path, float secondsPerMove, CivModel.IActorAction finalAction)
    {
        float timer = 0;
        for (int i = 1; i < path.Path.Count() - 1; i++)
        {
            Vector3 destPointsPos = GameManager.ModelPntToUnityPnt(path.Path.ElementAt(i), 0);
            float distance = (transform.position - destPointsPos).magnitude;
            Quaternion lookDir = Quaternion.LookRotation(destPointsPos - transform.position, transform.up);
            /*
            // hard coding for not-uniform-basis models
            if(path.Actor is CivModel.Unit && (CivModel.Unit)path.Actor is CivModel.Hwan.LEOSpaceArmada)
            {
                lookDir = lookDir * Quaternion.Euler(0, -43, 0);
            }*/
            Quaternion initDir = transform.rotation;
            while(true)
            {
                transform.rotation = Quaternion.Lerp(initDir, lookDir, timer);
                timer += Time.deltaTime * 3;
                if(timer > 1) break;
                yield return null;
            }
            timer = 0;
            while (timer < secondsPerMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, destPointsPos, distance * Time.deltaTime / secondsPerMove);
                timer += Time.deltaTime;
                yield return null;
            }
            timer = 0;
        }
        if(finalAction == finalAction.Owner.MovingAttackAct)
            yield return null;
        else
        {
            Vector3 destPointsPos = GameManager.ModelPntToUnityPnt(path.Path.ElementAt(path.Path.Count()-1), 0);
            float distance = (transform.position - destPointsPos).magnitude;
            Quaternion lookDir = Quaternion.LookRotation(destPointsPos - transform.position, transform.up);
            /*
            // hard coding for not-uniform-basis models
            if (path.Actor is CivModel.Unit && (CivModel.Unit)path.Actor is CivModel.Hwan.LEOSpaceArmada)
            {
                lookDir = lookDir * Quaternion.Euler(0, -43, 0);
            }*/
            Quaternion initDir = transform.rotation;
            while (true)
            {
                transform.rotation = Quaternion.Lerp(initDir, lookDir, timer);
                timer += Time.deltaTime * 3;
                if (timer > 1) break;
                yield return null;
            }
            timer = 0;
            while (timer < secondsPerMove)
            {
                transform.position = Vector3.MoveTowards(transform.position, destPointsPos, distance * Time.deltaTime / secondsPerMove);
                timer += Time.deltaTime;
                yield return null;
            }
            timer = 0;
        }
    }

    public void MoveStateExit()
    {
        if (_inMoveState) _inMoveState = false;
        else if (_inAttackState) _inAttackState = false;

        if (_parameterPoints == null)
            return;

        for (int i = 0; i < _parameterPoints.Length; i++)
        {
            if (_parameterPoints[i] != null)
            {
                CivModel.Position pos = _parameterPoints[i].Value.Position;
                GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
                GameManager.Instance.AdditionalTiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
            }
        }
        _parameterPoints = null;
        UIManager.Instance.ButtonInteractChange();
    }

    public void AttackStateEnter()
    {
        // State change
        if (_inAttackState) return;
        if (_inMoveState) MoveStateExit();
        if (_inSkillState) SkillStateExit();
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
                    GameManager.Instance.AdditionalTiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerRed();
                    //Debug.Log(pos.X + " " + pos.Y + " Filcker Red");
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

    IEnumerator AttackUnit(CivModel.Actor unitToAttack)
    {
        while (true)
        {
            CivModel.Terrain.Point destPoint = GameManager.Instance.selectedPoint;
            // 새로운 Point 을 선택했을 때
            if (unitToAttack.PlacedPoint.Value != destPoint)
            {
                // Flicker하고 있는 Tile을 선택했을 때
                if (GameManager.Instance.selectedTile.isFlickering)
                {
                    UIManager.Instance.updateSelectedInfo(unitToAttack);
                    if (unitToAttack.HoldingAttackAct != null && unitToAttack.HoldingAttackAct.IsActable(destPoint))
                    {
                        yield return AttackAnimation(unitToAttack, destPoint);
                        unitToAttack.HoldingAttackAct.Act(destPoint);
                        MoveStateExit();
                        GameManager.Instance.UpdateUnit();
                        break;
                    }
                }
                // Flicker 하지 않는 타일 선택
                else
                {
                    UIManager.Instance.updateSelectedInfo(unitToAttack);
                    MoveStateExit();
                    break;
                }
            }
            yield return null;
        }
    }

    IEnumerator AttackAnimation(CivModel.Actor unitToAttack, CivModel.Terrain.Point unitTarget)
    {
        float timer = 0;
        Vector3 attackUnitPos = transform.position;
        Vector3 targetUnitPos = GameManager.ModelPntToUnityPnt(unitTarget, 0);
        float distance = (attackUnitPos + new Vector3(0, 2, 0) - targetUnitPos).magnitude;
        /*
        // move up
        while (timer < 0.4f)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackUnitPos + new Vector3(0, 2, 0), 2 * Time.deltaTime / 0.4f);
            timer += Time.deltaTime;
            yield return null;
        }
        // hit
        while (timer < 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetUnitPos, distance * Time.deltaTime / 0.1f);
            timer += Time.deltaTime;
            yield return null;
        }
        // hit back
        while (timer < 0.7f)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackUnitPos + new Vector3(0, 2, 0), distance * Time.deltaTime / 0.2f);
            timer += Time.deltaTime;
            yield return null;
        }
        // move down
        while (timer < 1.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackUnitPos, distance * Time.deltaTime / 0.4f);
            timer += Time.deltaTime;
            yield return null;
        }*/
        yield return null;
    }

    public void SkillStateEnter(int index)
    {
        // State change
        if (_inSkillState && _currentSkill == index) return;
        if (_inMoveState) MoveStateExit();
        if (_inAttackState) MoveStateExit();
        _inSkillState = true;
        _currentSkill = index;


        // If SpecialActs[_currentSkill] is not parametered skill, this skill is immediately activated.
        if (!GameManager.Instance.selectedActor.SpecialActs[_currentSkill].IsParametered)
        {
            if (GameManager.Instance.selectedActor.SpecialActs[_currentSkill].IsActable(null))
            {
                //if spy
                if (unitModel is CivModel.Hwan.Spy || unitModel is CivModel.Finno.Spy)
                {
                    CivModel.Player SpiedPlayer = unitModel.PlacedPoint.Value.TileOwner;

                    // 플레이어 이름
                    string playerName = SpiedPlayer.PlayerName;
                    // 금 정보
                    double gold = SpiedPlayer.Gold;
                    double goldTurn = SpiedPlayer.GoldIncome;
                    // 인구 정보
                    double population = SpiedPlayer.Population;
                    // 행복도 정보
                    double happiness = SpiedPlayer.Happiness;
                    double happinessTurn = SpiedPlayer.HappinessIncome;
                    // 기술력 정보
                    double research = SpiedPlayer.Research;
                    double researchTurn = SpiedPlayer.ResearchIncome;
                    // 노동력 정보
                    double labor = SpiedPlayer.Labor;

                    string text = "적국 이름: " + playerName + "\n\n금: " + gold + " (턴당 " + goldTurn + ")\n\n" + "인구: " + population + "\n\n" + "행복: " + happiness + " (턴당 " + happinessTurn + ")\n\n" + "기술력: " + research + "(턴당 " + researchTurn + ")\n\n" + "노동력: " + labor;

                    UIManager.Instance.spyPanel.SetActive(true);
                    UIManager.Instance.spyContent.GetComponent<Text>().text = text;
                }
                // 공통적인 부분
                GameManager.Instance.selectedActor.SpecialActs[_currentSkill].Act(null);
                GameManager.Instance.UpdateUnit();
                // UIManager.Instance.UpdateUnitInfo(); Done in UpdateUnit
            }
            else
            {
                Debug.Log("아직 사용할 수 없습니다");
                AlarmManager.Instance.AddAlarm(UIManager.Instance.UnitPortrait.sprite, "스킬을 사용할 수 없습니다", () => GameManager.Focus(point), 0);
            }
            _inSkillState = false;
            return;
        }
        else
        {
            _skillParameterPoints.Clear();
            for (int i = 0; i < GameManager.Instance.Game.Terrain.Width; i++)
            {
                for (int j = 0; j < GameManager.Instance.Game.Terrain.Height; j++)
                {
                    CivModel.Terrain.Point? pnt = GameManager.Instance.Game.Terrain.GetPoint(i, j);

                    if (GameManager.Instance.selectedActor.SpecialActs[_currentSkill].IsActable(pnt))
                    {
                        CivModel.Position pos = pnt.Value.Position;
                        // Debug.Log("Skill @ ( " + pos.X + " ," + pos.Y + " )");
                        GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
                        GameManager.Instance.AdditionalTiles[pos.X, pos.Y].GetComponent<HexTile>().FlickerBlue();
                        _skillParameterPoints.Add(pnt.Value);
                    }
                }
            }
            if (_skillParameterPoints.Count != 0)
            {
                IEnumerator _coroutine = SkillUnit(GameManager.Instance.selectedActor);
                var _tile = GameManager.Instance.selectedTile;
                var _gameObject = GameManager.Instance.selectedGameObject;
                StartCoroutine(_coroutine);
            }
            else
            {
                Debug.Log("아직 사용할 수 없습니다");
                AlarmManager.Instance.AddAlarm(UIManager.Instance.UnitPortrait.sprite, "스킬을 사용할 수 없습니다", () => GameManager.Focus(point), 0);
                SkillStateExit();
            }
        }
    }

    IEnumerator SkillUnit(CivModel.Actor unitToSkill)
    {
        while (true)
        {
            // 새로운 Point 을 선택했을 때
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                // Flicker하고 있는 Tile을 선택했을 때
                if (GameManager.Instance.selectedTile.isFlickering)
                {
                    CivModel.Terrain.Point destPoint = GameManager.Instance.selectedPoint;
                    UIManager.Instance.updateSelectedInfo(unitToSkill);
                    if (unitToSkill.SpecialActs[_currentSkill].IsActable(destPoint))
                    {
                        try
                        {
                            unitToSkill.SpecialActs[_currentSkill].Act(destPoint);

                        }
                        catch (System.Exception e) { Debug.Log(e); }
                        finally
                        {
                            SkillStateExit();
                            GameManager.Instance.UpdateUnit();
                        }
                        break;
                    }
                    else
                    {
                        SkillStateExit();
                        break;
                    }
                }
                // Flicker 하지 않는 타일 선택
                else
                {
                    UIManager.Instance.updateSelectedInfo(unitToSkill);
                    SkillStateExit();
                    break;
                }
            }
            yield return null;
        }
    }


    public void SkillStateExit()
    {
        if (_inSkillState) _inSkillState = false;
        _currentSkill = -1;

        foreach (CivModel.Terrain.Point pnt in _skillParameterPoints)
        {
            CivModel.Position pos = pnt.Position;
            GameManager.Instance.Tiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
            GameManager.Instance.AdditionalTiles[pos.X, pos.Y].GetComponent<HexTile>().StopFlickering();
        }
        _skillParameterPoints.Clear();
        UIManager.Instance.ButtonInteractChange();
    }
}
