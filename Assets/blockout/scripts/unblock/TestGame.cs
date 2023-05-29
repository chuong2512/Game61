using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitcode_blockout
{
    public class TestGame : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            GameManager.getInstance().init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseDown()
        {
            GameData.getInstance().isLock = false;
            GetComponent<MeshRenderer>().GetComponent<Renderer>().material.color = Color.white;

            //init game
            blockout tg = GameObject.Find("blockout").GetComponent<blockout>();
            tg.clear();

            //set difficulty and level
            GameData.difficulty = 0;
            GameData.instance.cLevel = Random.Range(0, GameData.totalLevel[GameData.difficulty]);

            //start game;
            tg.init();

        }

        public void win()
        {
            GetComponent<MeshRenderer>().GetComponent<Renderer>().material.color = Color.red;
            GameData.instance.isWin = true;
        }
    }
}
