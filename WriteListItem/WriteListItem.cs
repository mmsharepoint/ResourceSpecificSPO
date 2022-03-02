using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResourceSpecificSPO.controller;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Linq;

namespace ResourceSpecificSPO
{
  public class WriteListItem
  {
    private readonly controller.TokenValidation tokenValidator;
    private CustomSettings appConfig;

    public WriteListItem(CustomSettings appCnfg,
            controller.TokenValidation tknValidator)
    {
      appConfig = appCnfg;
      tokenValidator = tknValidator;
    }

    [FunctionName("WriteListItem")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req, ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");

      NameValueCollection query = req.RequestUri.ParseQueryString();
      string url = query["url"];
      string listTitle = query["listtitle"];
      string title = query["title"];

      string clientID = appConfig.ClientID;
      string clientSecret = appConfig.ClientSecret;
      string authority = appConfig.Authority;

      log.LogInformation($"Configuration retrieved: AppID={clientID}, Authority={authority}");

      try
      {
        string authHeader = req.Headers.Authorization.Parameter;
        
        var x = await tokenValidator.ValidateTokenAsync(authHeader);
        if (x == null)
        {
          // return new ForbidResult();
          log.LogError("Execution is forbidden as user is not member of group: " + appConfig.SecurityGroupID);
        }
        var y = await tokenValidator.AnalyzeTokenAsync(authHeader);
        foreach(Claim c in y.Claims)
        {
          log.LogInformation(c.Type + " : " + c.Value);
        }
        var roleClaims = y.Claims.Where(c => c.Type == "groups");
        if (!roleClaims.Any(c => c.Value == appConfig.SecurityGroupID))
        {
          return new ForbidResult();
        }
      }
      catch (Exception ex)
      {
        log.LogError(ex.Message);
      }

      GraphController controller = new GraphController();
      controller.Initialize(clientID, authority, clientSecret);

      var response = controller.AddListItem(title, listTitle, url).Result;
      string responseMessage = String.Format("Item created with list item id {0}", response);

      return new OkObjectResult(responseMessage);
    }
  }
}
