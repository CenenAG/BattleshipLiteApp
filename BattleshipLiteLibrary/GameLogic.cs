using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleshipLiteLibrary
{
    public static class GameLogic
    {
        public static int GetShotCount(PlayerInfoModel winner)
        {
            return winner.ShotGrid.Count(x => x.Status != GridSpotStatus.Empty);
        }

        public static bool IdentifyShotResult(PlayerInfoModel opponent, string row, int column)
        {
            bool result = false;
            var shipSpot = opponent.ShipLocations.Find(x => x.SpotLetter == row.ToUpper() && x.SpotNumber == column);
            if (shipSpot == null)
            {
                return result;
            }
            if (shipSpot.Status == GridSpotStatus.Ship)
            {
                shipSpot.Status = GridSpotStatus.Sunk;
                result = true;
            }
            return result;
        }

        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string> { "A", "B", "C", "D", "E" };
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };

            foreach (string letter in letters)
            {
                foreach (int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }
            }
        }

        public static void MarkShotResult(PlayerInfoModel player, string row, int column, bool isAHit)
        {
            GridSpotStatus status = isAHit ? GridSpotStatus.Hit : GridSpotStatus.Miss;
            var shotGrid = player.ShotGrid.Find(x => x.SpotLetter == row.ToUpper() && x.SpotNumber == column);
            shotGrid.Status = status;
        }

        public static bool PlayerStillActive(PlayerInfoModel player)
        {
            //If still have one ship not sunk
            return player.ShipLocations.Exists(x => x.Status != GridSpotStatus.Sunk);
        }

        public static (string row, int column) SplitShotRowAndColumn(string shot)
        {
            string row = "";
            int column = 0;

            if (shot.Length != 2)
            {
                Console.WriteLine("This was an invalid shot type");
                Console.WriteLine();
                row = "";
                column = 0;
                return ("", 0);
            }

            row = shot.Substring(0, 1).ToUpper();
            bool validColumn = int.TryParse(shot.Substring(1, 1), out column);
            if (!validColumn)
            {
                Console.WriteLine("This was an invalid shot number");
                //throw new ArgumentException("This was an invalid shot number", "shot number");
            }
            return (row, column);


        }

        public static bool ValidateShot(PlayerInfoModel player, string row, int column)
        {
            return player.ShotGrid.Exists(x => x.SpotLetter == row.ToUpper() && x.SpotNumber == column && x.Status == GridSpotStatus.Empty);
        }

        private static void AddGridSpot(PlayerInfoModel model, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel()
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Empty
            };

            model.ShotGrid.Add(spot);
        }

        public static bool PlaceShips(PlayerInfoModel model, string location)
        {
            bool output = false;
            (string row, int column) = SplitShotRowAndColumn(location);

            bool isValidLocation = ValidateGridLocation(model, row, column);
            bool isSpotOpen = ValidateShipLocation(model, row, column);

            if (isValidLocation && isSpotOpen)
            {
                model.ShipLocations.Add(new GridSpotModel
                {
                    SpotLetter = row,
                    SpotNumber = column,
                    Status = GridSpotStatus.Ship
                });
                output = true;
            }
            return output;
        }

        private static bool ValidateShipLocation(PlayerInfoModel model, string row, int column)
        {
            return !model.ShipLocations.Exists(x => x.SpotLetter == row.ToUpper() && x.SpotNumber == column);
        }

        private static bool ValidateGridLocation(PlayerInfoModel model, string row, int column)
        {
            return model.ShotGrid.Exists(x => x.SpotLetter == row.ToUpper() && x.SpotNumber == column);
        }
    }
}
