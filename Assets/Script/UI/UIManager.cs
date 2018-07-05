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
        GameObject map = GameObject.Find("MapUI");
		GameObject management = GameObject.Find("ManagementUI");
		GameObject quest = GameObject.Find("QuestUI");
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
			if (go != map) map.SetActive(false);
			if (go != management) management.SetActive(false);
			if (go != quest) quest.SetActive(false);
		}
        else
        {
            go.SetActive(false);
            if (go != mapUI) mapUI.SetActive(true);
        }
    }
}