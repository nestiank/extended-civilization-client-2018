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

    public bool isFlickering;

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
        terrains.GetChild((int)point.Type).gameObject.SetActive(true);
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

    // Flicker with blue color. This is used for parametered move and skill.
    public void FlickerBlue()
    {
        isFlickering = true;
        Debug.Log(GameManager.I.Pos2Str(point.Position) + " is flickering with blue");
    }

    // Blink with red color. This is used for attack.
    public void FlickerRed()
    {
        isFlickering = true;
        Debug.Log(GameManager.I.Pos2Str(point.Position) + " is flickering with red");
    }

    public void StopFlickering()
    {
        isFlickering = false;
        Debug.Log(GameManager.I.Pos2Str(point.Position) + " stopped flickering");
    }
}
