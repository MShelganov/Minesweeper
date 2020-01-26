using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace pazzle.game
{
    public class Tile : MonoBehaviour
    {
        public int Row;
        public int Col;
        public ParticleSystem CrushedStones;
        public GameObject Stone;
        public GameObject Selecter;
        public TextMeshPro CountText;
        public GameObject FlagObject;
        private bool _mine = false;
        private bool _flag = false;
        private bool _open = false;

        #region Getters amd Setters
        public bool Mine
        {
            get { return _mine; }
            set
            {
                if (!_mine && value)
                    CountText.text = "M";
                _mine = value;
            }
        }

        public bool Flag
        {
            get { return _flag; }
            set
            {
                if (!_open)
                {
                    FlagObject.SetActive(value);
                    _flag = value;
                }
            }
        }

        public bool Select
        {
            get; set;
        }
        
        public string Text
        {
            get
            {
                return CountText.text;
            }
            set
            {
                CountText.text = value;
            }
        }

        public bool Open
        {
            get { return _open; }
            set
            {
                if (!_open && value)
                {
                    if (CrushedStones != null)
                    {
                        ParticleSystem.EmissionModule em = CrushedStones.emission;
                        em.enabled = true;
                        CrushedStones.Play();
                    }
                    _open = true;
                    Destroy(Stone);
                    //Stone.SetActive(false);
                    FlagObject.SetActive(false);
                }
            }
        }
        #endregion Getters amd Setters

    }
}
