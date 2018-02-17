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
    private IEnumerator _coroutine;

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
        Debug.Log(gameObject.name + " is flickering with blue");
        if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.blue);
        StartCoroutine(_coroutine);
    }

    // Blink with red color. This is used for attack.
    public void FlickerRed()
    {
        isFlickering = true;
        Debug.Log(gameObject.name + " is flickering with red");
        if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
            return;
        _coroutine = Flicker(Color.red);
        StartCoroutine(_coroutine);
    }

    public void StopFlickering()
    {
        isFlickering = false;
        Debug.Log(gameObject.name + " stopped flickering");
        if (terrains.GetChild((int)point.Type).GetComponent<Renderer>() == null)
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

        while(true)
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
