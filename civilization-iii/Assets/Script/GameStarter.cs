using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{

    public Text txt;
    // Use this for initialization
    void Start()
    {
        GameInfo.SetPlayer(CivModel.Hwan.HwanPlayerConstant.HwanPlayer);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartHwan()
    {
        GameInfo.SetPlayer(CivModel.Hwan.HwanPlayerConstant.HwanPlayer);
        txt.text = "환국으로 게임 시작";
    }
    public void StartSuomen()
    {
        GameInfo.SetPlayer(CivModel.Finno.FinnoPlayerConstant.FinnoPlayer);
        txt.text = "수오미로 게임 시작";
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
