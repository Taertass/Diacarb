using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diacarb.Core
{
    public class DiacarbClient : IDiacarbClient
    {
        private const string myApiId = "fcaa3e2b";
        private const string myApiKey = "2cddeb33b2becc27a6f1b66f5268f257";

        public async Task<IDietaryResult> GetDietaryResultAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            IDietaryResult dietaryResult = null;

            string stringPayload = await Task.Run(() => JsonConvert.SerializeObject(new
            {
                query = query,
                timezone = "US/Eastern"
            }));

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            StringContent httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://trackapi.nutritionix.com/v2/natural/nutrients"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(stringPayload, Encoding.UTF8, "application/json")
                };

                //Add the app id and key
                request.Headers.Add("x-app-id", myApiId);
                request.Headers.Add("x-app-key", myApiKey);

                HttpResponseMessage response = await httpClient.SendAsync(request);
                
                if (response.Content != null)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    JObject jsonResult = JObject.Parse(responseContent);
                    string message = (string)jsonResult["message"];

                    if (!string.IsNullOrWhiteSpace(message))
                        throw new Exception(message);

                    JArray foods = jsonResult["foods"] as JArray;
                    JToken foodResult = foods[0];

                    string foodName = (string)foodResult["food_name"];
                    double servingQuantity = (double)foodResult["serving_qty"];
                    string servingUnit = (string)foodResult["serving_unit"];
                    double totalCarbohydrate = (double)foodResult["nf_total_carbohydrate"];
                    double dietaryFiber = (double)foodResult["nf_dietary_fiber"];

                    dietaryResult = new DietaryResult()
                    {
                        FoodName = foodName,
                        ServingUnit = servingUnit,
                        ServingQuantity = servingQuantity,
                        Carbohydrates = totalCarbohydrate - dietaryFiber
                    };
                }
            }

            return dietaryResult;
        }
        
        public Task<IDietaryResult> GetDietaryResultAsync(string quantity, string unit, string foodName)
        {
            if (string.IsNullOrWhiteSpace(quantity))
                throw new ArgumentNullException(nameof(quantity));
            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentNullException(nameof(unit));
            if (string.IsNullOrWhiteSpace(foodName))
                throw new ArgumentNullException(nameof(foodName));

            //Convert unit from deciliters to milliliters
            if (unit.ToLower().Contains("deciliter"))
            {
                double parsedUnit = 0;
                if (double.TryParse(quantity, out parsedUnit))
                {
                    parsedUnit = parsedUnit * 100;

                    quantity = parsedUnit.ToString();
                    unit = "milliliters";
                }
            }

            string queryText = $"{quantity} {unit} of {foodName}";
            return GetDietaryResultAsync(queryText);
        }

        public IDietaryResult GetDietaryResult(string quantity, string unit, string foodName)
        {
            return GetDietaryResultAsync(quantity, unit, foodName).Result;
        }
    }
}
