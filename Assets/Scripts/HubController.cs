using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pazzle.game.contrellers
{
    public class HubController : GameController
    {
        void Awake()
        {
            target = new Vector3(595f, 32f, 510f);
        }

        public override void Start()
        {
            base.Start();
            target = new Vector3(595f, 32f, 510f);
        }

        public override void Update()
        {
            base.Update();

        }
    }
}