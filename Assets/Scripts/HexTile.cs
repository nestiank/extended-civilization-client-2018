using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using CivModel.Common;
using System;

public class HexTile : MonoBehaviour {
    public CivModel.Terrain.Point point;

    Transform terrains;
    Transform units;

	// Use this for initialization
	void Start () {
        terrains = transform.GetChild(0).transform;
        units = transform.GetChild(1).transform;
	}
	
	// Update is called once per frame
	void Update () {

    }

    // Render tile terrain
    public void ChangeTile()
    {
        if (point.Type == CivModel.TerrainType.Hill)
        {
            terrains.Find("Hill").gameObject.SetActive(true);
        }
        else if (point.Type == CivModel.TerrainType.Mount)
        {
            terrains.Find("Mount").gameObject.SetActive(true);
        }
    }

    // This method should be changed when unit type increses
    public void DrawUnit(CivModel.Unit unit)
    {
        if (unit == null)
        {
            foreach (Transform child in units)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Transform child in units)
            {
                if (child.gameObject.name == "Pioneer")
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}
