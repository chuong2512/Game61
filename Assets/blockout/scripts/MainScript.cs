using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace Hitcode_blockout
{
    public class MainScript : MonoBehaviour
    {
        // Use this for initialization

        //data
        public int timeCount = 120; //how much time you left.The more time,the better score

        Text timeTxt; //the text ui showes the count down time text

        //fix position bug
        Vector3[] corners = new Vector3[4];
        float moveX;

        void Start()
        {
            refreshView();
            //StartCoroutine("waitAsecond");

            //fadeOut ();
            all_game = GameObject.Find("all_game");
            all_level = GameObject.Find("all_level");
            //fix position bug
            all_game.transform.parent.GetComponent<RectTransform>().GetLocalCorners(corners);
            moveX = (corners[3] - corners[0]).x;

            if (all_level != null) //only when level menu loaded
            {
                float tx = GameObject.Find("Back_UI_Cam").GetComponent<Camera>()
                    .ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x * 2;


                all_game.transform.localPosition = new Vector3(moveX, all_game.transform.localPosition.y, 0);
            }
            else
            {
                init(); //for test
            }

            GameData.instance.main = this;

            StartCoroutine("nextframe");

            //gameWin();
        }

        GameObject all_level; //levelmenu container
        GameObject all_game;

        IEnumerator nextframe()
        {
            yield return new WaitForEndOfFrame();
        }


        public void init()
        {
            GameData.getInstance().currentScene = 2;
            //GameObject.Find("shikaku").GetComponent<Shikaku>().initSingleMode();
            GameObject.Find("blockout").GetComponent<blockout>().init();

            refreshView();
        }


        /// <summary>
        /// check if win every second
        /// </summary>
        /// <returns>The asecond.</returns>
        IEnumerator waitAsecond()
        {
            yield return new WaitForSeconds(1);

            if (timeCount > 0)
            {
                if (GameData.getInstance().isWin == false)
                {
                    timeCount--;
                    StartCoroutine("waitAsecond");
                    timeTxt.text = timeCount.ToString();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// Refreshs the view.
        /// </summary>
        public void refreshView()
        {
            GameObject.Find("lb_level").GetComponent<Text>().text = (GameData.getInstance().cLevel + 1).ToString();
            //tip
            //GameData.instance.isLock = false;

            timeTxt = GameObject.Find("timeTxt").GetComponent<Text>();


            GameObject tIns = GameObject.Find("lb_ins");
            if (tIns != null)
            {
                tIns.GetComponent<Text>().text = Localization.Instance.GetString("lb_ins");
            }
        }


        //handler event
        public void OnRetryClick()
        {
            if (GameData.instance.isLock) return;
            // Add event handler code here
            GameManager.getInstance().playSfx("click");
            GameObject.Find("blockout").GetComponent<blockout>().clear(true);
            GameObject btnTip = GameObject.Find("btnTip");
            if (btnTip)
            {
                btnTip.GetComponent<Button>().interactable = true;
            }
        }

        public GameObject panelWin; //win panel gameobject
        public GameObject panelBuyCoinC;
        WinPanel winpanel; //winpanel controller

        /// <summary>
        /// when game wins.
        /// </summary>
        /// 
        public void gameWin()
        {
            //fire win event

            GameManager.getInstance().playSfx("win");
            //if (GameData.getInstance().cLevel % 5 == 0 && GameData.getInstance().cLevel > 0)
            //{

            //}
            //if(musicScript.nTick == 0)
            //{
            print(GameData.instance.cLevel + "clevel");
            if (GameData.instance.cLevel > 0 && GameData.instance.cLevel % 2 == 1)
            {
                GameManager.getInstance().showInterestitial();
            }


            GameData.getInstance().isWin = true;
            int threeStar = 10;
            int twoStar = threeStar + 5; //
            int oneStar = threeStar + 20; //

            int starGet = 0;
            if ((120 - timeCount) <= threeStar)
            {
                starGet = 3;
            }
            else if ((120 - timeCount) > threeStar && (120 - timeCount) <= twoStar)
            {
                starGet = 2;
            }
            else if ((120 - timeCount) > twoStar && (120 - timeCount) <= oneStar)
            {
                starGet = 1;
            }
            else
            {
                starGet = 0;
            }


            panelWin.SetActive(true);

            winpanel = panelWin.GetComponent<WinPanel>();
            winpanel.showHidePanel(starGet);


            if (GameData.instance.cLevel >= GameData.instance.levelPass[GameData.difficulty])
            {
                PlayerPrefs.SetInt("levelPassed" + GameData.difficulty, GameData.instance.cLevel + 1);
                GameData.instance.levelPass[GameData.difficulty] = GameData.instance.cLevel + 1;
            }


            if (GameData.instance.levelStates != null)
            {
                GameData.instance.levelStates[GameData.difficulty][GameData.instance.cLevel] = 1;
                PlayerPrefs.SetInt("blockout_" + GameData.difficulty + "_" + GameData.instance.cLevel, 1);


                //save score
                int tallScore = 0;
                for (int i = 0; i < GameData.totalLevel.Length; i++)
                {
                    for (int j = 0; j < GameData.totalLevel[i]; j++)
                    {
                        if (GameData.instance.levelStates[i][j] == 1)
                        {
                            tallScore++;
                        }
                    }
                }


                GameData.getInstance().bestScore = tallScore;
            }

            GameManager.getInstance().submitGameCenter();
        }

        /// <summary>
        /// deal Button clicks handler.
        /// </summary>
        /// <param name="control">Control.</param>
        public void buttonHandler(GameObject control)
        {
            GameManager.getInstance().playSfx("click");
            switch (control.name)
            {
                case "btnMain":
                    fadeIn("MainMenu");
                    break;
                case "btnMenu":
                    fadeIn("LevelMenu");
                    break;
            }
        }

        public void loadMainScene()
        {
            GameObject.Find("particle").SetActive(false);

            SceneManager.LoadScene("Scenes/Menu");
        }

        public void loadLevelScene()
        {
            if (GameData.instance.isLock) return;

            GameManager.getInstance().playSfx("click");
            all_level = GameObject.Find("all_level");
            if (all_level != null)
            {
                GameData.instance.isLock = true; //fix position bug


                //fix postion bug
                Vector3 fixedPosition1 = new Vector3(moveX, all_game.transform.localPosition.y, 0);
                Vector3 fixedPosition2 = new Vector3(0, all_level.transform.localPosition.y, 0);


                //all_level.transform.parent.GetComponent<LevelMenu>().refreshLevel();//uncomment this if you want to choose level difficulty after cancel a game
                all_level.transform.parent.GetComponent<LevelMenu>().refreshView();
                //all_level.transform.DOLocalMove(fixedPosition2, 1f).SetDelay(.4f).SetEase(Ease.OutBounce).OnComplete(() =>
                ATween.MoveTo(all_level, ATween.Hash("islocal", true, "position", fixedPosition2, "delay", .4f,
                    "easetype", ATween.EaseType.easeOutBounce, "time", 1f, "OnComplete", (System.Action) (() =>
                    {
                        GameData.instance.currentScene = 1;

                        //fix position bug
                        all_game.transform.localPosition = fixedPosition1;
                        all_level.transform.localPosition = fixedPosition2;
                    })));


                all_game = GameObject.Find("all_game");
                if (all_game != null)
                {
                    GameData.instance.isLock = true;
                    //float tx = GameObject.Find("Back_UI_Cam").GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x * 2;
                    //all_game.transform.DOLocalMove(fixedPosition1, 1f).SetDelay(.4f).SetEase(Ease.OutBounce).OnComplete(() =>
                    ATween.MoveTo(all_game,
                        ATween.Hash("islocal", true, "position", fixedPosition1, "delay", .4f, "easetype",
                            ATween.EaseType.easeOutBounce, "time", 1f, "OnComplete",
                            (System.Action) (() => { GameData.instance.isLock = false; })));

                    GameObject.Find("blockout").GetComponent<blockout>().clear();
                }

                //print("islock11" + GameData.instance.isLock);
            }
            else
            {
                //GameObject.Find("particle").SetActive(false);

                SceneManager.LoadScene("Scenes/Menu");
            }
        }

        //void level2MainComplete(Vector3 postion1,Vector3 postion2)
        //{
        //    {
        //        GameData.instance.currentScene = 1;

        //        //fix position bug
        //        all_game.transform.localPosition = fixedPosition1;
        //        all_level.transform.localPosition = fixedPosition2;
        //    };
        //}

        /// <summary>
        /// camera fade out
        /// </summary>
        public Image mask;

        void fadeOut()
        {
            mask.gameObject.SetActive(true);
            mask.color = Color.black;
            ATween.ValueTo(mask.gameObject,
                ATween.Hash("from", 1, "to", 0, "time", 1, "onupdate", "OnUpdateTween", "onupdatetarget",
                    this.gameObject, "oncomplete", "fadeOutOver", "oncompletetarget", this.gameObject));
        }

        /// <summary>
        /// camera fade in
        /// </summary>
        /// <param name="sceneName">Scene name.</param>
        void fadeIn(string sceneName)
        {
            SceneManager.LoadScene("Scenes/Menu");
        }


        void fadeInOver(string sceneName)
        {
            SceneManager.LoadScene("Scenes/Menu");
        }

        void fadeOutOver()
        {
            mask.gameObject.SetActive(false);
        }


        /// <summary>
        /// tween update event
        /// </summary>
        /// <param name="value">Value.</param>
        void OnUpdateTween(float value)

        {
            mask.color = new Color(0, 0, 0, value);
        }
    }
}