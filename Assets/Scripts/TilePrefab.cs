using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Sprites;
using CivModel;

public class TilePrefab : MonoBehaviour {
    [SerializeField]
    public Sprite TileSprite;

    private Sprite tileSprite;
    private Sprite districtSprite;
    private Sprite unitSprite;

    // Use this for initialization
    void Start () {
        tileSprite = TileSprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = tileSprite;
	}
	
	// Update is called once per frame
	void Update () {

    }
    public void MovableTile()
    {
        string color = "Magenta";
        ChangeTile(color);
    }
    public void ChangeTile(CivModel.Terrain.Point terrainPoint)
    {
        var t1 = terrainPoint.Type;
        string color = "None";
        string[] colors = new string[]{ "Blue", "Green", "Yellow", "Red", "Orange","Magenta", "Gray", "IronGreen", "DarkPurple", "None" };
        
        switch (t1)
        {
            case TerrainType.Plain:
                color = colors[1];
                break;
            case TerrainType.Ocean:
                color = colors[0];
                break;
            case TerrainType.Mount:
                color = colors[4];
                break;
            case TerrainType.Forest:
                color = colors[8];
                break;
            case TerrainType.Swamp:
                color = colors[7];
                break;
            case TerrainType.Tundra:
                color = colors[6];
                break;
            case TerrainType.Ice:
                color = colors[9];
                break;
            case TerrainType.Hill:
                color = colors[2];
                break;

                //3 , 5 not used (RED, MAGENTA) - prototype
        }
        ChangeTile(color);
    }
    public void ChangeTile(string tile)
    {
        if (tile == null)
        {
            tileSprite = TileSprite;
            gameObject.GetComponent<SpriteRenderer>().sprite = tileSprite;
            return;
        }

        CIVGameManager.TileSprite ts = CIVGameManager.TileSprite.None;
        Enum.TryParse(tile, out ts);
        tileSprite = CIVGameManager.TileSprites[(int)ts];

        gameObject.GetComponent<SpriteRenderer>().sprite = tileSprite;
    }
    public void BuildDistrict(CivModel.TileBuilding building)
    {
        if (building == null)
        {
            BuildDistrict("None");
        }
        else
            BuildDistrict(building?.ToString().Split('.').Last());
    }
    public void BuildDistrict(string dist)
    {
        CIVGameManager.DistrictSprite ds = CIVGameManager.DistrictSprite.None;
        Enum.TryParse(dist, out ds);
        districtSprite = CIVGameManager.DistrictSprites[(int)ds];

        GetComponentsInChildren<SpriteRenderer>()[1].sprite = districtSprite;
    }
    public void DrawUnit(CivModel.Unit unit)
    {
        if(unit == null)
        {
            DrawUnit("None");
        }
        else
            DrawUnit(unit?.ToString().Split('.').Last());
    }
    public void DrawUnit(string unit)
    {
        CIVGameManager.UnitSprite us = CIVGameManager.UnitSprite.None;
        Enum.TryParse(unit, out us);
        unitSprite = CIVGameManager.UnitSprites[(int)us];
        GetComponentsInChildren<SpriteRenderer>()[2].sprite = unitSprite;
    }
    public void DestroyDistrict()
    {
        districtSprite = CIVGameManager.DistrictSprites[(int)CIVGameManager.DistrictSprite.None];
    }
}
