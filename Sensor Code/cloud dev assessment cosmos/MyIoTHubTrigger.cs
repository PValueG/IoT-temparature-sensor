using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace my.uni
{
  

    public class TemparaturesItem
  {
    [JsonProperty("id")]
    public string Id {get; set;}
    public double Temperature {get; set;}
    public double Humidity {get; set;}

    public double HeartRate {get; set;}
  }
    public class MyIoTHubTrigger
    {
        private static HttpClient client = new HttpClient();
        
        [FunctionName("myIoTHubTrigger")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "AzureEventHubConnectionString")] EventData message,
        [CosmosDB(databaseName: "IoTdata",
                                 collectionName: "Temparatures",
                                 ConnectionStringSetting = "cosmosDBConnectionString")] out TemparaturesItem output,
                       ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");


        var jsonBody = Encoding.UTF8.GetString(message.Body);
        dynamic data = JsonConvert.DeserializeObject(jsonBody);
        double temperature = data.temperature;
        double humidity = data.humidity;
        double heartRate = data.heartRate;

        output = new TemparaturesItem
      {
      Temperature = temperature,
      Humidity = humidity,
      HeartRate = heartRate
      };
    


        }
        [FunctionName("Verifier")]
        public static IActionResult Verifier(
    [   HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "loaderio-27fd5fe6e76858f744782afad73c56d9")] HttpRequest req,
        ILogger log
        ) {
            return new OkObjectResult("loaderio-27fd5fe6e76858f744782afad73c56d9");
          } 
          [FunctionName("GetTemperature")]
      public static IActionResult GetTemperature(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "temperature/")] HttpRequest req,
        [CosmosDB(databaseName: "IoTdata",
                  collectionName: "Temparatures",
                  ConnectionStringSetting = "cosmosDBConnectionString",
                      SqlQuery = "SELECT * FROM c")] IEnumerable< TemparaturesItem> temperatureItem,
                  ILogger log)
      {
        return new OkObjectResult(temperatureItem);
      }


    }
}