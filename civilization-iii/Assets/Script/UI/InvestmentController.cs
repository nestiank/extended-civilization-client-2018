using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CivModel;
using CivModel.Common;

public class InvestmentController : MonoBehaviour {

    public GameObject InvestmentUI;
    public GameObject Tax;
    public GameObject EcoInv;
    public GameObject TechInv;
    public GameObject Logistics;

    private Slider taxSlider;
    private Slider eiSlider;
    private Slider tiSlider;
    private Slider logiSlider;

    private Text taxRateText;
    private Text eiRateText;
    private Text tiRateText;
    private Text logiRateText;

    private static InvestmentController _IVUIController;
    public static InvestmentController I { get { return _IVUIController; } }

    void Awake()
    {
        // Singleton
        if (_IVUIController != null)
        {
            Destroy(this);
            return;
        }
        else
        {
            _IVUIController = this;
        }
        // Use this when scene changing exists
        // DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        taxSlider = Tax.GetComponentInChildren<Slider>();
        eiSlider = EcoInv.GetComponentInChildren<Slider>();
        tiSlider = TechInv.GetComponentInChildren<Slider>();
        logiSlider = Logistics.GetComponentInChildren<Slider>();
        initSlider();
    }
	
	// Update is called once per frame
	void Update () {
        GameManager.Instance.Game.PlayerInTurn.TaxRate = ((double)((int)(taxSlider.value * 100))) / 100f;
        GameManager.Instance.Game.PlayerInTurn.EconomicInvestmentRatio = ((double)((int)(eiSlider.value * 100))) / 100f;
        GameManager.Instance.Game.PlayerInTurn.ResearchInvestmentRatio = ((double)((int)(tiSlider.value * 100))) / 100f;
        GameManager.Instance.Game.PlayerInTurn.RepairInvestmentRatio = ((double)((int)(logiSlider.value * 100))) / 100f;

        taxRateText.text = ((int)(taxSlider.value * 100)).ToString() + "%";
        eiRateText.text = ((int)(eiSlider.value * 100)).ToString() + "%";
        tiRateText.text = ((int)(tiSlider.value * 100)).ToString() + "%";
        logiRateText.text = ((int)(logiSlider.value * 100)).ToString() + "%";
    }

    public void initSlider()
    {
        taxSlider.maxValue = 1f;
        taxSlider.minValue = 0f;

        eiSlider.maxValue = 2f;
        eiSlider.minValue = 0f;

        tiSlider.maxValue = 2f;
        tiSlider.minValue = 0f;

        logiSlider.maxValue = 1f;
        logiSlider.minValue = 0f;

        taxSlider.value = (float)GameManager.Instance.Game.PlayerInTurn.TaxRate;
        eiSlider.value = (float)GameManager.Instance.Game.PlayerInTurn.EconomicInvestmentRatio;
        tiSlider.value = (float)GameManager.Instance.Game.PlayerInTurn.ResearchInvestmentRatio;
        logiSlider.value = (float)GameManager.Instance.Game.PlayerInTurn.RepairInvestmentRatio;
        Text[] texts = InvestmentUI.GetComponentsInChildren<Text>();
        foreach (Text txt in texts)
        {
            switch (txt.name)
            {
                case "TRate":
                    taxRateText = txt;
                    break;
                case "PIRate":
                    eiRateText = txt;
                    break;
                case "TIRate":
                    tiRateText = txt;
                    break;
                case "LRate":
                    logiRateText = txt;
                    break;
                case "Current PIRate":
                    txt.text = "100%";
                    break;
                case "Current TIRate":
                    txt.text = "100%";
                    break;
                case "Current LRate":
                    txt.text = "50%";
                    break;
            }
        }
    }

    public void ChangeTaxValue(float adden)
    {
        taxSlider.value += adden;
    }
    public void ChangeEIValue(float adden)
    {
        eiSlider.value += adden;
    }
    public void ChangeTIValue(float adden)
    {
        tiSlider.value += adden;
    }
    public void ChangeLogiValue(float adden)
    {
        logiSlider.value += adden;
    }

    public void ChangeTaxPlus(float adden)
    {
        taxSlider.value += 0.01f;
        if (taxSlider.value > 1) taxSlider.value = 1;
    }
    public void ChangeEIPlus(float adden)
    {
        eiSlider.value += 0.01f;
        if (taxSlider.value > 2) taxSlider.value = 2;
    }
    public void ChangeTIPlus(float adden)
    {
        tiSlider.value += 0.01f;
        if (taxSlider.value > 2) taxSlider.value = 2;
    }
    public void ChangeLogiPlus(float adden)
    {
        logiSlider.value += 0.01f;
        if (taxSlider.value > 1) taxSlider.value = 1;
    }

    public void ChangeTaxMinus(float adden)
    {
        taxSlider.value -= 0.01f;
        if (taxSlider.value < 0) taxSlider.value = 0;
    }
    public void ChangeEIMinus(float adden)
    {
        eiSlider.value -= 0.01f;
        if (eiSlider.value < 0) eiSlider.value = 0;
    }
    public void ChangeTIMinus(float adden)
    {
        tiSlider.value -= 0.01f;
        if (taxSlider.value < 0) taxSlider.value = 0;
    }
    public void ChangeLogiMinus(float adden)
    {
        logiSlider.value -= 0.01f;
        if (taxSlider.value < 0) taxSlider.value = 0;
    }
}