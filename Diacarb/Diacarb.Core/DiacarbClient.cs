using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diacarb.Core
{
    public interface IDietaryResult
    {
        double Carbohydrates { get; set; }
        string FoodName { get; set; }
        double ServingQuantity { get; set; }
        string ServingUnit { get; set; }
    }
    public class DietaryResult : IDietaryResult
    {
        public string FoodName { get; set; }

        public double ServingQuantity { get; set; }

        public string ServingUnit { get; set; }

        public double Carbohydrates { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{nameof(FoodName)}: {FoodName}");
            sb.AppendLine($"{nameof(ServingQuantity)}: {ServingQuantity}");
            sb.AppendLine($"{nameof(ServingUnit)}: {ServingUnit}");
            sb.AppendLine($"{nameof(Carbohydrates)}: {Carbohydrates}");

            return sb.ToString();
        }
    }

    public interface IDiacarbClient
    {
        Task<IDietaryResult> GetDietaryResultAsync(string query);
    }

    public class DiacarbClient : IDiacarbClient
    {
        private const string myApiId = "fcaa3e2b";
        private const string myApiKey = "2cddeb33b2becc27a6f1b66f5268f257";

        public async Task<IDietaryResult> GetDietaryResultAsync(string query)
        {
            IDietaryResult dietaryResult = null;

            var payload = new
            {
                query = query,
                timezone = "US/Eastern"
            };


            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://trackapi.nutritionix.com/v2/natural/nutrients"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(stringPayload, Encoding.UTF8, "application/json")
                };

                
                //request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("x-app-id", myApiId);
                request.Headers.Add("x-app-key", myApiKey);

                var response = await httpClient.SendAsync(request);
                //var jsonTask = response.Content;


                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

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
    }
}
