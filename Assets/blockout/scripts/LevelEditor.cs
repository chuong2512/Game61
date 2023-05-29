using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Hitcode_blockout
{
    public class LevelEditor : MonoBehaviour
    {

        Text txtSize;
        GameObject container;

        public int gridSizeX = 5;
        public int gridSizeY = 5;

        public float frameWidth;//gameframe width;
        public float frameHeight;//gameframe height;
                                 // Use this for initialization
        GameObject chessBoard;
        InputField jsonField;

        public GameObject editorScene, testScene;
        void Start()
        {
            container = GameObject.Find("container");
            txtSize = GameObject.Find("txtSize").GetComponent<Text>();
            if (txtSize.text == "")
            {
                txtSize.text = "5";
            }


            jsonField = GameObject.Find("JsonField").GetComponent<InputField>();
            chessBoard = GameObject.Find("editorBoard");

            frameWidth = chessBoard.GetComponent<SpriteRenderer>().bounds.size.x;
            frameHeight = chessBoard.GetComponent<SpriteRenderer>().bounds.size.y;


            gridSizeX = int.Parse(txtSize.text);
            gridSizeY = int.Parse(txtSize.text);

            occupied = new int[gridSizeX, gridSizeY];

        }

        // Update is called once per frame
        void Update()
        {

        }


        public void plus()
        {
            int tSize = int.Parse(txtSize.text);
            if (tSize < 20)
            {
                tSize++;
                clearAll();
            }
            txtSize.text = tSize.ToString();
        }

        public void Minus()
        {
            int tSize = int.Parse(txtSize.text);
            if (tSize > 5)
            {
                tSize--;
                clearAll();
            }
            txtSize.text = tSize.ToString();
        }


        public void clearAll()
        {
            foreach (Transform tblock in container.transform)
            {
                Destroy(tblock.gameObject);
            }

            gridSizeX = int.Parse(txtSize.text);
            gridSizeY = int.Parse(txtSize.text);
            occupied = new int[gridSizeX, gridSizeY];

            recordData(true);
        }

        bool isTesting = true;
        public Text playText; 
        public void play()
        {
            if (isTesting)
            {
                if (tempLevel == null) return;

                testScene.SetActive(true);
                editorScene.SetActive(false);
                playText.text = "edit";
                isTesting = false;

                GameData.getInstance().isTesting = true;
                GameData.getInstance().testData = tempLevel;
            }
            else
            {
                testScene.SetActive(false);
                editorScene.SetActive(true);
                playText.text = "play";
                isTesting = true;
            }
        }

        public void save()
        {

        }

        public void load()
        {

        }

        public int[,] occupied;
        public GameObject newPickUp;







        public JSONNode tempLevel= null;
        public void recordData(bool isForce = false)
        {
            tempLevel = null;
            
            bool illeague = false;
            if (container.transform.childCount == 0)
            {
                jsonField.text = "no data";
                illeague = true;
            }



            JSONNode tExitNode = new JSONObject();
            ExitArea tExitArea = new ExitArea();



            JSONArray blockArr = new JSONArray();
            GameObject startBlock = null;
            int startCode = -1;
            foreach (Transform tchild in container.transform)
            {
                BlockNode tblockNode = new BlockNode();

                int tx = Mathf.RoundToInt((tchild.transform.position.x - chessBoard.transform.position.x) / frameWidth * gridSizeX);
                int ty = Mathf.RoundToInt((tchild.transform.position.y - chessBoard.transform.position.y) / frameHeight * gridSizeY);
                tblockNode.x = tx;
                tblockNode.y = ty;

                if (tx < 0 || tx >= gridSizeX || ty < 0 || ty >= gridSizeY)
                {
                    illeague = true;
                }

                switch (tchild.name)
                {
                    case "new_blocks1":
                        tblockNode.w = 2; tblockNode.h = 1;
                        tExitArea.x = gridSizeX;
                        tExitArea.y = ty;
                        tExitNode = tExitArea.SaveToJSON();
                        startBlock = tchild.gameObject;
                        startCode = blockArr.Count;
                        break;
                    case "new_blocks2":
                        tblockNode.w = 2; tblockNode.h = 1;
                        break;
                    case "new_blocks3":
                        tblockNode.w = 1; tblockNode.h = 2;
                        break;
                    case "new_blocks4":
                        tblockNode.w = 3; tblockNode.h = 1;
                        break;
                    case "new_blocks5":
                        tblockNode.w = 1; tblockNode.h = 3;
                        break;
                }
                blockArr.Add(tblockNode.SaveToJSON());


            }

            //put start block to the first
            if(startCode > 0)
            {
                JSONNode tJson = blockArr[startCode];
                JSONNode tStartJson = blockArr[0];
                blockArr[0] = tJson;
                blockArr[startCode] = tStartJson;
            }
          
               
    

            JSONNode tlevelData = new JSONObject();
            BlockLevel tblockLevel = new BlockLevel();
            tblockLevel.e = tExitNode;
            tblockLevel.w = gridSizeX;
            tblockLevel.h = gridSizeY;
            tblockLevel.b = blockArr;
            tlevelData = tblockLevel.SaveToJSON();

            if (startBlock == null)
            {
                illeague = true;
            }
            if (!illeague && !isForce)
            {
                jsonField.text = tlevelData.ToString();
                tempLevel = tlevelData;
            }
            else
            {
                jsonField.text = "level map is empty or without the startBlock";
            }
        }






    }
















    public class BlockNode
    {
        public int w = 0;
        public int h = 0;
        public int x = 0;
        public int y = 0;

        public JSONNode SaveToJSON()
        {
            JSONNode node = new JSONObject();
            node["x"] = x;
            node["y"] = y;
            node["w"] = w;
            node["h"] = h;
            return node;
        }

    }

    public class BlockLevel
    {
        public JSONArray b;
        public JSONNode e;
        public int w;
        public int h;

        public JSONNode SaveToJSON()
        {
            JSONNode node = new JSONObject();
            node["b"] = b;
            node["e"] = e;
            node["w"] = w;
            node["h"] = h;
            return node;
        }
    }




    public class ExitArea
    {
        public int x = 0;
        public int y = 0;

        public JSONNode SaveToJSON()
        {
            JSONNode node = new JSONObject();
            node["x"] = x;
            node["y"] = y;
            return node;
        }
    }










}
