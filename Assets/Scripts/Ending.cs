using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivPresenter;

public class Ending : MonoBehaviour {

    public GameObject WinPrefab;
    public GameObject LosePrefab;

    private Presenter mPresenter;
    private static GameObject result;
    private bool shown;

    // Use this for initialization
    void Start () {
        shown = false;
        mPresenter = CIVGameManager.GetGameManager().GetComponent<CIVGameManager>().GetPresenter();
	}
	
	// Update is called once per frame
	void Update ()
    {
        switch (mPresenter.State)//for debug
        {
            case CivPresenter.Presenter.States.Victory:
                {
                    WinorLose(true);
                    break;
                }
            case CivPresenter.Presenter.States.Defeated:
                {
                    WinorLose(false);
                    break;
                }
        }

    }
    public void WinorLose(bool win)
    {
        if(result != null)
        { return; }
        if(shown)
        { return; }
        if (win)
        {
            result = Instantiate(WinPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            result.transform.SetParent(this.gameObject.transform);
            result.transform.localScale = new Vector3(1f, 1f, 1f);
            result.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        else
        {
            result = Instantiate(LosePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            result.transform.SetParent(this.gameObject.transform);
            result.transform.localScale = new Vector3(1f, 1f, 1f);
            result.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        shown = true;

    }
}
