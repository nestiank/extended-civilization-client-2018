using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingMgr : MonoBehaviour {

    public static EndingMgr instance = null;

    public Image EndingImage;
    public Sprite ImageSprite;


    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start () {
        EndingImage.sprite = ImageSprite;
        ChangeSprite();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeSprite()
    {
        if (GameManager.Instance.Game.PlayerInTurn.IsVictoried)
        {
            if (GameManager.Instance.Game.PlayerInTurn == GameManager.Instance.Game.Players[0])
            {
                ImageSprite = Resources.Load<Sprite>("Endings/Hwan_ending");
            }
            else
            {
                ImageSprite = Resources.Load<Sprite>("Endings/Finno_ending");
            }
        }
        else if (GameManager.Instance.Game.PlayerInTurn.IsDefeated)
        {
            if (GameManager.Instance.Game.PlayerInTurn == GameManager.Instance.Game.Players[0])
            {
                ImageSprite = Resources.Load<Sprite>("Endings/Finno_ending");
            }
            else
            {
                EndingMgr.instance.ImageSprite = Resources.Load<Sprite>("Endings/Hwan_ending");
            }
        }
        else if (GameManager.Instance.Game.PlayerInTurn.IsDrawed)
        {
            EndingMgr.instance.ImageSprite = Resources.Load<Sprite>("Endings/Draw_ending");
        }
    }
}
