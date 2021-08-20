# Summary
This little demo Azure Function shows how to deal with Microsoft Graph's resource specific consent (rsc) permissions.
It also contains a corresponding SPFx webpart to call the Azure function. This webpart also shows how to restrict those calls to a specific security group.

For detailed description refer to the [author's blog post](https://mmsharepoint.wordpress.com/2021/08/18/accessing-sharepoint-sites-with-resource-specific-consent-rsc-and-microsoft-graph/)

## Version history

Version|Date|Author|Comments
-------|----|----|--------
1.0|August 18, 2021|[Markus Moeller](https://twitter.com/moeller2_0)|Initial release
2.0|August 20, 2021|[Markus Moeller](https://twitter.com/moeller2_0)|Added SPFx webpart to call Azure function

## Disclaimer

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

## Minimal Path to Awesome

- Clone this repository
    ```bash
    git clone https://github.com/PnP/teams-dev-samples.git
    ```
- Register an app in Azure AD [also described here](https://mmsharepoint.wordpress.com/2021/08/18/accessing-sharepoint-sites-with-resource-specific-consent-rsc-and-microsoft-graph/#appreg)
  - with client secret
  - with application permission "Sites.Selected"
  - grant admin consent
- Open Solution in Visual Studio
- Create local.settings.json from local - Sample.settings.json
  - Insert ClientID
  - Insert Client secret
  - Insert tenant host or id
- Create a simple basic list in one of your sites ("Title" field sufficient)
- Add "Write" permission to your site via [Microsoft Graph](https://docs.microsoft.com/en-us/graph/api/site-post-permissions?view=graph-rest-1.0&tabs=http) with your app registration from above. Also described [here](https://mmsharepoint.wordpress.com/2021/08/18/accessing-sharepoint-sites-with-resource-specific-consent-rsc-and-microsoft-graph/#rsc)
- In Visual Studio run F5
- Call your Azure function via http://localhost:7071/api/WriteListItem?url=<Your Site url>&listtitle=<Your list title>&title=<Your desired new item title>
To also use the webpart:
- Add an Azure function app as described [here](https://mmsharepoint.wordpress.com/2021/08/20/restrict-calls-to-azure-functions-from-spfx/#function)
- Fill our /webpart/src/webparts/resourceSpecificSpo/components/azFunc.json accrodingly
- Run gulp bundle --ship && gulp package-solution --ship
- Publsih the .sppkg file to your app catalog
- Approve the Api permission request inside your SPO tenant admin center
- Install and instantiate the webpart on a page inside your site with the test list from above
- In the webpart property pane add the list title
Now you are good to go