using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public AudioSource Hwan;
    public AudioSource Suomen;
    public AudioSource Fortuna;
    // Use this for initialization
    void Start () {
        if (GameInfo.UserPlayer == CivModel.Hwan.HwanPlayerConstant.HwanPlayer)
            Hwan.mute = false;
        else if (GameInfo.UserPlayer == CivModel.Finno.FinnoPlayerConstant.FinnoPlayer)
            Suomen.mute = false;
        else Fortuna.mute = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
