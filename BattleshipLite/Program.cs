using BattleshipLiteLibrary;
using BattleshipLiteLibrary.Models;
using System;

namespace BattleshipLite
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, opponent);

                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinue == true)
                {
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else
                {
                    winner = activePlayer;
                }


            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine();
            Console.WriteLine($"Congratulations to {winner.UsersName} for winning!");
            Console.WriteLine($"{winner.UsersName} took {GameLogic.GetShotCount(winner)} shots. ");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;
            do
            {
                string shot = AskForShot();
                (row, column) = GameLogic.SplitShotRowAndColumn(shot);
                isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                if (isValidShot == false)
                {
                    Console.WriteLine("Invalid Shot Location. Please Try Again.");
                }

            } while (isValidShot == false);

            //Determine shot results
            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);

            //Record results
            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);
        }

        private static string AskForShot()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Please enter for shot selection: ");
            string output = Console.ReadLine();

            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            Console.Clear();
            Console.WriteLine($"Player {activePlayer.UsersName} ");
            Console.WriteLine();

            string currentRow = activePlayer.ShotGrid[0].SpotLetter;
            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }
                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber} ");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" X  ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" O  ");
                }
                else
                {
                    Console.Write(" ?  ");
                }
            }
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to Battleship Lite App");
            Console.WriteLine("Created by Zen Walker");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine($"Player Information for {playerTitle}");

            //Ask the user name
            output.UsersName = AskForUserName();

            //Load up the shot grid
            GameLogic.InitializeGrid(output);

            //Ask for the 5 ship placement
            PlaceShips(output);

            //Clear
            Console.Clear();

            return output;
        }

        private static string AskForUserName()
        {
            Console.Write("What is your Name?: ");
            string output = Console.ReadLine();
            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($"Where do you want to place ship number {model.ShipLocations.Count + 1}: ");
                string location = Console.ReadLine();

                
                bool isValidLocation = GameLogic.PlaceShips(model, location);

                if (isValidLocation == false)
                {
                    Console.WriteLine("That was not a valid location. Please try again.");
                }
            } while (model.ShipLocations.Count < 5);
        }




    }
}

