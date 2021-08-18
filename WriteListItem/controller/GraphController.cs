using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ResourceSpecificSPO.controller
{
  class GraphController
  {
    private GraphServiceClient graphClient;

    public void Initialize(string clientId, string authority, string clientSecret)
    {
      var clientApplication = ConfidentialClientApplicationBuilder.Create(clientId)
                                              .WithAuthority(authority)
                                              .WithClientSecret(clientSecret)
                                              .Build();
      List<string> scopes = new List<string>();
      scopes.Add("https://graph.microsoft.com/.default");
      string accessToken = clientApplication.AcquireTokenForClient(scopes).ExecuteAsync().Result.AccessToken;
      GraphServiceClient graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                      requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                    }));
      this.graphClient = graphClient;
    }

    public async Task<string> AddListItem(string title, string listTitle, string siteUrl)
    {
      string siteId = await GetSiteIDByUrl(siteUrl);

      ListItem item = new ListItem
      {
        Fields = new FieldValueSet
        {
          AdditionalData = new Dictionary<string, object>()
          {
            {"Title", title}            
          }
        }
      };
      try
      {
        ListItem newItem = await this.graphClient.Sites[siteId].Lists[listTitle].Items.Request().AddAsync(item);
        return newItem.Id;
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
      
    }
    private async Task<string> GetSiteIDByUrl(string siteUrl)
    {
      Uri url = new Uri(siteUrl);
      string host = url.Host;
      string relativePath = siteUrl.Split(host)[1];      
      try
      {
        Site site = await this.graphClient.Sites.GetByPath(relativePath, host).Request().GetAsync();
        return site.Id;
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }    
  }
}
