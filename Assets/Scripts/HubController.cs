using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pazzle.game;

namespace pazzle.game.contrellers
{
    public class HubController : GameController
    {

        public override void Start()
        {
            base.Start();
            target = new Vector3(595.0f, 50.0f, 510.0f);
            nextStepTarget = new Vector3(target.x, target.y, target.z);
            maxArea = new Rect(200.0f, 200.0f, 800f, 775f);
        }

        public override void Update()
        {
            base.Update();

            if (Physics.Raycast(ray, out raycastHit, distance))
            {
                if (Input.GetMouseButtonDown(0)) // Left
                {
                    switch (raycastHit.collider.tag)
                    {
                        case Tags.Domain:
                            Debug.Log(Tags.Domain.ToString());
                            break;
                    }
                }
            }
        }
    }
}