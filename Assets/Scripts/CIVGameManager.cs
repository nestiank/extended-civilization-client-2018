using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;
using CivModel;

public class CIVGameManager : MonoBehaviour {

    private static GameObject gameManager;

	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this);
        //System.Diagnostics.Debug.Assert(gameObject == null);
		if(gameManager == null)
        {
            gameManager = this.gameObject;
        }
        else
        {
            Destroy(this);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
