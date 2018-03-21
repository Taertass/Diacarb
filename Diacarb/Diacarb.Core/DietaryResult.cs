using System;
using System.Text;

namespace Diacarb.Core
{
    public class DietaryResult : IDietaryResult
    {
        public string FoodName { get; set; }

        public double ServingQuantity { get; set; }

        public string ServingUnit { get; set; }

        public double Carbohydrates { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            double roundedCarbs = Math.Round(Carbohydrates, 0);
            string carbText = roundedCarbs == 1 ? "carb" : "carbs";

            sb.Append($"{ServingQuantity} ");
            sb.Append($"{ServingUnit} ");
            sb.Append($"of {FoodName} ");
            sb.Append($"are {roundedCarbs} {carbText}.");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
