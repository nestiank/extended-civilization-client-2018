using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexTile : MonoBehaviour
{
    // 현재 tile의 위치의 point class
    public CivModel.Terrain.Point point;

    // prefab의 자식 gameobject들 
    GameObject terrains;
    GameObject tilebuildings;

    /*
     * TODO
     * Hextile Prefab을 수정하여 terrain와 tilebuilding을 설정할 수 있게 해야함.
     */

    /*
     * 관련 legacy code
    public void ChangeTile()
    {
        if (terrains == null)
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
    }*/
}
