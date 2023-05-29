using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hitcode_blockout {
    public class DragInEditor : MonoBehaviour {

        Text txtSize;
        // Use this for initialization
        LevelEditor levelEditor;
        void Start() {
            txtSize = GameObject.Find("txtSize").GetComponent<Text>();
            chessBoard = GameObject.Find("editorBoard").GetComponent<SpriteRenderer>();
            placerect = GameObject.Find("placeRect");





            levelEditor = GameObject.Find("Canvas").GetComponent<LevelEditor>();

        }

        // Update is called once per frame
        void Update() {

        }


        private void OnMouseEnter()
        {
            levelEditor.gridSizeX = int.Parse(txtSize.text);
            levelEditor.gridSizeY = int.Parse(txtSize.text);
            if (container != null && container.transform.childCount == 0)
            {
                levelEditor.occupied = new int[levelEditor.gridSizeX, levelEditor.gridSizeY];
            }
        }


        private Vector3 screenPoint;
        private Vector3 offset;
        private Camera cam;




        public int myId;

        GameObject placerect;
        SpriteRenderer chessBoard;






        public List<Vector2> polyBounds;//record bound for each polygon
        public List<Vector2> currentStartPoses;//polygon's current start positions


        float zoomx = 1; float zoomy = 1;
        GameObject container;
        void OnMouseDown()
        {

            container = GameObject.Find("container");
            cam = GameObject.Find("gameCam").GetComponent<Camera>();
            screenPoint = cam.WorldToScreenPoint(gameObject.transform.position);

            offset = gameObject.transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

            if (gameObject.name.Contains("new_") == false)//not pick up a instance
            {
                GameObject tBlock1 = GameObject.Find("new_blocks1");
                if (tBlock1 == null)//blocks1 can only have one instance
                {
                    levelEditor.newPickUp = Instantiate(gameObject, container.transform);
                    levelEditor.newPickUp.name = "new_" + gameObject.name;
                }
                else
                {
                    if (gameObject.name != "blocks1")
                    {
                        levelEditor.newPickUp = Instantiate(gameObject, container.transform);
                        levelEditor.newPickUp.name = "new_" + gameObject.name;
                    }
                    else
                    {
                        levelEditor.newPickUp = null;
                        return;
                    }

                }
            }
            else
            {
                levelEditor.newPickUp = gameObject;
            }

            levelEditor.newPickUp.transform.position = transform.position;


            //set newpickup to the right size
            float bW = levelEditor.newPickUp.GetComponent<SpriteRenderer>().bounds.size.x;
            float bH = levelEditor.newPickUp.GetComponent<SpriteRenderer>().bounds.size.y;




            switch (levelEditor.newPickUp.name)
            {
                case "new_blocks1":
                    zoomx = 2; zoomy = 1;
                    break;
                case "new_blocks2":
                    zoomx = 2; zoomy = 1;
                    break;
                case "new_blocks3":
                    zoomx = 1; zoomy = 2;
                    break;
                case "new_blocks4":
                    zoomx = 3; zoomy = 1;
                    break;
                case "new_blocks5":
                    zoomx = 1; zoomy = 3;
                    break;
            }


            levelEditor.newPickUp.transform.localScale = new Vector3(zoomx * levelEditor.newPickUp.transform.localScale.x * (levelEditor.frameWidth / levelEditor.gridSizeX) / bW,
                zoomy * levelEditor.newPickUp.transform.localScale.y * (levelEditor.frameHeight / levelEditor.gridSizeY) / bH);
            levelEditor.newPickUp.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -.02f);
            //newPickUp.transform.Translate(toffset2);


            //set flashred to the same size;
            float rectW = placerect.GetComponent<SpriteRenderer>().bounds.size.x;
            float rectH = placerect.GetComponent<SpriteRenderer>().bounds.size.y;




            placerect.transform.localScale = new Vector3(zoomx * placerect.transform.localScale.x * (levelEditor.frameWidth / levelEditor.gridSizeX) / rectW,
                zoomy * placerect.transform.localScale.y * (levelEditor.frameHeight / levelEditor.gridSizeY) / rectH);



            getCurrentGrid(levelEditor.newPickUp);


            setCurrentGrid(levelEditor.newPickUp, 0);//clear occupied grid

        }

        void OnMouseDrag()
        {
            if (levelEditor.newPickUp == null) return;
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            Vector3 curPosition = cam.ScreenToWorldPoint(curScreenPoint) + offset;
            levelEditor.newPickUp.transform.position = new Vector3(curPosition.x, curPosition.y, -.02f);

            getCurrentGrid(levelEditor.newPickUp.gameObject);
        }


        private void OnMouseUp()
        {

            if (levelEditor.newPickUp == null) return;
            //placerect.SetActive(false);
            placerect.GetComponent<SpriteRenderer>().enabled = false;

            if (!canplace)
            {

                placed = false;
                DestroyImmediate(levelEditor.newPickUp);
                setCurrentGrid(levelEditor.newPickUp, 0);//clear its occupy
                placerect.transform.position = new Vector3(-999999, 0, 0);
                levelEditor.newPickUp = null;
            }
            else
            {



                bool tisOccupied = false;

                tisOccupied = checkIntersection();//the most complicate algorithm for polygon intersection dectection




                //can place down
                if (!tisOccupied)
                {
                    levelEditor.newPickUp.transform.position = placerect.transform.position;
                    //GetComponent<BoxCollider2D>().enabled = false;
                    placed = true;

                    setCurrentGrid(levelEditor.newPickUp, 1);//set occupied grid
                }
                else
                {
                    placed = false;
                    DestroyImmediate(levelEditor.newPickUp);
                    setCurrentGrid(levelEditor.newPickUp, 0);//clear occupied grid
                    placerect.transform.position = new Vector3(-999999, 0, 0);
                    levelEditor.newPickUp = null;
                }
            }

            levelEditor.recordData();
        }


        bool placed = false;
        public bool canplace = false;

        Vector2 cGridPos;

        public void getCurrentGrid(GameObject gameObject)
        {

            int tx = Mathf.RoundToInt((gameObject.transform.position.x - chessBoard.transform.position.x) / levelEditor.frameWidth * levelEditor.gridSizeX);
            int ty = Mathf.RoundToInt((gameObject.transform.position.y - chessBoard.transform.position.y) / levelEditor.frameHeight * levelEditor.gridSizeY);


            cGridPos = new Vector2(tx, ty);

            //print(tx + "__" + ty + "_______");






            if (cGridPos.x >= 0 && cGridPos.y >= 0 && cGridPos.x <= (levelEditor.gridSizeX - zoomx) && cGridPos.y <= (levelEditor.gridSizeY - zoomy))
            {
                //placerect.SetActive(true);
                placerect.GetComponent<SpriteRenderer>().enabled = true;
                canplace = true;
            }
            else
            {
                //placerect.SetActive(false);
                placerect.GetComponent<SpriteRenderer>().enabled = false;
                canplace = false;
            }



            placerect.transform.position = new Vector3(chessBoard.transform.position.x + tx * (levelEditor.frameWidth / levelEditor.gridSizeX),
                chessBoard.transform.position.y + ty * (levelEditor.frameHeight / levelEditor.gridSizeY));





        }


        void setCurrentGrid(GameObject gameObject, int value)
        {


            if (cGridPos.x >= 0 && cGridPos.y >= 0 && cGridPos.x <= (levelEditor.gridSizeX - zoomx) && cGridPos.y <= (levelEditor.gridSizeY - zoomy))
            {
                if (levelEditor.newPickUp == null) return;
                switch (levelEditor.newPickUp.name)
                {
                    case "new_blocks1":
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] = value;
                        levelEditor.occupied[(int)cGridPos.x + 1, (int)cGridPos.y] = value;
                        //print(cGridPos.x + "___" + cGridPos.y);
                        break;
                    case "new_blocks2":
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] = value;
                        levelEditor.occupied[(int)cGridPos.x + 1, (int)cGridPos.y] = value;
                        break;
                    case "new_blocks3":
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] = value;
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y + 1] = value;
                        break;
                    case "new_blocks4":
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] = value;
                        levelEditor.occupied[(int)cGridPos.x + 1, (int)cGridPos.y] = value;
                        levelEditor.occupied[(int)cGridPos.x + 2, (int)cGridPos.y] = value;
                        break;
                    case "new_blocks5":
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] = value;
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y + 1] = value;
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y + 2] = value;
                        break;
                }
            }
        }

        bool checkIntersection()
        {

            bool isIntersected = false;
            int value = 0;
            if (cGridPos.x >= 0 && cGridPos.y >= 0 && cGridPos.x <= (levelEditor.gridSizeX - zoomx) && cGridPos.y <= (levelEditor.gridSizeY - zoomy))
            {
                switch (levelEditor.newPickUp.name)
                {
                    case "new_blocks1":
                        if (levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] != value ||
                        levelEditor.occupied[(int)cGridPos.x + 1, (int)cGridPos.y] != value)
                        {
                            isIntersected = true;
                        }
                        break;
                    case "new_blocks2":
                        if (levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] != value ||
                        levelEditor.occupied[(int)cGridPos.x + 1, (int)cGridPos.y] != value)
                        {
                            isIntersected = true;
                        }
                        break;
                    case "new_blocks3":
                        if (levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] != value ||
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y + 1] != value)
                        {
                            isIntersected = true;
                        }
                        break;
                    case "new_blocks4":
                        if (levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] != value ||
                        levelEditor.occupied[(int)cGridPos.x + 1, (int)cGridPos.y] != value ||
                        levelEditor.occupied[(int)cGridPos.x + 2, (int)cGridPos.y] != value)
                        {
                            isIntersected = true;
                        }
                        break;
                    case "new_blocks5":
                        if (levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y] != value ||
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y + 1] != value ||
                        levelEditor.occupied[(int)cGridPos.x, (int)cGridPos.y + 2] != value)
                        {
                            isIntersected = true;
                        }
                        break;
                }

            }
            return isIntersected;
        }


       
    }
}