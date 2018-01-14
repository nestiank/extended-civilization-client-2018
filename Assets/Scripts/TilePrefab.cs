using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Sprites;

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
    public void ChangeTile(CivModel.Terrain.Point terrainPoint)
    {
        var t1 = terrainPoint.Type1;
        var t2 = terrainPoint.Type2;
        string color = "None";
        string[] colors = new string[]{ "Blue", "Green", "Yellow", "Red", "None" };

        color = colors[((int)t1 + (int)t2) % 5];

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
