using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hitcode_blockout
{
    public class BlockOnMouseDrag : MonoBehaviour
    {

        // Use this for initialization
        float startX = 0f;
        float startY = 0f;
      
        float myW, myH;
        int type = 0;
        Camera camera1;

        void Start()
        {
           

            startX = transform.position.x;
            startY = transform.position.y;

            type = int.Parse(name);
      

            camera1 = transform.root.GetComponent<Camera>();
            if (camera1 == null) camera1 = transform.root.GetComponentInChildren<Camera>();

        }

        // Update is called once per frame
        void Update()
        {

        }
        bool canMove = true;
        int min = 0; int max = 0;
        private Vector3 offset = Vector3.zero;
        int tx, ty;
        void OnMouseDown()
        {
            if (!canMove)
                return;

            if (GameData.getInstance().isLock) return;

            Vector3 worldPos = camera1.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = transform.position.z;
            offset = worldPos - transform.position;



            int tx = Mathf.RoundToInt((transform.position.x - GameData.getInstance().startPos.x) / GameData.instance.tileWidth);
            int ty = Mathf.RoundToInt((transform.position.y - GameData.getInstance().startPos.y) / GameData.instance.tileWidth);

      
            //clear origin data occupy first
            BlockOutData.Instance.setBlockState(type, tx, ty, 0);
            //check the max movable position;
            min = BlockOutData.Instance.getleft(int.Parse(name), tx, ty);

            //print("min"+min);
            max = BlockOutData.Instance.getright(int.Parse(name), tx, ty);
            //print("max" + max); 

        }

        void OnMouseUp()
        {
            if (!canMove)
                return;
            if (GameData.getInstance().isLock) return;
            offset = Vector3.zero;


            if (type % 2 == 0 || type == 1)//horizon blocks
            {

                //transform.DOMoveX(Mathf.RoundToInt((transform.position.x - startX) / GameData.instance.tileWidth) * GameData.instance.tileWidth + startX, .2f);
                ATween.MoveTo(gameObject, ATween.Hash("islocal", false, "x", Mathf.RoundToInt((transform.position.x - startX) / GameData.instance.tileWidth) * GameData.instance.tileWidth + startX, "delay", 0f, "easetype", ATween.EaseType.linear, "time", .2f, "OnComplete", (System.Action)(() => { })));

            }
            else//vertical blocks
            {
                //transform.DOMoveY(Mathf.RoundToInt((transform.position.y - startY) / GameData.instance.tileWidth) * GameData.instance.tileWidth + startY, .2f);
                ATween.MoveTo(gameObject, ATween.Hash("islocal", false, "y", Mathf.RoundToInt((transform.position.y - startY) / GameData.instance.tileWidth) * GameData.instance.tileWidth + startY, "delay", 0f, "easetype", ATween.EaseType.linear, "time", .2f, "OnComplete", (System.Action)(() => { })));

            }

            //set the settled block state
            int tx = Mathf.RoundToInt((transform.position.x - GameData.getInstance().startPos.x) / GameData.instance.tileWidth);
            int ty = Mathf.RoundToInt((transform.position.y - GameData.getInstance().startPos.y) / GameData.instance.tileWidth);


            BlockOutData.Instance.setBlockState(type, tx, ty, 1);


            if (type == 1)
            {

                if (tx == GameData.instance.exitPos.x - 2 && ty == GameData.instance.exitPos.y)
                {//hero block is on exit,//win;
                    canMove = false;
                    GameData.instance.isLock = true;
                    //transform.DOMoveX(transform.position.x + GameData.instance.tileWidth * 2+1, 2).OnComplete(() =>
                    ATween.MoveTo(gameObject, ATween.Hash("islocal", false, "x", transform.position.x + GameData.instance.tileWidth * 2 + 1, "delay", 0f, "easetype", ATween.EaseType.linear, "time", 2f, "OnComplete", (System.Action)(() =>
                    { 
                    if (GameData.instance.main != null)
                        {
                            GameData.instance.main.gameWin();
                        }
                        else
                        {
                            //here write your own win condition for your minigame
                            GameObject testCube = GameObject.Find("testCube");
                            if (testCube != null)
                            {
                                testCube.GetComponent<TestGame>().win();
                            }
                        }
                    })));
                }
            }


            for (int i = 0; i < GameData.instance.blockSizey; i++)
            {
                string tline = "";
                for (int j = 0; j < GameData.instance.blockSizey; j++)
                {
                    tline += BlockOutData.Instance.blockState[j, i];
                }
                //print(tline);
            }
        }


        void OnMouseDrag()
        {
            if (!canMove)
                return;
            if (GameData.getInstance().isLock) return;
            Vector3 point = camera1.ScreenToWorldPoint(Input.mousePosition);

            if (type % 2 == 0 || int.Parse(name) == 1)
            {//for horizon blocks

                point.y = startY;

                float tminx = min * GameData.instance.tileWidth + GameData.getInstance().startPos.x;
                float tmaxx = tminx + (max - min) * GameData.instance.tileWidth;//(BlockOutData.Instance.frameW / 6);


                float minx = Mathf.Min(tminx, tmaxx);
                float maxx = Mathf.Max(tminx, tmaxx);

                point.x = Mathf.Clamp(point.x - offset.x, minx, maxx);
            }
            else//for vertical blocks
            {
 
                point.x = startX;

                float tmin = Mathf.Min(min, max);
                float tmax = Mathf.Max(min, max);

                float tminy = GameData.getInstance().startPos.y + tmin * GameData.instance.tileWidth;
                float tmaxy = tminy + (tmax - tmin) * GameData.instance.tileWidth ;


                float miny2 = Mathf.Min(tminy, tmaxy);
                float maxy2 = Mathf.Max(tminy, tmaxy);


                point.y = Mathf.Clamp(point.y - offset.y, miny2, maxy2);
            }


            //point.z = Mathf.Clamp(point.z, -2.5f, 2.5f);
            transform.position = point;
        }



    }
}