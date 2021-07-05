using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleApp13
{
    class Program
    {
        private static int YHeight = 24;
        private static int XWidth = 21;
        private static float FrameDuration = .02f;
        private static bool IsComplete = false;

        static void Main(string[] args)
        {
            Console.SetWindowSize(XWidth, YHeight);
            char[,] HourGlass = GetInitialHourGlassState();
            RenderHourglass(HourGlass);
            Thread.Sleep(TimeSpan.FromSeconds(FrameDuration));
            Console.Read();
            while (!IsComplete)
            {
                HourGlass = SimulateSandMovementFromBottomUp(HourGlass);
                RenderHourglass(HourGlass);
                Thread.Sleep(TimeSpan.FromSeconds(FrameDuration));
            }              
        }

        private static char[,] GetInitialHourGlassState()
        {
            var init = Hourglass.InitialCells;
            var hourGlass = new char[XWidth, YHeight];

            for (int y = 0; y < init.Length; y++)
            {
                var row = init[y].ToCharArray();
                for (int x = 0; x < row.Length; x++)
                {
                    hourGlass[x, y] = row[x];
                }
            }
            return hourGlass;
        }

        static char[,] SimulateSandMovementFromBottomUp(char[,] hourGlass)
        {
            var random = new Random();
            int movementThisFrame = 0;
            //Start from bottom layer and loop upwards until top layer
            for (int y = YHeight - 1; y > 0; y--)
            {
                List<int> SandInThisRow = new List<int>();
               
                for (int x = 0; x < XWidth - 1; x++)
                {
                    SandInThisRow.Add(x);                  
                }

                while(SandInThisRow.Count > 0)
                {
                    var x = random.Next(0, SandInThisRow.Count());
                    SandInThisRow.RemoveAt(x);

                    if (Hourglass.CellTypes[hourGlass[x, y]] == CellType.Sand)
                    {
                        var nextMove = GetNextMoveForSand(x, y, hourGlass);
                        switch (nextMove)
                        {
                            case MovementThisFrame.FallDown:
                                hourGlass[x, y] = ' ';
                                hourGlass[x, y + 1] = '*';
                                
                                movementThisFrame++;
                                break;
                            case MovementThisFrame.FallDown_Left:
                                hourGlass[x, y] = ' ';
                                hourGlass[x - 1, y + 1] = '*';
                                movementThisFrame++;
                                break;
                            case MovementThisFrame.FallDown_Right:
                                hourGlass[x, y] = ' ';
                                hourGlass[x + 1, y + 1] = '*';
                                movementThisFrame++;
                                break;
                            case MovementThisFrame.None:
                                break;
                            default:
                                break;
                        }
                    }                       
                }
            }

            //if (movementThisFrame == 0)
            //    IsComplete = true;
            return hourGlass;
        }

        static MovementThisFrame GetNextMoveForSand(int x, int y, char[,] hourGlass)
        {
            //Check if grain of sand can fall downward
            if (Hourglass.CellTypes[hourGlass[x, y + 1]] == CellType.Empty)
            {
                return MovementThisFrame.FallDown;
            }

            //Dont have to do out-of-bounds checks here because the sand is always kept 1 or more units away 
            //from the end of the array.
            bool canFallDownLeft = Hourglass.CellTypes[hourGlass[x - 1, y + 1]] == CellType.Empty;
            bool canFallDownRight = Hourglass.CellTypes[hourGlass[x + 1, y + 1]] == CellType.Empty;

            if(canFallDownLeft && canFallDownRight)
            {
                var random = new Random();
                var tieBreaker = random.Next(2);
                if (tieBreaker == 0)
                    return MovementThisFrame.FallDown_Left;
                return MovementThisFrame.FallDown_Right;
            }

            if (canFallDownLeft)
                return MovementThisFrame.FallDown_Left;

            if (canFallDownRight)
                return MovementThisFrame.FallDown_Right;

            return MovementThisFrame.None;
        }

        static void RenderHourglass(char[,] hourGlass)
        {
            Console.Clear();
            for (int y = 0; y < hourGlass.GetLength(1); y++)
            {
                for (int x = 0; x < hourGlass.GetLength(0); x++)
                {                
                    Console.Write(hourGlass[x, y]);                   
                }
                if(y < hourGlass.GetLength(1) - 1)
                Console.WriteLine();
            }          
        }
    }

    public class Hourglass
    {

        public static string[] InitialCells = new string[]
        {
             @"+8-=-=-=-=-=-=-=-=-8+",
             @"|.,.-'''''''''''-.,.|",
             @"|/*****************\|",
             @"|\*****************/|",
             @"|.\***************/.|",
             @"|..\*************/..|",
             @"|...\***********/...|",
             @"|....\*********/....|",
             @"|.....\*******/.....|",
             @"|......\*****/......|",
             @"|.......\***/.......|",
             @"|........\*/........|",
             @"|......../ \........|",
             @"|......./   \.......|",
             @"|....../     \......|",
             @"|...../       \.....|",
             @"|..../         \....|",
             @"|.../           \...|",
             @"|../             \..|",
             @"|./               \.|",
             @"|/                 \|",
             @"|\                 /|",
             @"|.'--___________--'.|",
             @"+8-=-=-=-=-=-=-=-=-8+"
        };


        public static Dictionary<char, CellType> CellTypes = new Dictionary<char, CellType> {
        { '/', CellType.Glass },
        { '|', CellType.Wood },
        { '\\', CellType.Glass },
        { '-', CellType.Glass },
         { ',', CellType.Glass },
        { '\'', CellType.Glass },
        { '_', CellType.Glass },
        { '*', CellType.Sand },
        { ' ', CellType.Empty },
        { '.', CellType.Glass },
        { '=', CellType.Wood },
        { '+', CellType.Wood },
        { '8', CellType.Wood }
};
    }



    public enum CellType { Wall, Empty, Sand, Outside, Glass, Wood }
    public enum MovementThisFrame { FallDown, FallDown_Left, FallDown_Right, None}
}
