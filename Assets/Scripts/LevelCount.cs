using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pazzle.game
{
    public class LevelCount
    {
        public static LevelCount TEST { get => new LevelCount(0); }

        public int Value { get; set; }

        public LevelCount(int value)
        {
            this.Value = value;
        }
    }
}
