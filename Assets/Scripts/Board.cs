using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pazzle.game.dungeon
{
    public class Board : MonoBehaviour
    {
        // Public variables
        public int Mines = 0;
        public int Rows = 3;
        public int Cols = 3;
        public float xOffset = 1.0f;
        public float zOffset = 1.0f;
        public Tile StonePrefab;
        // Private variables
        private Tile[,] Grid;
        private float xPush = 0.0f;
        private float zPush = 0.0f;
        private int correctFlags;
        private int wrongFlags;

        // Getters
        public int FlaggedMines { get { return (this.correctFlags + this.wrongFlags); } }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            if (StonePrefab == null)
            {
                Debug.LogException(new Exception("Stone Prefab is null!"));
                return;
            }
            if (Rows < 1 || Cols < 1)
            {
                Debug.LogException(new Exception("Board size can not be less than 1x1"));
                return;
            }
            if (Mines > Rows * Cols)
                Debug.Log("Illegal number of mines");
        }

        #region Begin Game

        /// <summary>
        /// Constructor
        /// </summary>
        public void SetSetup(int Mines, int Rows, int Cols)
        {
            this.Mines = Mines;
            this.Rows = Rows;
            this.Cols = Cols;
            this.correctFlags = 0;
            this.wrongFlags = 0;

        }

        /// <summary>
        /// Create grid of blocks
        /// </summary>
        public void SetDimensions()
        {
            Grid = new Tile[Rows, Cols];
            xPush = (transform.position.x + Rows * xOffset) / 2;
            zPush = (transform.position.x + Cols * zOffset) / 2;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Grid[row, col] = Instantiate(StonePrefab,new Vector3((transform.position.x + row * xOffset) - xPush, transform.position.y, (transform.position.z + col * zOffset) - zPush), transform.rotation) as Tile;
                    Grid[row, col].Row = row;
                    Grid[row, col].Col = col;
                }
            }
        }

        public void SetEmptySpace(int row, int col, int width, int height)
        {
            if(width <= 0)
            {
                Debug.LogException(new Exception("Width must be greater than or equal to one!"));
                return;
            }

            if(height <= 0)
            {
                Debug.LogException(new Exception("Height must be greater than or equal to one!"));
                return;
            }

            int wInHalf = width == 1? 0 : width / 2;
            int hInHalf = height == 1? 0 : height / 2;

            for (int r = row - wInHalf; r <= row + wInHalf; ++r)
            {
                for (int c = col - hInHalf; c <= col + hInHalf; ++c)
                {
                    if (IsInside(r, c))
                    {
                        Grid[r, c].Open = true;
                    }
                }
            }
        }

        public void SetNumberOfMines()
        {
            int placed = 0;
            while (placed < Mines)
            {
                int row = UnityEngine.Random.Range(0, Rows);
                int col = UnityEngine.Random.Range(0, Cols);
                if (!Grid[row, col].Mine && !Grid[row, col].Open)
                {
                    Grid[row, col].Mine = true;
                    placed++;
                }
            }
        }

        #endregion Begin Game

        #region Check Game

        /// <summary>
        /// Method to check if the current position is mined
        /// </summary>
        /// <param name="row">Row of grid</param>
        /// <param name="col">Column of grid</param>
        /// <returns></returns>
        public bool IsBomb(int row, int col)
        {
            if (this.IsInside(row, col))
            {
                return Grid[row, col].Mine;
            }
            return false;
        }

        /// <summary>
        /// Method to check if the current position is mined
        /// </summary>
        /// <returns>bool</returns>
        public bool IsFlagged(int row, int col)
        {
            if (this.IsInside(row, col))
            {
                return this.Grid[row, col].Flag;
            }
            return false;
        }

        private bool IsInside(int row, int col)
        {
            return row >= 0 && col >= 0 && row < Rows && col < Cols;
        }

        private int NumberOfSurroundingMines(int row, int col)
        {
            int count = 0;
            for (int r = row - 1; r <= row + 1; ++r)
            {
                for (int c = col - 1; c <= col + 1; ++c)
                {
                    if (IsInside(r, c) && Grid[r, c].Mine)
                        count++;
                }
            }
            return count;
        }

        #endregion Check Game

        /// <summary>
        /// Method to determine the current cell's status
        /// it redirect to Plate.Check() to determine
        /// if the cell is mined,or how many mines are around it
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public int Open(int row, int col)
        {
            if (this.IsInside(row, col))
            {
                // Checks for number of surrounding mines
                int result = Check(row, col);
                // Checks for end of game
                CheckFinish();
                return result;
            }
            //Debug.LogException(new Exception(String.Format("Invalid Grid reference call [{0}, {1}] on reveal",row,col)));
            return -1;
        }

        /// <summary>
        /// Method to count the mines around the current cell and to put number on it depending ot the cell count
        /// </summary>
        /// <returns> If there're no mines around it redirect to MinesGrid.RevealPlate method to check all cells around for mines around them </returns>
        public int Check(int row, int col)
        {
            int counter = 0;
            Tile gr = Grid[row, col];

            if (!gr.Open && !gr.Flag)
            {
                gr.Open = true;

                // check all neighbours for bombs 
                for (int i = 0; i < 9; i++)
                {
                    // don't check itself
                    if (i == 4) continue;
                    // if there is a bomb, counts it
                    if (IsBomb(row + i / 3 - 1, col + i % 3 - 1)) counter++;
                }

                if (counter == 0)
                {
                    // check all neighbours for bombs 
                    for (int i = 0; i < 9; i++) 
                    {
                        // don't check itself
                        if (i == 4) continue;
                        // reveal all neighbours
                        Open(row + i / 3 - 1, col + i % 3 - 1);
                    }
                }
                else
                {
                    gr.Text = counter.ToString();
                }
            }
            return counter;
        }

        /// <summary>
        /// Method to put or remove flag if some cell is selected
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void FlagMine(int row, int col)
        {
            if (this.IsInside(row, col))
            {

                Tile tl = Grid[row, col];
                if (!tl.Flag)
                {
                    if (tl.Mine)
                        this.correctFlags++;
                    else
                        this.wrongFlags++;
                }
                else
                {
                    if (tl.Mine)
                        this.correctFlags--;
                    else
                        this.wrongFlags--;
                }
                // updates the flagged value
                tl.Flag = !tl.Flag;
                // checks for end of game
                CheckFinish(); 
            }
        }

        /// <summary>
        /// Method to check if the board is fully resolved
        /// </summary>
        private void CheckFinish()
        {
            // Assumes that the game is not finished
            bool hasFinished = false;
            // We have zero more flags to put
            if (this.wrongFlags == 0 && this.FlaggedMines == this.Mines) 
            {
                // Assumes that all plates are revealed
                hasFinished = true;
                foreach (Tile item in Grid)
                {
                    if (!item.Open && !item.Mine)
                    {
                        // If a plate is not revealed than the game is not finished
                        hasFinished = false; 
                        break;
                    }
                }
            }
            /*
             *ToDo: when the game has finished the timer must be stopped immediately
             */
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
