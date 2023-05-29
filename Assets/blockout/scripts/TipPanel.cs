using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Hitcode_blockout
{
    public class TipPanel : MonoBehaviour
    {

        // Use this for initialization
        bool canTick = true;
        Transform bg;
        void Start()
        {
            bg = transform.Find("bg");
        }

        private void OnEnable()
        {
            lb_notip = transform.Find("bg").Find("lb_notip").GetComponentInChildren<Text>();
            canTick = true;

            GameObject.Find("tipTitle").GetComponentInChildren<Text>().text = Localization.Instance.GetString("askTip");
            GameObject.Find("btnYes").GetComponentInChildren<Text>().text = Localization.Instance.GetString("btnyes");
            GameObject.Find("btnNo").GetComponentInChildren<Text>().text = Localization.Instance.GetString("btnno");
        }

        // Update is called once per frame
        int n = 0;

        void Update()
        {
            if (n < 20)
            {
                n++;
                return;
            }
            else
            {
                n = 0;
            }
   
            if (!canTick)
                return;


            if (GameData.instance.tipRemain > 0)
            {
                
                lb_notip.text = Localization.Instance.GetString("tipRemain") + GameData.instance.tipRemain ;
            }
            else
            {
                lb_notip.text = Localization.Instance.GetString("buyTip");
            }

            return;
            if (GameData.getInstance().tipRemain == 0)
            {

                //			DateTime tnow = System.DateTime.Now;
                TimeSpan ts = new TimeSpan(50, 0, 0, 0);
                DateTime dt2 = DateTime.Now.Subtract(ts);
                long tcTime = dt2.Ticks / 10000000;



                int tTimeLasts = (int)(tcTime - long.Parse(GameData.getInstance().tickStartTime));


                int secondRemain = 300 - tTimeLasts;
                if (secondRemain <= 0)
                {
                    secondRemain = 0;
                    //count of;
                    PlayerPrefs.SetInt("tipRemain", 1);
                    PlayerPrefs.SetString("tipStart", "0");
                    GameData.getInstance().tipRemain = 1;
                    GameData.getInstance().tickStartTime = "0";
                    GameData.getInstance().main.refreshView();
                    checkUI();
                    print("startrefresh");
                }

                //lb_notip.text = Localization.Instance.GetString("nextTip") + (secondRemain).ToString() + " seconds";
                

            }
        }
        //	public delegate void PanelChangedEventHandler();
        //	public event PanelChangedEventHandler showPanel;
        bool isShowed;
        bool canShow = true;
        public void showTipPanel()
        {
            GameManager.getInstance().playSfx("click");
            
            if (GameData.instance.isLock) return;
         
            
            showOrHideTipPanel();
            //GetComponent<Image>().raycastTarget = true;
        }

        bool isOpenStore;
        public void yesHandler()
        {
            if (!isShowed)
                return;
            GameManager.getInstance().playSfx("click");
            showOrHideTipPanel();

            if (GameData.instance.tipRemain > 0)
            {

                showTip();
            }
            else
            {
                //buy tip
                print("open store");
                isOpenStore = true;
            }
        }

        public void buyNow()
        {
            if (!isShowed)
                return;
            GameManager.getInstance().playSfx("click");
            showOrHideTipPanel();

            //buy tip
            print("open store");
            isOpenStore = true;
        }

        public void noHandler()
        {
            GameManager.getInstance().playSfx("click");
            showOrHideTipPanel();
        }





        public void OnShowCompleted()
        {
            // Add event handler code here
            //		print ("showOver");
            isShowed = true;
            canShow = true;
        }

        public void OnHideCompleted()
        {
            //		print ("hideOver");	
            isShowed = false;
            canShow = true;
            GameData.getInstance().isLock = false;
            GameObject.Find("btnRetry").GetComponent<Button>().interactable = true;

            //GetComponent<Image>().raycastTarget = false;
            if (isOpenStore)
            {
                GameData.instance.main.panelBuyCoinC.SetActive(true);
                GameData.instance.isLock = true;
                isOpenStore = false;
            }
        }

        Text lb_notip;
        Button btnYes, btnNo;
        void checkUI()
        {

            btnYes = bg.Find("btnYes").GetComponent<Button>();
            btnNo = bg.Find("btnNo").GetComponent<Button>();
            //		print (GameData.getInstance ().tipRemain + "remain");
            //if (GameData.getInstance().tipRemain == 0)
            //{

            //    lb_notip.enabled = true;
            //    btnYes.interactable = false;

            //}
            //else
            //{
            //    btnYes.interactable = true;
            //    lb_notip.enabled = false;
            //}
            //if (GameData.getInstance().isLock)
            //   Gameo.Find("btnRetryB").GetComponent<Button>().interactable = false;
        }
        float startX;
        public void showOrHideTipPanel()
        {
            if (!canShow)
                return;
            gameObject.SetActive(true);
            GameData.getInstance().tickStartTime = PlayerPrefs.GetString("tipStart", "0");
            // Add event handler code here
            if (!isShowed)
            {
                
                //bg.DOMoveX(0, .2f).SetEase(Ease.Linear).OnComplete(OnShowCompleted);
                ATween.MoveTo(bg.gameObject, ATween.Hash("islocal", false, "x", 0, "delay",0f, "easetype", ATween.EaseType.linear, "time", .2f, "OnComplete", (System.Action)(OnShowCompleted)));
                
                    startX = bg.transform.position.x;

                canShow = false;
                GameData.getInstance().isLock = true;
                //disable some UI;
                checkUI();

            }
            else
            {

                canShow = false;

                //transform.Find("bg").DOMoveX(startX, .2f).SetEase(Ease.Linear).OnComplete(OnHideCompleted);
                ATween.MoveTo(bg.gameObject, ATween.Hash("islocal", false, "x", startX, "delay", 0f, "easetype", ATween.EaseType.linear, "time", .2f, "OnComplete", (System.Action)(OnHideCompleted)));

            }


        }

        void showTip()
        {

       
            if (GameData.getInstance().tipRemain > 0)
            {
                GameData.getInstance().tipRemain--;
                PlayerPrefs.SetInt("tipRemain", GameData.getInstance().tipRemain);
                GameData.getInstance().main.refreshView();

                //have not give a tip
                //GameData.getInstance().tickStartTime = PlayerPrefs.GetString("tipStart", "0");
                //if (GameData.getInstance().tickStartTime == "0")
                //{
                //    canTick = false;
                //    //				long tcTime = System.DateTime.Now.Ticks;

                //    TimeSpan ts = new TimeSpan(50, 0, 0, 0);
                //    DateTime dt2 = DateTime.Now.Subtract(ts);
                //    //				print (dt2.Ticks/10000000/3600);
                //    long tcTime = dt2.Ticks / 10000000;

                //    PlayerPrefs.SetString("tipStart", tcTime.ToString());
                //    GameData.getInstance().tickStartTime = tcTime.ToString();
                //    //				print (tcTime+"tctime11");
                //    canTick = true;
                //}
            }

            //if (GameData.getInstance().tipRemain == 0)
            //{
            //    canTick = true;
            //}
            //else
            //{
            //    canTick = false;
            //}

            

            //tip is remove a block
            Transform tContainer = GameObject.Find("gridContainer").transform;
            foreach(Transform tblock in tContainer)
            {

                string tblockName = tblock.name;
                if (tblockName.Length < 3 && int.Parse(tblockName) != 1)
                {
                    if (tContainer.transform.childCount == 3)//only one tip removable block left
                    {
                        if (GameData.instance.tipRemain > 0)
                        {
                            //if no tip neccessary anymore(all block removede).Disable tip button to disable waste.
                            //if no tip left,not disable the button. click tip again would ask for buy
                            GameObject.Find("btnTip").GetComponent<Button>().interactable = false;
                        }
                    }

                    Destroy(tblock.gameObject);
                    int tx = Mathf.RoundToInt((tblock.transform.position.x - GameData.getInstance().startPos.x) / GameData.instance.tileWidth);
                    int ty = Mathf.RoundToInt((tblock.transform.position.y - GameData.getInstance().startPos.y) / GameData.instance.tileWidth);





                    switch (int.Parse(tblockName))//name is type
                    {
                        case 1:
                            break;
                        case 2:
                            BlockOutData.getInstance().blockState[tx, ty] = 0;
                            BlockOutData.getInstance().blockState[tx+1, ty] = 0;
                            break;
                        case 3:
                            BlockOutData.getInstance().blockState[tx, ty] = 0;
                            BlockOutData.getInstance().blockState[tx, ty + 1] = 0;
                            break;
                        case 4:
                            BlockOutData.getInstance().blockState[tx, ty] = 0;
                            BlockOutData.getInstance().blockState[tx + 1, ty] = 0;
                            BlockOutData.getInstance().blockState[tx + 2, ty] = 0;
                            break;
                        case 5:
                            BlockOutData.getInstance().blockState[tx, ty] = 0;
                            BlockOutData.getInstance().blockState[tx, ty + 1] = 0;
                            BlockOutData.getInstance().blockState[tx, ty + 2] = 0;
                            break;
                    }
                    break;
                }

                

            }

           
        }

    }
}
