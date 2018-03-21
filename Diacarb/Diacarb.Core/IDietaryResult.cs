using System;

namespace Diacarb.Core
{
    public interface IDietaryResult
    {
        double Carbohydrates { get; set; }
        string FoodName { get; set; }
        double ServingQuantity { get; set; }
        string ServingUnit { get; set; }
    }
}
