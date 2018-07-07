using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;

public class UIManager : MonoBehaviour
{
    GameObject mapUI;
	GameObject managementUI;
	GameObject questUI;
    GameObject selectedUnit;

    public Ray ray;
    public RaycastHit hit;

	// Use this for initialization
	void Start()
    {
        mapUI = GameObject.Find("MapUI");
		managementUI = GameObject.Find("ManagementUI");
		questUI = GameObject.Find("QuestUI");
        // mapUI.SetActive(false);
        managementUI.SetActive(false);
        questUI.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0)) {
            if(Physics.Raycast(ray, out hit)){
                selectedUnit = hit.transform.gameObject;
                Debug.Log(selectedUnit.name);
                Unit unt = selectedUnit.GetComponent<Unit>();
                if(unt != null)
                    GameManager.Instance.selectedUnit = unt.point.Unit;
            }
        }

    }

    public void onClick(GameObject go)
    {
        if (go.activeSelf == false)
        {
            go.SetActive(true);
			if (go != mapUI) mapUI.SetActive(false);
			if (go != managementUI) managementUI.SetActive(false);
			if (go != questUI) questUI.SetActive(false);
		}
        else
        {
            go.SetActive(false);
            if (go != mapUI) mapUI.SetActive(true);
        }
    }

    public void onClickMove() {


    }


}