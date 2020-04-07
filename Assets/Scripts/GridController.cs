﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private int rows, cols = 0;
    [SerializeField]
    private float tileSize;
    [Header("Debug Tools")]
    [SerializeField]
    private bool setGridUpdate;

    private Tile[,] gridTiles;

    public Tile[,] GridTiles
    {
        get
        {
            return gridTiles;
        }

        set
        {
            gridTiles = value;
        }
    }
    public int Rows { get => rows; }
    public int Cols { get => cols; }

    
    void Start()
    {
        gridTiles = new Tile[cols, rows];

        FillGrid();
    }

    void Update()
    {
        //Used at the beginning of development to determine grid size/positioning.
        if (setGridUpdate)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    gridTiles[i, j].transform.position = GetGridPosition(i, j);
                }
            }
        }
    
        if(Input.GetKeyDown(KeyCode.R))
        {
            ClearGrid();
            FillGrid();
        }
    }

    private void FillGrid()
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject go = Instantiate(tilePrefab, GetGridPosition(i, j), Quaternion.identity, this.gameObject.transform);
                go.name = "Tile_" + i + "_" + j;
                gridTiles[i, j] = go.GetComponent<Tile>();
                gridTiles[i, j].SetVisual();
                gridTiles[i, j].TileCoordinates = new Vector2(i, j);
            }
        }
    }

    private void ClearGrid()
    {
        //Since the grid can be changed during runtime just clear all the tiles manually instead of going by rows/cols.
        for (int i = transform.childCount; i --> 0; )
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }

        Array.Clear(gridTiles, 0, gridTiles.Length);
    }

    private Vector3 GetGridPosition(int i, int j)
    {
        Vector3 gridPos = new Vector3();

        gridPos = this.gameObject.transform.localPosition + new Vector3((i * tileSize), (-j * tileSize), 0f);

        return gridPos;
    }

    public IEnumerator RefillGrid()
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if(gridTiles[i, j] == null)
                {
                    print("Empty spot found at: " + i + "," + j);
                }
            }
        }

        //TODO: Event OnFillCompleted();

        yield return 0;
    }
}
