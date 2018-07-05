using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;

public class UIManager : MonoBehaviour
{
    GameObject mapUI;

    // Use this for initialization
    void Start()
    {
        mapUI = GameObject.Find("MapUI");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClick(GameObject go)
    {
        if (go.activeSelf == false)
        {
            go.SetActive(true);
            if (go != mapUI) mapUI.SetActive(false);
        }
        else
        {
            go.SetActive(false);
            if (go != mapUI) mapUI.SetActive(true);
        }
    }
}