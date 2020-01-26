using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pazzle.game
{
    public enum LevelCount
    {
        TEST, ONE, TWO
    }
    public class GameController : MonoBehaviour
    {
        public LevelCount Level = LevelCount.TEST;
        public Camera ThisCamera;
        public Board BoardGame;
        public Selecter TileSelecter;
        // Camera
        public float cameraSpeed = 20f;
        public float smoothSpeed = 0.125f;
        public float panBorderThickness = 20f;
        private Vector3 target;

        private RaycastHit raycastHit;
        // Level
        private int mines, rowGrid, colGrid, rowHole, colHole, widthHole, heightHole;

        void Start()
        {
            if (BoardGame == null || ThisCamera == null)
            {
                Debug.LogException(new Exception("Board is null!"));
                return;
            }
            target = new Vector3(0, 40, -25);
            switch (Level)
            {
                case LevelCount.TEST:
                    mines = 10;
                    rowGrid = 10;
                    colGrid = 10;
                    rowHole = 5;
                    colHole = 5;
                    widthHole = 2;
                    heightHole = 2;
                    break;
                case LevelCount.ONE:
                    // TODO
                    break;
                case LevelCount.TWO:
                    // TODO
                    break;
            }
            BoardGame.SetSetup(mines, rowGrid, colGrid);
            BoardGame.SetDimensions();
            BoardGame.SetEmptySpace(rowHole, colHole, widthHole, heightHole);
            BoardGame.SetNumberOfMines();
        }

        void Update()
        {
            Ray ray = ThisCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out raycastHit, 100.0f))
            {
                if (raycastHit.collider.tag == "Tile")
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




                if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
                    target.z += cameraSpeed * Time.deltaTime;
                else if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
                    target.z -= cameraSpeed * Time.deltaTime;

                if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
                    target.x -= cameraSpeed * Time.deltaTime;
                else if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
                    target.x += cameraSpeed * Time.deltaTime;
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

        void OnGUI()
        {
            Vector3 pos = target;
            pos.y += Input.mouseScrollDelta.y * 1f;
            target = pos;
        }
        void FixedUpdate()
        {
            Vector3 smoothedPosition = Vector3.Lerp(ThisCamera.transform.position, target, smoothSpeed);
            ThisCamera.transform.position = smoothedPosition;
        }
    }
}
