using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pazzle.game.contrellers
{
    [ExecuteInEditMode]
    public class GameController : MonoBehaviour
    {
        public LevelCount Level;
        public Camera MainCamera;
        // Camera
        public float cameraSpeed = 20.0f;
        public float cameraHeight = 10.0f;
        public float smoothSpeed = 0.125f;
        public float panBorderThickness = 20f;
        // Settings
        public bool interpolate = true;
        [Range(0, 360)]
        public float panAngle = 70.0f;
        [Range(100, 180)]
        public float tiltAngle = 160.0f;
        [Range(20, 500)]
        public float distance = 100.0f;
        [Range(1, 12)]
        public uint steps = 8;
        [Range(1, 12)]
        public float yFactor = 1;
        public bool wrapPanAngle = false;
        public Vector3 pos = new Vector3(0.0f, 36.0f, -93.0f);
        public Vector3 target;

        protected Ray ray;
        protected RaycastHit raycastHit;
        protected Vector3 nextStepTarget;
        protected Rect maxArea;

        private float currentPanAngle = 0;
        private float currentTiltAngle = 90;
        private float lastPanAngle;
        private float lastTiltAngle;
        private float lastMouseX;
        private float lastMouseY;
        private const float hoverConst = 0.3f;
        private bool move = false;


        public virtual void Start()
        {
            if (MainCamera is null)
            {
                Debug.LogException(new Exception("The MainCamera is null!"));
                return;
            }
            target = new Vector3();
            nextStepTarget = new Vector3();
            currentPanAngle = panAngle;
            currentTiltAngle = tiltAngle;
            maxArea = new Rect(-500.0f, -500.0f, 1000f, 1000f);
        }

        public virtual void Update()
        {
            ray = MainCamera.ScreenPointToRay(Input.mousePosition);

            MoveCamera();

            SetHover();

            HoverCamera();
        }

        private void SetHover()
        {
            // Right Mouse button
            if (Input.GetMouseButtonDown(1)) // Pressed
            {
                move = true;
                lastPanAngle = panAngle;
                lastTiltAngle = tiltAngle;
                lastMouseX = Input.mousePosition.x;
                lastMouseY = Input.mousePosition.y;
            }
            if (Input.GetMouseButtonUp(1)) // Unpressed
            {
                move = false;
            }
        }

        private void MoveCamera()
        {
            if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                if (nextStepTarget.z < maxArea.yMax)
                {
                    nextStepTarget.z += cameraSpeed * Time.deltaTime;
                }
            }
            else if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorderThickness)
            {
                if (nextStepTarget.z > maxArea.yMin)
                {
                    nextStepTarget.z -= cameraSpeed * Time.deltaTime;
                }
            }

            if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorderThickness)
            {
                if (nextStepTarget.x > maxArea.xMin)
                {
                    nextStepTarget.x -= cameraSpeed * Time.deltaTime;
                }
            }
            else if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                if (nextStepTarget.x < maxArea.xMax)
                {
                    nextStepTarget.x += cameraSpeed * Time.deltaTime;
                }
            }
        }

        private void HoverCamera()
        {
            if (!tiltAngle.Equals(currentTiltAngle) || !panAngle.Equals(currentPanAngle) || !target.Equals(pos))
            {

                if (wrapPanAngle)
                {
                    if (panAngle < 0)
                    {
                        currentPanAngle += panAngle % 360 + 360 - panAngle;
                        panAngle = panAngle % 360 + 360;
                    }
                    else
                    {
                        currentPanAngle += panAngle % 360 - panAngle;
                        panAngle = panAngle % 360;
                    }

                    while (panAngle - currentPanAngle < -180)
                        currentPanAngle -= 360;

                    while (panAngle - currentPanAngle > 180)
                        currentPanAngle += 360;
                }

                if (interpolate)
                {
                    currentTiltAngle += (tiltAngle - currentTiltAngle) / (steps + 1);
                    currentPanAngle += (panAngle - currentPanAngle) / (steps + 1);
                }
                else
                {
                    currentPanAngle = panAngle;
                    currentTiltAngle = tiltAngle;
                }

                //snap coords if angle differences are close
                if ((Mathf.Abs(tiltAngle - currentTiltAngle) < 0.01) && (Mathf.Abs(panAngle - currentPanAngle) < 0.01))
                {
                    currentTiltAngle = tiltAngle;
                    currentPanAngle = panAngle;
                }

                if (MainCamera is null) return;
                pos.x = target.x + distance * Mathf.Sin(currentPanAngle * Mathf.Deg2Rad) * Mathf.Cos(currentTiltAngle * Mathf.Deg2Rad);
                pos.z = target.z + distance * Mathf.Cos(currentPanAngle * Mathf.Deg2Rad) * Mathf.Cos(currentTiltAngle * Mathf.Deg2Rad);
                pos.y = target.y + distance * Mathf.Sin(currentTiltAngle * Mathf.Deg2Rad) * yFactor;
                MainCamera.transform.position = pos;
                MainCamera.transform.LookAt(target);
            }
        }

        void OnGUI()
        {
            distance += Input.mouseScrollDelta.y * 1.0f;
        }

        void FixedUpdate()
        {
            Vector3 smoothedPosition = Vector3.Lerp(target, nextStepTarget, smoothSpeed);
            target = smoothedPosition;

            if (move)
            {
                panAngle = hoverConst * (Input.mousePosition.x - lastMouseX) + lastPanAngle;
                tiltAngle = hoverConst * (Input.mousePosition.y - lastMouseY) + lastTiltAngle;
            }
        }
    }
}
