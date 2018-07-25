using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewFieldSquareControl : MonoBehaviour {
    Vector3 Scaler;
    private int _zoom_counter;
    public int zoom_counter { get { return _zoom_counter; } }
    private static ViewFieldSquareControl _view = null;
    public static ViewFieldSquareControl ViewInstance { get { return _view; } }

    private void Awake()
    {
        _view = this;
    }
    // Use this for initialization
    void Start () {
        Scaler = new Vector3(0.22f, 0.22f, 0.22f);
        _zoom_counter = 2;
	}
	
	// Update is called once per frame
	void Update () {
        if (!(Input.mousePosition.x > 705 * Screen.width / 1022 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < 172 * Screen.height / 639))
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && _zoom_counter < 2)
            {
                GetComponent<Transform>().localScale += Scaler;
                transform.Translate(0, 0, 2.5f);
                _zoom_counter++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && _zoom_counter > 0)
            {
                GetComponent<Transform>().localScale -= Scaler;
                transform.Translate(0, 0, -2.5f);
                _zoom_counter--;
            }
        }
    }
}
