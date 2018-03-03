using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using CivModel.Common;
using System;

public class HexTile : MonoBehaviour
{
    public CivModel.Terrain.Point point;

    Transform terrains;
    Transform buildings;
    Transform units;

    public bool isFlickering;
    private IEnumerator _coroutine;

    // Use this for initialization
    void Start()
    {
        terrains = transform.GetChild(0).transform;
        units = transform.GetChild(1).transform;
        buildings = transform.GetChild(2).transform;

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Render tile terrain
    public void ChangeTile()
    {
        if(terrains == null)
        {
        }
        else
        {
            foreach (Transform child in terrains)
            {
                child.gameObject.SetActive(false);
            }
        }

        if (point.TileBuilding is CivModel.CityBase)
        {
            terrains.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            terrains.GetChild((int)point.Type).gameObject.SetActive(true);
        }
    }

    public void BuildDistrict(CivModel.TileBuilding building)
    {
        buildings.GetChild(2).gameObject.SetActive(false);
        for (int i = 0; i < 2; i++)
        {
            foreach (Transform child in buildings.GetChild(i))
            {
                child.gameObject.SetActive(false);
            }
        }

        if (building != null)
        {
            TileBuildingObject(building);
        }
    }
    void TileBuildingObject(CivModel.TileBuilding building)
    {
        Transform side;

        if (building is CivModel.CityBase)
        {
            //Debug.Log(building.Owner.Team);
            buildings.GetChild(2).gameObject.SetActive(true);
            side = buildings.GetChild((building.Owner.Team + 1) %  2);
            side.GetChild(0).gameObject.SetActive(true);
        }
        else if (building is CivModel.Hwan.HwanEmpireIbiza)
        {
            side = buildings.GetChild(0);
            side.GetChild(1).gameObject.SetActive(true);
        }
        else if (building is CivModel.Finno.AncientFinnoOctagon)
        {
            side = buildings.GetChild(1);
            side.GetChild(1).gameObject.SetActive(true);
        }
        else if (building is CivModel.Hwan.HwanEmpireLatifundium)
        {
            side = buildings.GetChild(0);
            side.GetChild(2).gameObject.SetActive(true);
        }
        else if (building is CivModel.Finno.AncientFinnoGermaniumMine)
        {
            side = buildings.GetChild(1);
            side.GetChild(2).gameObject.SetActive(true);
        }
        else if (building is CivModel.Hwan.HwanEmpireFIRFortress)
        {
            side = buildings.GetChild(0);
            side.GetChild(3).gameObject.SetActive(true);
        }
        else if (building is CivModel.Finno.AncientFinnoFIRFortress)
        {
            side = buildings.GetChild(1);
            side.GetChild(3).gameObject.SetActive(true);
        }
        else if (building is CivModel.Hwan.HwanEmpireKimchiFactory)
        {
            side = buildings.GetChild(0);
            side.GetChild(4).gameObject.SetActive(true);
        }
        else if (building is CivModel.Finno.AncientFinnoFineDustFactory)
        {
            side = buildings.GetChild(1);
            side.GetChild(4).gameObject.SetActive(true);
        }
    }


    // This method should be changed when unit type increses
    public void DrawUnit(CivModel.Unit unit)
    {
        if (unit == null)
        {
            units.gameObject.SetActive(false);
            return;
        }
        units.gameObject.SetActive(true);

        foreach (Material m in GameManager.I.materials)
        {
            if (m == GameManager.I.materials[(int)UnitToEnum(unit)])
            {
                units.GetComponent<Renderer>().material = m;
            }
        }
    }
    Units UnitToEnum(CivModel.Unit unit)
    {
        if (unit is CivModel.Hwan.Pioneer)
        {
            return Units.HwanPioneer;
        }
        else if (unit is CivModel.Hwan.BrainwashedEMUKnight)
        {
            return Units.HwanBrainwashedEmuKnight;
        }
        else if (unit is CivModel.Hwan.DecentralizedMilitary)
        {
            return Units.HwanDecentralizedMilitary;
        }
        else if (unit is CivModel.Hwan.Spy)
        {
            return Units.HwanSpy;
        }
        else if (unit is CivModel.Hwan.UnicornOrder)
        {
            return Units.HwanUnicornOrder;
        }
        else if (unit is CivModel.Hwan.LEOSpaceArmada)
        {
            return Units.HwanLEO;
        }
        else if (unit is CivModel.Hwan.JediKnight)
        {
            return Units.HwanJediKnight;
        }
        else if (unit is CivModel.Hwan.ProtoNinja)
        {
            return Units.HwanProtoNinja;
        }
        else if (unit is CivModel.Hwan.JackieChan)
        {
            return Units.HwanJackieChan;
        }
        else if (unit is CivModel.Finno.Pioneer)
        {
            return Units.FinnoPioneer;
        }
        else if (unit is CivModel.Finno.EMUHorseArcher)
        {
            return Units.FinnoEmuHorseArcher;
        }
        else if (unit is CivModel.Finno.DecentralizedMilitary)
        {
            return Units.FinnoDecentralizedMilitary;
        }
        else if (unit is CivModel.Finno.Spy)
        {
            return Units.FinnoSpy;
        }
        else if (unit is CivModel.Finno.ElephantCavalry)
        {
            return Units.FinnoElephantCavarly;
        }
        else if (unit is CivModel.Finno.AncientSorcerer)
        {
            return Units.FinnoAncientSorcerer;
        }
        else if (unit is CivModel.Finno.JediKnight)
        {
            return Units.FinnoJediKnight;
        }
        else if (unit is CivModel.Finno.AutismBeamDrone)
        {
            return Units.FinnoAutismBeamDrone;
        }
        else
        {
            return Units.HwanBrainwashedEmuKnight;
        }
    }

    // Flicker with blue color. This is used for parametered move and skill.
    public void FlickerBlue()
    {
        isFlickering = true;
        //Debug.Log(gameObject.name + " is flickering with blue");
        if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.blue);
        StartCoroutine(_coroutine);
    }

    // Blink with red color. This is used for attack.
    public void FlickerRed()
    {
        isFlickering = true;
        //Debug.Log(gameObject.name + " is flickering with red");
        if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.red);
        StartCoroutine(_coroutine);
    }

    public void StopFlickering()
    {
        isFlickering = false;
        //Debug.Log(gameObject.name + " stopped flickering");
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
