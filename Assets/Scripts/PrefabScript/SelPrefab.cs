using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;
using CivPresenter;

public class SelPrefab : MonoBehaviour {

    private static Presenter presenter;
    private static ManagementUIController uicontroller;

    private Text unitName;
    private Image unitPrt;
    private Text theNumberofProduce;
    private Button[] buttons;
    // Use this for initialization
    void Awake () {
        Debug.Log("call SelPre");
        unitName = gameObject.GetComponentsInChildren<Text>()[1];
        unitName.text = "초기";
        foreach (Image unt in gameObject.GetComponentsInChildren<Image>())
        {
            if (unt.name == "Portrait")
            {
                unitPrt = unt;
            }
        }
        buttons = gameObject.GetComponentsInChildren<Button>();
        theNumberofProduce = gameObject.GetComponentsInChildren<Text>()[2];
    }
    void Start()
    {
        uicontroller = ManagementUIController.GetManagementUIController();
    }

    // Update is called once per frame
    void Update () {
	}

    public GameObject MakeItem(IProductionFactory fact)
    {
        Debug.Log("Selection Queue Item Made");
        string nameofFactory = ProductionFactoryTraits.GetFactoryName(fact);
        unitPrt.sprite = Resources.Load<Sprite>("Unit_portrait/" + nameofFactory +"_portrait");
        unitName.text = nameofFactory;
        theNumberofProduce.text = "X 1";
        return this.gameObject;
    }
    public GameObject MakeItem()
    {
        unitPrt.enabled = false;
        Debug.Log("NULL Selection Queue");
        unitName.text = "생산 가능 유닛 없음";
        unitName.fontSize = 10;
        theNumberofProduce.text = "";
        return this.gameObject;
    }
    public void SetButton(int i)
    {
        foreach (Button but in buttons)
        {
            if (but.name == "Produce")
            {
                but.onClick.AddListener(delegate() { ProduceItem(i); });
            }
        }
    }
    public static void SetPresenter()
    {
        presenter = CIVGameManager.GetGameManager().GetComponent<CIVGameManager>().GetPresenter();
    }
    private void ProduceItem(int i)
    {
        if(presenter.State == Presenter.States.ProductUI)
        {
            presenter.CommandApply();
            for(int k = 0; k < i; k++)
            {
                presenter.CommandArrowKey(Direction.Down);
            }
            presenter.CommandApply();
            uicontroller.MakeProductionQ();
            uicontroller.MakeDeploymentQ();
        }
    }
}

