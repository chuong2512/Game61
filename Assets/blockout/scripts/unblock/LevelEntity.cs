using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hitcode_blockout
{
    public class LevelEntity
    {

        public LevelEntity()
        {
            pieces = new List<Piece>();
        }



        // Update is called once per frame
        void Update()
        {

        }


        public int minMoves;
        public List<Piece> pieces;

       

    }

    public class Piece
    {
        public int x = 0;
        public int y = 0;
        public int pType = 0;


         public int _w;
        public int _h;
        public int _x;
        public int _y;
    }
}