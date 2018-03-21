using System;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Diacarb.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Diacarb.Lambda
{
    public class Function
    {
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            string textResult = "";
            bool shouldEndSession = false;

            if (input.Request is LaunchRequest launchRequest)
            {
                textResult = "Try asking something like. How many carbs in 10 grams of strawberries";
            }
            else if (input.Request is IntentRequest intentRequest)
            {

                if (intentRequest?.Intent?.Name == "GetDiataryResultIntent")
                {
                    try
                    {
                        IDiacarbClient client = new DiacarbClient();

                        Slot servingUnit = null;
                        if (intentRequest.Intent.Slots.ContainsKey("servingUnit"))
                            servingUnit = intentRequest.Intent.Slots["servingUnit"];

                        Slot unit = null;
                        if (intentRequest.Intent.Slots.ContainsKey("unit"))
                            unit = intentRequest.Intent.Slots["unit"];

                        Slot food = null;
                        if (intentRequest.Intent.Slots.ContainsKey("food"))
                            food = intentRequest.Intent.Slots["food"];

                        string servingUnitText = servingUnit?.Value;
                        string foodText = food?.Value;
                        string unitText = unit?.Value;

                        IDietaryResult dietaryResult = client.GetDietaryResult(servingUnitText, unitText, foodText);
                        textResult = dietaryResult.ToString();
                    }
                    catch(ArgumentNullException ex)
                    {
                        if(ex.ParamName.ToLower() == "quantity")
                            textResult = "Please specify your quantity.";
                        else if (ex.ParamName.ToLower() == "unit")
                            textResult = "Please specify your quantity.";
                        else if (ex.ParamName.ToLower() == "foodname")
                            textResult = "Please specify your food.";
                    }
                    catch (Exception ex)
                    {
                        context.Logger.Log(ex.ToString());
                        textResult = "Oops something unexpected happened. Please try again.";
                    }

                    shouldEndSession = false;
                }
                else if (intentRequest?.Intent?.Name == "AMAZON.CancelIntent")
                {
                    shouldEndSession = true;
                    textResult = "Goodbye!";
                }
                else if (intentRequest?.Intent?.Name == "AMAZON.StopIntent")
                {
                    shouldEndSession = true;
                    textResult = "Goodbye!";
                }
                else if (intentRequest?.Intent?.Name == "AMAZON.HelpIntent")
                {
                    shouldEndSession = false;
                    textResult = "Try asking something like. How many carbs in 10 grams of strawberries";
                }
                else
                {
                    shouldEndSession = false;
                    textResult = "I could not understand you. Try asking something like. How many carbs in 10 grams of strawberries";
                }
            }
            else
            {
                shouldEndSession = false;
                textResult = "I could not understand you. Try asking something like. How many carbs in 10 grams of strawberries";
            }

            return new SkillResponse()
            {
                Version = "1.0",
                Response = new ResponseBody()
                {
                    ShouldEndSession = shouldEndSession,
                    OutputSpeech = new PlainTextOutputSpeech()
                    {
                        Text = textResult
                    }
                }
            };
        }
    }
}
