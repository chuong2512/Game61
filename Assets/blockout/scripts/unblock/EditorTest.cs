using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitcode_blockout
{
    public class EditorTest : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
           

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            GameManager.getInstance().init();
            GameData.getInstance().isLock = false;

            //init game
            blockout tg = GameObject.Find("blockout").GetComponent<blockout>();
            tg.clear();

            GameData.instance.isTesting = true;
            //start game;
            tg.init();
        }


        void OnMouseDown()
        {
           

        }

        public void win()
        {
            GetComponent<MeshRenderer>().GetComponent<Renderer>().material.color = Color.red;
            GameData.instance.isWin = true;
        }
    }
}
