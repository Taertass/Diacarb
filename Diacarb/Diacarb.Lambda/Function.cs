using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>        
        //public string FunctionHandler(string input, ILambdaContext context)
        //{
        //    return "<speak>I works dude</speak>";
        //}

        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            if (input.Request is IntentRequest intentRequest)
            {
                if (intentRequest?.Intent?.Name == "GetDiataryResultIntent")
                {
                    string textResult = "";

                    try
                    {
                        Slot servingUnit = intentRequest.Intent.Slots["servingUnit"];
                        Slot unit = intentRequest.Intent.Slots["unit"];
                        Slot food = intentRequest.Intent.Slots["food"];

                        string queryText = $"{servingUnit.Value} {unit.Value} of {food.Value}";
                        context.Logger.Log(queryText);

                        IDiacarbClient client = new DiacarbClient();
                        Task<IDietaryResult> getDietaryResultTask = client.GetDietaryResultAsync(queryText);
                        IDietaryResult dietaryResult = getDietaryResultTask.Result;
                        textResult = dietaryResult.ToString();
                    }
                    catch (Exception ex)
                    {
                        context.Logger.Log(ex.ToString());
                        textResult = "Oops something unexpected happened";
                    }

                    return new SkillResponse()
                    {
                        Version = "1.0",
                        Response = new ResponseBody()
                        {
                            ShouldEndSession = false,
                            OutputSpeech = new PlainTextOutputSpeech()
                            {
                                Text = textResult
                            }
                        }
                    };
                }
                else
                {
                    return new SkillResponse()
                    {
                        Version = "1.0",
                        Response = new ResponseBody()
                        {
                            ShouldEndSession = false,
                            OutputSpeech = new PlainTextOutputSpeech()
                            {
                                Text = "I could not understand you. Try asking something like. How many carbs in 10 grams of strawberries"
                            }
                        }
                    };
                }
            }
            else
            {
                return new SkillResponse()
                {
                    Version = "1.0",
                    Response = new ResponseBody()
                    {
                        ShouldEndSession = false,
                        OutputSpeech = new PlainTextOutputSpeech()
                        {
                            Text = "Try asking something like. How many carbs in 10 grams of strawberries"
                        }
                    }
                };
            }
        }
    }
}
