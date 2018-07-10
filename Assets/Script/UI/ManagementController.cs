using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementController : MonoBehaviour {

    private static ManagementController managementcontroller;

    void Awake()
    {
        DontDestroyOnLoad(this);
        if (managementcontroller == null)
        {
            managementcontroller = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void begin()
    {

    }

    public static ManagementController GetManagementController()
    {
        if (managementcontroller == null)
        {
            Debug.Log("ManagementController not made");
            throw new MissingComponentException();
        }
        return managementcontroller;
    }
}
