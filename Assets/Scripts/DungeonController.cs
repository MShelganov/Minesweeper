using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pazzle.game.contrellers
{
    public class DungeonController : GameController
    {
        public Board BoardGame;
        public Selecter TileSelecter;

        /// <summary>
        /// The level attributes.
        /// </summary>
        private int mines, rowGrid, colGrid, rowHole, colHole, widthHole, heightHole;

        public override void Start()
        {
            base.Start();
            target = new Vector3(0f, 0f, 0f);

            if (BoardGame is null)
            {
                Debug.LogException(new Exception("The Board is null!"));
                return;
            }
            switch (Level.Value)
            {
                case 0: // Test
                    mines = 10;
                    rowGrid = 10;
                    colGrid = 10;
                    rowHole = 5;
                    colHole = 5;
                    widthHole = 2;
                    heightHole = 2;
                    break;
                case 1:
                    // TODO
                    break;
                case 2:
                    // TODO
                    break;
            }
            BoardGame.SetSetup(mines, rowGrid, colGrid);
            BoardGame.SetDimensions();
            BoardGame.SetEmptySpace(rowHole, colHole, widthHole, heightHole);
            BoardGame.SetNumberOfMines();

        }

        public override void Update()
        {
            base.Update();

            if (Physics.Raycast(ray, out raycastHit, 100.0f))
            {
                if (raycastHit.collider.tag == Tags.Tile)
                {
                    Tile tile = raycastHit.transform.gameObject.GetComponentInParent<Tile>();
                    int row = tile.Row;
                    int col = tile.Col;

                    if (TileSelecter.transform.position != tile.transform.position)
                    {
                        TileSelecter.transform.position = tile.transform.position;
                        TileSelecter.gameObject.SetActive(true);
                    }

                    if (BoardGame != null)
                    {
                        tile.Select = true;
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (BoardGame.IsBomb(row, col))
                            {

                            }
                            else
                            {
                                // opens the plate and checks for surrounding bombs
                                int count = BoardGame.Open(row, col);
                            }
                        }
                        if (Input.GetMouseButtonDown(1))
                            BoardGame.FlagMine(tile.Row, tile.Col);
                    }
                }
                else
                {
                    if (TileSelecter.gameObject.activeSelf)
                    {
                        TileSelecter.gameObject.SetActive(false);
                        TileSelecter.ResetOffest();
                    }
                }
            }
            else
            {
                if (TileSelecter.gameObject.activeSelf)
                {
                    TileSelecter.gameObject.SetActive(false);
                    TileSelecter.ResetOffest();
                }
            }
        }
    }
}