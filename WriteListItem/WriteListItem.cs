using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResourceSpecificSPO.controller;

namespace ResourceSpecificSPO
{
  public static class WriteListItem
  {
    [FunctionName("WriteListItem")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");

      string url = req.Query["url"];
      string listTitle = req.Query["listtitle"];
      string title = req.Query["title"];

      string clientID = Environment.GetEnvironmentVariable("ClientID");
      string clientSecret = Environment.GetEnvironmentVariable("ClientSecret");
      string authority = Environment.GetEnvironmentVariable("Authority");
      //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      //dynamic data = JsonConvert.DeserializeObject(requestBody);
      //name = name ?? data?.name;
      GraphController controller = new GraphController();
      controller.Initialize(clientID, authority, clientSecret);

      var response = controller.AddListItem(title, listTitle, url).Result;
      string responseMessage = String.Format("Item created with list item id {0}", response);

      return new OkObjectResult(responseMessage);
    }
  }
}
