﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action<int> OnLinkSuccesfull;
    
    [SerializeField] private GridController grid;
    [SerializeField] private LinkController linkController;
    [SerializeField] private ScoreHandler scoreHandler;

    private List<GameObject> selectedTiles;
    private bool lockInput, gameEnd;

    public bool LockInput { get => lockInput; set => lockInput = value; }
    public bool GameEnd { get => gameEnd; set => gameEnd = value; }


    void Start()
    {
        selectedTiles = new List<GameObject>();
        grid.OnFillCompleted += () => { lockInput = false; };
        lockInput = true;
    }

    void Update()
    {
        if (!lockInput && !gameEnd && Input.GetMouseButton(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                if(selectedTiles.Count == 0) //First hit
                {
                    selectedTiles.Add(hitInfo.transform.gameObject);
                    selectedTiles[selectedTiles.Count - 1].GetComponent<Tile>().ToggleLinkVisual(true);
                }
                else if(CheckSelection(hitInfo))
                {
                    selectedTiles.Add(hitInfo.transform.gameObject);
                    selectedTiles[selectedTiles.Count - 1].GetComponent<Tile>().ToggleLinkVisual(true);
                }
            }
        }
        if (!lockInput && !gameEnd && Input.GetMouseButtonUp(0))
        {
            FinishSelectedLink();
        }
    }

    private bool CheckSelection(RaycastHit hitInfo)
    {
        //If selected tile is not yet added to selected tiles; proceed.
        if(!selectedTiles.Contains(hitInfo.transform.gameObject)) 
        {
            //If selected tile is of the same type as the previously selected tile & a neighbour; proceed.
            if(linkController.IsTileMatchingNeighbour(hitInfo.transform.gameObject.GetComponent<Tile>(), selectedTiles[selectedTiles.Count - 1].GetComponent<Tile>()))
            {
                return true;
            }
        }
        else //Tile already in the list, maybe the player wants to remove his latest link, check if hit tile is previous hit tile.
        {
            if(selectedTiles.Count > 1 && selectedTiles[selectedTiles.Count - 2].Equals(hitInfo.transform.gameObject))
            {
                //print("Previous tile hit!");
                selectedTiles[selectedTiles.Count - 1].GetComponent<Tile>().ToggleLinkVisual(false);
                selectedTiles.RemoveAt(selectedTiles.Count - 1); //Remove latest hit tile, because the player swiped back to previous tile on his/her link.
            }
        }

        return false;
    }

    private void ResetSelection()
    {
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            selectedTiles[i].GetComponent<Tile>().ToggleLinkVisual(false);
        }
        selectedTiles.Clear();
    }

    private void FinishSelectedLink()
    {
        //Check if selected link is big enough then walk through the whole grid and set all selected tiles as completed.
        if(selectedTiles.Count > linkController.TilesRequiredForLink - 1)
        {
            for (int i = 0; i < grid.Cols; i++)
            {
                for (int j = 0; j < grid.Rows; j++)
                {
                    for (int k = 0; k < selectedTiles.Count; k++)
                    {
                        if (grid.GridTiles[i, j] != null && grid.GridTiles[i, j].gameObject.Equals(selectedTiles[k]))
                        {
                            //Completed tiles are hidden as their information is still needed to collapse the grid from the top.
                            grid.GridTiles[i, j].IsCompleted = true;
                            grid.GridTiles[i, j].GetComponent<SpriteRenderer>().sprite = null;
                            //The CompletedLink list is later used to delete this completed link.
                            grid.CompletedLink.Add(grid.GridTiles[i, j]);
                            break;
                        }
                    }
                }
            }

            if(OnLinkSuccesfull != null)
                OnLinkSuccesfull(selectedTiles.Count);

            scoreHandler.IncrementMovesTaken();
            ResetSelection();

            lockInput = true;
            StartCoroutine(grid.RefillGrid());
        }
        else //Link not big enough
        {
            //TODO: Show some red glow or something to remind the player that a minium of 3 links are needed.

            ResetSelection();
        }
    }
}
