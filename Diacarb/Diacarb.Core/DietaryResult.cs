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

            sb.Append($"{ServingQuantity} ");
            sb.Append($"{ServingUnit} ");
            sb.Append($"of {FoodName} ");
            sb.Append($"are {Carbohydrates} carbs.");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
