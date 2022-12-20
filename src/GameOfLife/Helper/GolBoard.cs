using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class GolBoard
    {

        public Coord2D Amount;

        public int AmountOfCells => Amount.X * Amount.Y;


        public GolBoard()
        {
            Amount = new Coord2D();
        }


        public void SetBoardWidth(byte width)
        {
            Amount.X = width;
        }

        public void SetBoardHeight(byte height)
        {
            Amount.Y = height;
        }


        //public List<Coord2D> RandomFill2(int percentage)
        //{

        //    if(percentage <= 0 || percentage >= 100)
        //    {
        //        percentage = 30;
        //    }


        //    List<Coord2D> randLife = new List<Coord2D>();

        //    Random random = new Random();

        //    for (byte x = 0; x < Amount.X; x++)
        //    {
        //        for (byte y = 0; y < Amount.Y; y++)
        //        {
        //            int rand = random.Next(0, 100);

        //            if (rand <= percentage)
        //            {
        //                randLife.Add(new Coord2D(x, y));
        //                //Squares[new Coord2D(x, y)].Fill = GolBrush.Life;
        //            }
        //        }
        //    }

        //    return randLife;
        //}


        public IEnumerable<Coord2D> RandomCells(double percentage)
        {

            HashSet<Coord2D> randLife = new HashSet<Coord2D>();

            double m = AmountOfCells * (percentage / 100);

            int n = 0;

            Random random = new Random();

            while (n < m)
            {
                int randX = random.Next(0, Amount.X - 1);
                int randY = random.Next(0, Amount.Y - 1);

                bool res = randLife.Add(new Coord2D((byte)randX, (byte)randY));

                if (res)
                {
                    n++;
                }
            }

            return randLife;
        }

    }
}