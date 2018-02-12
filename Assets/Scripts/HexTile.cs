using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using CivModel.Common;

public class HexTile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // This method should be changed when 
    public void DrawUnit(CivModel.Unit unit)
    {
        if (unit == null)
        {
            foreach(Transform child in transform)
            {
                child.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "Pioneer")
                {
                    child.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    child.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }
}
