using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	public GameObject Hwan, Finno;
	// Use this for initialization
	void Start () {
		transform.GetChild(GameInfo.UserPlayer).GetComponent<AudioSource>().Play();
	}
	
}
