using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CivModel;
using CivModel.Common;

public class CIVGameManager : MonoBehaviour {
    public float outerRadius;
    public float innerRadius;

    public int width = 80;
    public int height = 20;

    public GameObject cellPrefab;
    private GameObject[,] _cells;

    // Use this for initialization
    void Start() {
        innerRadius = outerRadius * Mathf.Sqrt(3.0f) / 2;
        _cells = new GameObject[width, height];
        DrawMap();
	}
	
	// Update is called once per frame
	void Update() {
		
	}

    void DrawMap()      // Draw hexagonal tile map
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector3 pos = new Vector3(2 * i * innerRadius, -0.05f, -j * outerRadius * 1.5f);
                if (j % 2 != 0)
                {
                    pos.x -= innerRadius;
                }
                _cells[i, j] = Instantiate(cellPrefab, pos, Quaternion.identity);
                _cells[i, j].name = "(" + i + "," + j + ")";
            }
        }
    }
}
