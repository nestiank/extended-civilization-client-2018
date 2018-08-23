using System.Collections;
using System.Collections.Generic;
using CivModel;
using UnityEngine;

public class SelectRay : MonoBehaviour
{
	void Update ()
    {
        if (GameManager.Instance.selectedActor is CivModel.Actor
            && GameManager.Instance.selectedActor.PlacedPoint != null)
        {
            transform.position = GameManager.ModelPntToUnityPnt
                (GameManager.Instance.selectedActor.PlacedPoint.Value, 0.3f);
            transform.GetChild(0).gameObject.SetActive(true);
        }

        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
