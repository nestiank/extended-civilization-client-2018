using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CivModel;

public static class UnitAnimation
{
    static readonly string particleResourcePath = "Particles/";

    static List<GameObject> HwanSoldierBulletPool;
    static List<GameObject> FinnoSoldierBulletPool;
    static List<GameObject> TurtleshipFireballPool;
    static List<GameObject> FinnoDroneBulletPool;

    //정적 배열<오락객체> 거북선불공못 = 새로운 배열<오락객체>();

    public static void AnimationParticleObjectPool()
    {
        HwanSoldierBulletPool = new List<GameObject>();
        FinnoSoldierBulletPool = new List<GameObject>();
        TurtleshipFireballPool = new List<GameObject>();
        FinnoDroneBulletPool = new List<GameObject>();

        for (int i = 0; i < 6; i++)
        {
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>
            (particleResourcePath + "HwanSoldierBullet"));
            obj.SetActive(false);
            HwanSoldierBulletPool.Add(obj);

            obj = Object.Instantiate(Resources.Load<GameObject>
                (particleResourcePath + "FinnoSoldierBullet"));
            obj.SetActive(false);
            FinnoSoldierBulletPool.Add(obj);
        }

        for(int i = 0; i < 3; i++)
        {
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>
            (particleResourcePath + "TurtleshipFireball"));
            obj.SetActive(false);
            TurtleshipFireballPool.Add(obj);
        }
        
        for(int i = 0; i < 6; i++)
        {
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>
           (particleResourcePath + "FinnoDroneBullet"));
            obj.SetActive(false);
            FinnoDroneBulletPool.Add(obj);
        }  
    }

    static GameObject GetPoolObj(List<GameObject> pool)
    {
        for(int i = 0; i < pool.Count(); i++)
        {
            if (!pool[i].activeInHierarchy) return pool[i];
        }
        return null;
    }

    static GameObject GetPoolObj(List<GameObject> pool, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < pool.Count(); i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].transform.position = position;
                pool[i].transform.localRotation = rotation; 
                return pool[i];
            }
        }
        return null;
    }


    /*
     * rotate unit to destination 
     * fullRotateSeconds : Time for full rotation (180 degrees) 
     */

    static IEnumerator LookRotate(Unit unit, Vector3 destination, float fullRotateSeconds)
    {
        float timer = 0;
        Vector3 look = destination - unit.transform.position;
        look.y = 0;
        Quaternion lookDir = Quaternion.LookRotation(look, unit.transform.up);
        Quaternion initDir = unit.transform.rotation;
        float angle = Quaternion.Angle(lookDir, initDir);
        timer += (1 - angle / 180);

        while (timer <= 1)
        {
            float interpolation = (timer - 1 + angle / 180) * (180 / angle); 
            unit.transform.rotation = Quaternion.Lerp(initDir, lookDir, timer);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    static IEnumerator FireProjectile(Unit unit, List<GameObject> pool, CivModel.Terrain.Point targetPoint, float projectileSpeed)
    {
        float timer = 0;
        GameObject projectile = GetPoolObj(pool, unit.transform.position + new Vector3(0, 0.6f, 0),
            unit.transform.localRotation);
        Projectile prj = projectile.GetComponent<Projectile>();
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.6f);
        projectile.SetActive(true);
        prj.StartFire();

        while (timer < 3)
        {
            if (prj.hasCollided) break;
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position,
                destination, projectileSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            
            yield return null;
        }
    }

    static IEnumerator FireProjectiles(Unit unit, List<GameObject> pool, int fireCount, CivModel.Terrain.Point targetPoint, float projectileSpeed, float firePerSeconds)
    {
        float timer = 0;
        float firedelay = 1 / firePerSeconds;
        Projectile[] projectiles = new Projectile[fireCount];
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.6f);
        while (timer < 3)
        {
            if (projectiles[fireCount - 1]!= null && projectiles[fireCount - 1].hasCollided)
                break;
            for (int i = 0; i < fireCount; i++)
            {
                if (projectiles[i] == null && timer > firedelay * i)
                {
                    projectiles[i] = GetPoolObj(pool, unit.transform.position + new Vector3(0, 0.6f, 0),
                        unit.transform.localRotation).GetComponent<Projectile>();
                    projectiles[i].gameObject.SetActive(true);
                    projectiles[i].StartFire();
                }

                if (projectiles[i] != null && projectiles[i].hasFired && !projectiles[i].hasCollided)
                    projectiles[i].transform.position = Vector3.MoveTowards(projectiles[i].transform.position,
                        destination, projectileSpeed * Time.deltaTime);
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    static IEnumerator Charge(Unit unit, CivModel.Terrain.Point targetPoint, float chargeSpeed)
    {
        float timer = 0;
        Vector3 initPos = unit.transform.position;
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0);
        KeyValuePair<Unit, Unit> units = GameManager.Instance.UnitDic[targetPoint.Unit];
        Unit targetUnit = units.Key;
        Unit addi_targetUnit = units.Value;
        Quaternion initDir = unit.transform.localRotation;
        Quaternion readyDir = Quaternion.AngleAxis(-40f, unit.transform.right) * unit.transform.localRotation;
        Quaternion targetInitDir = targetUnit.transform.localRotation;
        Quaternion addi_targetInitDir = addi_targetUnit.transform.localRotation;
        Quaternion fallDir = Quaternion.FromToRotation(targetUnit.transform.up,
             (targetUnit.transform.position - unit.transform.position));

        while (timer < 1f)
        {
            if (Quaternion.Angle(unit.transform.localRotation, readyDir) < 1) break;
            unit.transform.localRotation = Quaternion.RotateTowards(unit.transform.localRotation,
                readyDir, 150 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 1f)
        {
            if (Quaternion.Angle(unit.transform.localRotation, initDir) < 1) break;
            unit.transform.localRotation = Quaternion.RotateTowards(unit.transform.localRotation,
                initDir, 100 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        yield return new WaitForSeconds(0.2f);
        while (timer < 1f)
        {
            if (Vector3.Distance(unit.transform.position, destination) < 0.1f)
                break;
            
            else unit.transform.position = Vector3.MoveTowards(unit.transform.position, 
                destination, chargeSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 1f)
        {
            if (Quaternion.Angle(targetUnit.transform.localRotation, fallDir) < 1) break;
            targetUnit.transform.localRotation = Quaternion.RotateTowards(targetUnit.transform.localRotation,
                fallDir, 180 * Time.deltaTime);
            addi_targetUnit.transform.localRotation = Quaternion.RotateTowards(addi_targetUnit.transform.localRotation,
                fallDir, 180 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 2f)
        {
            if (Quaternion.Angle(targetUnit.transform.localRotation, targetInitDir) < 1)
                break;
            else
            {
                targetUnit.transform.localRotation = Quaternion.RotateTowards(targetUnit.transform.localRotation,
                    targetInitDir, 100 * Time.deltaTime);
                addi_targetUnit.transform.localRotation = Quaternion.RotateTowards(addi_targetUnit.transform.localRotation,
                    addi_targetInitDir, 100 * Time.deltaTime);
                unit.transform.position = Vector3.MoveTowards(unit.transform.position, initPos, chargeSpeed / 3 * Time.deltaTime);
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

    static IEnumerator JackieCharge(Unit unit, CivModel.Terrain.Point targetPoint, float chargeSpeed)
    {
        float timer = 0;
        float speed = 0;
        Vector3 initPos = unit.transform.position;
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0);
        KeyValuePair<Unit, Unit> units = GameManager.Instance.UnitDic[targetPoint.Unit];
        Unit targetUnit = units.Key;
        Unit addi_targetUnit = units.Value;
        Quaternion initDir = unit.transform.localRotation;
        Quaternion targetInitDir = targetUnit.transform.localRotation;
        Quaternion addi_targetInitDir = addi_targetUnit.transform.localRotation;
        Quaternion fallDir = Quaternion.FromToRotation(targetUnit.transform.up,
             (targetUnit.transform.position - unit.transform.position));

        while(timer < 1f)
        {
            unit.transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime, unit.transform.up);
            speed += Time.deltaTime * 5780;
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 1f)
        {
            unit.transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime, unit.transform.up);

            if (Vector3.Distance(unit.transform.position, destination) < 0.1f)
                break;

            else unit.transform.position = Vector3.MoveTowards(unit.transform.position,
                destination, chargeSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 1f)
        {
            unit.transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime, unit.transform.up);
            if (Quaternion.Angle(targetUnit.transform.localRotation, fallDir) < 1) break;
            targetUnit.transform.localRotation = Quaternion.RotateTowards(targetUnit.transform.localRotation,
                fallDir, 180 * Time.deltaTime);
            addi_targetUnit.transform.localRotation = Quaternion.RotateTowards(addi_targetUnit.transform.localRotation,
                fallDir, 180 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 2f)
        {
            unit.transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime, unit.transform.up);
            speed -= Time.deltaTime * 2890;
            if (Quaternion.Angle(targetUnit.transform.localRotation, targetInitDir) < 1)
                break;
            else
            {
                targetUnit.transform.localRotation = Quaternion.RotateTowards(targetUnit.transform.localRotation,
                    targetInitDir, 100 * Time.deltaTime);
                addi_targetUnit.transform.localRotation = Quaternion.RotateTowards(addi_targetUnit.transform.localRotation,
                    addi_targetInitDir, 100 * Time.deltaTime);
                unit.transform.position = Vector3.MoveTowards(unit.transform.position, initPos, chargeSpeed / 3 * Time.deltaTime);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        unit.transform.localRotation = initDir;
    }

    static IEnumerator Fly(Unit unit, CivModel.Terrain.Point targetPoint, float flySpeed, float angle)
    {
        float timer = 0;
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0);
        float distance = Vector3.Distance(unit.transform.position, destination);
        Vector3 middle = unit.transform.position + (destination - unit.transform.position).normalized * distance / 2
            + new Vector3(0, distance / 2 * Mathf.Tan(angle * Mathf.Deg2Rad), 0);
        Quaternion initDir = unit.transform.localRotation;
        Quaternion readyDir = Quaternion.AngleAxis(-angle, unit.transform.right) * unit.transform.localRotation;
        while (timer < 1f)
        {
            if (Quaternion.Angle(unit.transform.localRotation, readyDir) < 1) break;
            unit.transform.localRotation = Quaternion.RotateTowards(unit.transform.localRotation,
                readyDir, 150 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 2.5f)
        {
            if (Vector3.Distance(unit.transform.position, middle) < 0.1f)
                break;

            else unit.transform.position = Vector3.MoveTowards(unit.transform.position,
                middle, flySpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (timer < 2.5f)
        {
            if (Vector3.Distance(unit.transform.position, destination) < 0.1f &&
                Quaternion.Angle(unit.transform.localRotation, initDir) < 1)
                break;

            else
            {
                unit.transform.position = Vector3.MoveTowards(unit.transform.position,
                    destination, flySpeed * Time.deltaTime);
                unit.transform.localRotation = Quaternion.RotateTowards(unit.transform.localRotation,
                    initDir, 120 * Time.deltaTime);
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }


    /*
     * Default Method for move animation.
     * 
     * unit : unit script of moving Actor
     * path : IMovePath of moving act
     * secondsPerMove : seconds per move animation
     * finalAction : checks final action of unit - MovingAttackAct or MovingAct
     */
    public static IEnumerator MoveorMovingAttackAnimation(Unit unit, IMovePath path, 
        float secondsPerMove, CivModel.IActorAction finalAction)
    {
        float timer = 0;
        for (int i = 1; i < (finalAction == finalAction.Owner.MovingAttackAct ? 
            path.Path.Count() - 1 : path.Path.Count()); i++)
        {
            Vector3 destPointsPos = GameManager.ModelPntToUnityPnt(path.Path.ElementAt(i), 0);
            yield return LookRotate(unit, destPointsPos, 0.3f);
            float distance = (unit.transform.position - destPointsPos).magnitude;
            while (timer < secondsPerMove)
            {
                unit.transform.position = Vector3.MoveTowards(unit.transform.position, 
                    destPointsPos, distance * Time.deltaTime / secondsPerMove);
                timer += Time.deltaTime;
                yield return null;
            }
            timer = 0;
        }
    }

    /*
     * Default Method for Attack Animation
     * 
     * unit : unit script of moving Actor
     * targetPoint : Target of attack
     */
    public static IEnumerator AttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.6f);
        yield return LookRotate(unit, destination, 0.3f);

        if (unit.unitModel is CivModel.Hwan.BrainwashedEMUKnight ||
            unit.unitModel is CivModel.Finno.EMUHorseArcher)
            yield return OstrichAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.DecentralizedMilitary)
            yield return SoldierAttackAnimation(unit, targetPoint, true);

        else if (unit.unitModel is CivModel.Finno.DecentralizedMilitary)
            yield return SoldierAttackAnimation(unit, targetPoint, false);

        else if (unit.unitModel is CivModel.Hwan.Spy ||
                 unit.unitModel is CivModel.Finno.Spy)
            yield return SpyAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.LEOSpaceArmada)
            yield return TurtleshipAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Finno.ElephantCavalry)
            yield return ElephantAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.UnicornOrder)
            yield return UnicornAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Finno.AncientSorcerer)
            yield return SorcererAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.ProtoNinja)
            yield return NinjaAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Finno.AutismBeamDrone)
            yield return DroneAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.JediKnight ||
                 unit.unitModel is CivModel.Finno.JediKnight)
            yield return JediAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.JackieChan)
            yield return JackieChanAttackAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Finno.GenghisKhan)
            yield return GenghisKhanAttackAnimation(unit, targetPoint);
    }

    static IEnumerator OstrichAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return Charge(unit, targetPoint, 7);
    }

    static IEnumerator SoldierAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint, bool isHwan)
    {
        if (isHwan) yield return FireProjectiles(unit, HwanSoldierBulletPool, 3, targetPoint, 5f, 5f);
        else yield return FireProjectiles(unit, FinnoSoldierBulletPool, 3, targetPoint, 5f, 5f);
    }

    static IEnumerator SpyAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.6f);
        GameObject Attack = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "Attack"),
            unit.transform);
        yield return new WaitForSeconds(0.2f);
        Object.Destroy(Attack);
        GameObject Hit = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "SlashHit"),
            destination, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        Object.Destroy(Hit);
    }

    static IEnumerator TurtleshipAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return FireProjectile(unit, TurtleshipFireballPool, targetPoint, 2.5f);
    }

    static IEnumerator ElephantAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return Charge(unit, targetPoint, 10);
    }

    static IEnumerator UnicornAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return Charge(unit, targetPoint, 10);
    }

    static IEnumerator SorcererAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.1f);
        GameObject Aura = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoSorcererAttackAura"),
            unit.transform.position + new Vector3(0, 0.2f, 0), Quaternion.Euler(-90f, 0, 0));
        GameObject Charge = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoSorcererAttackCharge"),
            unit.transform);
        Charge.transform.localPosition = new Vector3(-0.65f, 1.65f, 0.38f);
        yield return new WaitForSeconds(0.5f);
        Object.Destroy(Charge);
        GameObject Attack = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoSorcererAttack"),
            destination, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Object.Destroy(Aura);
        Object.Destroy(Attack);
    }

    static IEnumerator NinjaAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.6f);
        GameObject Attack = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "NinjaAttack"),
            unit.transform);
        yield return new WaitForSeconds(0.2f);
        Object.Destroy(Attack);
        GameObject Hit = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "SlashHit"),
            destination, Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        Object.Destroy(Hit);
    }

    static IEnumerator DroneAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return FireProjectiles(unit, FinnoDroneBulletPool, 3, targetPoint, 5f, 5f);
    }

    static IEnumerator JediAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.6f);
        GameObject Attack = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "Attack"),
            unit.transform);
        yield return new WaitForSeconds(0.1f);
        GameObject Attack2 = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "Attack"),
            unit.transform);
        Attack2.transform.localRotation = Quaternion.Euler(0, 0, 90f);
        yield return new WaitForSeconds(0.1f);
        Object.Destroy(Attack);
        GameObject Hit = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "SlashHit"),
            destination, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Object.Destroy(Attack2);
        GameObject Hit2 = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "SlashHit"),
            destination, Quaternion.Euler(0, 0, 90));
        yield return new WaitForSeconds(0.2f);
        Object.Destroy(Hit);
        yield return new WaitForSeconds(0.1f);
        Object.Destroy(Hit2);
    }

    static IEnumerator JackieChanAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return JackieCharge(unit, targetPoint, 10);
    }

    static IEnumerator GenghisKhanAttackAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return Charge(unit, targetPoint, 10);
    }

    /*
     * Default Method for Skill Animation
     * 
     * unit : unit script of moving Actor
     * targetPoint : Target of skill. null if there is no target
     */
    public static IEnumerator SkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        if(targetPoint != null)
        {
            Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.6f);
            yield return LookRotate(unit, destination, 0.3f);
        }
            
        if (unit.unitModel is CivModel.Hwan.Spy || unit.unitModel is CivModel.Finno.Spy)
            yield return SpySkillAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.LEOSpaceArmada)
            yield return TurtleshipSkillAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Finno.ElephantCavalry)
            yield return ElephantSkillAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.UnicornOrder)
            yield return UnicornSkillAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Finno.AncientSorcerer)
            yield return SorcererSkillAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.ProtoNinja)
            yield return NinjaSkillAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Finno.AutismBeamDrone)
            yield return DroneSkillAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Hwan.JediKnight)
            yield return JediSkillAnimation(unit, targetPoint, true);
        else if(unit.unitModel is CivModel.Finno.JediKnight)
            yield return JediSkillAnimation(unit, targetPoint, false);

        else if (unit.unitModel is CivModel.Hwan.JackieChan)
            yield return JackieChanSkillAnimation(unit, targetPoint);

        else if (unit.unitModel is CivModel.Finno.GenghisKhan)
            yield return GenghisKhanSkillAnimation(unit, targetPoint);
    }
    
    static IEnumerator TurtleshipSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.1f);
        GameObject Laser = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "HwanTurtleshipLaser"),
           destination + new Vector3(0, 15f, 0), Quaternion.identity);
        Laser.GetComponent<Laserbeam>().Fire(destination, 0.3f);
        yield return null;
    }
    
    static IEnumerator NinjaSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.1f);
        Vector3 jump = unit.transform.localPosition + new Vector3(0, 10, 0);
        float timer = 0;
        while(timer < 1f)
        {
            if (Vector3.Distance(unit.transform.localPosition, jump) < 0.01) break;
            unit.transform.localPosition = Vector3.MoveTowards(unit.transform.localPosition, jump, 20 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < 1f)
        {
            unit.transform.localRotation *= Quaternion.AngleAxis(2000 * Time.deltaTime, unit.transform.up);
            unit.transform.localPosition = Vector3.MoveTowards(unit.transform.localPosition, destination, 50 * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        GameObject NinjaKill = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "NinjaKill"),
           destination + new Vector3(0, 0.5f, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.3f);
        Object.Destroy(NinjaKill);
        yield return null;
    }

    static IEnumerator JackieChanSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        GameObject Aura = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "HwanJackieChanSkill"),
                unit.transform);
        Aura.transform.localPosition = new Vector3(0, 0.4f, 0);
        float timer = 0;
        float speed = 0;
        while(timer < 1.6f)
        {
            unit.transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime, unit.transform.up);
            if (timer < 0.8f)
                speed += Time.deltaTime * 2000;
            else
                speed -= Time.deltaTime * 2000;
            timer += Time.deltaTime;
            yield return null;
        }
        Object.Destroy(Aura);
    }

    static IEnumerator SorcererSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.1f);
        GameObject Aura = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoSorcererSkillAura"),
            unit.transform.position + new Vector3(0, 0.2f, 0), Quaternion.Euler(-90f, 0, 0));
        GameObject Charge = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoSorcererSkillCharge"),
            unit.transform);
        Charge.transform.localPosition = new Vector3(-0.65f, 1.65f, 0.38f);
        yield return new WaitForSeconds(0.5f);
        Object.Destroy(Charge);
        GameObject Attack = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoSorcererSkill"),
            destination, Quaternion.identity);
        yield return new WaitForSeconds(1.3f);
        Object.Destroy(Aura);
        Object.Destroy(Attack);
        yield return null;
    }

    static IEnumerator DroneSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        Vector3 destination = GameManager.ModelPntToUnityPnt(targetPoint, 0.6f);
        GameObject Laser = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoDroneLaser"),
            unit.transform);
        Laser.GetComponent<Laserbeam>().Fire(destination, 0.3f);
        yield return new WaitForSeconds(1.3f);
        GameObject After = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoDroneLaserAfter"),
            destination + new Vector3(0, 0.1f, 0), Quaternion.Euler(-90, 0, 0));
        yield return new WaitForSeconds(1);
        Object.Destroy(After);
    }

    static IEnumerator UnicornSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return Fly(unit, targetPoint, 20, 60f);
    }

    static IEnumerator ElephantSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        yield return Fly(unit, targetPoint, 15, 0);
    }

    static IEnumerator JediSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint, bool isHwan)
    {
        GameObject Shield;
        if (isHwan)
        {
            Shield = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "HwanJediShield"),
                unit.transform);
        }
        else
        {
            Shield = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoJediShield"),
                unit.transform);
        }
        Shield.transform.localPosition = new Vector3(0, 0.8f, 0);
        yield return new WaitForSeconds(2);
        Object.Destroy(Shield);
    }

    static IEnumerator SpySkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        GameObject Fog = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "SpySkill"),
            unit.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        yield return new WaitForSeconds(2);
        Object.Destroy(Fog);
    }

    static IEnumerator GenghisKhanSkillAnimation(Unit unit, CivModel.Terrain.Point targetPoint)
    {
        GameObject Aura = Object.Instantiate(Resources.Load<GameObject>(particleResourcePath + "FinnoGKSkill"),
                unit.transform);
        Aura.transform.localPosition = new Vector3(0, 0.4f, 0);
        yield return new WaitForSeconds(1.7f);
        Object.Destroy(Aura);
    }
}
