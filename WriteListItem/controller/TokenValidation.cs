using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ResourceSpecificSPO.controller
{
  public class TokenValidation
  {
    private CustomSettings appConfig;
    private const string scopeType = @"http://schemas.microsoft.com/identity/claims/scope";
    private ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
    private ClaimsPrincipal _claimsPrincipal;

    private string _wellKnownEndpoint = string.Empty;
    private string _tenantId = string.Empty;
    private string _audience = string.Empty;
    private string _instance = string.Empty;
    private string _requiredScope = "user_impersonation";

    public TokenValidation(CustomSettings appCnfg)
    {
      appConfig = appCnfg;
      _tenantId = appConfig.TenantId;
      _audience = appConfig.Audience;
      _instance = appConfig.Instance;
      // _wellKnownEndpoint = $"{_instance}{_tenantId}/v2.0/.well-known/openid-configuration";
      _wellKnownEndpoint = $"{_instance}common/.well-known/openid-configuration";      
    }

    public async Task<ClaimsPrincipal> ValidateTokenAsync(string authorizationHeader)
    {
      if (string.IsNullOrEmpty(authorizationHeader))
      {
        return null;
      }

      var oidcWellknownEndpoints = await GetOIDCWellknownConfiguration();

      var tokenValidator = new JwtSecurityTokenHandler();

      var validationParameters = new TokenValidationParameters
      {
        RequireSignedTokens = false,
        ValidAudience = _audience,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = false,
        ValidateLifetime = true,
        IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys,
        ValidIssuer = oidcWellknownEndpoints.Issuer.Replace("{tenantid}", _tenantId)
      };

      try
      {
        SecurityToken securityToken;
        _claimsPrincipal = tokenValidator.ValidateToken(authorizationHeader, validationParameters, out securityToken);

        if (IsScopeValid(_requiredScope))
        {
          if (isGroupMember(appConfig.SecurityGroupID))
          {
            return _claimsPrincipal;
          }
        }

        return null;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public async Task<JwtSecurityToken> AnalyzeTokenAsync(string authorizationHeader)
    {
      if (string.IsNullOrEmpty(authorizationHeader))
      {
        return null;
      }

      var tokenValidator = new JwtSecurityTokenHandler();

      var token = tokenValidator.ReadJwtToken(authorizationHeader);
      
      return token;
    }
    private async Task<OpenIdConnectConfiguration> GetOIDCWellknownConfiguration()
    {
      try
      {
        _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
           _wellKnownEndpoint, new OpenIdConnectConfigurationRetriever());

        return await _configurationManager.GetConfigurationAsync();
      }
      catch(Exception ex)
      {
        throw ex;
      }
    }

    private bool IsScopeValid(string scopeName)
    {
      if (_claimsPrincipal == null)
      {
        return false;
      }

      var scopeClaim = _claimsPrincipal.HasClaim(x => x.Type == scopeType)
          ? _claimsPrincipal.Claims.First(x => x.Type == scopeType).Value
          : string.Empty;

      if (string.IsNullOrEmpty(scopeClaim))
      {
        return false;
      }

      if (!scopeClaim.Equals(scopeName, StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }

      return true;
    }

    private bool isGroupMember(string groupID)
    {
      var roleClaims = _claimsPrincipal.Claims.Where(c => c.Type.ToLower() == "groups");
      if (roleClaims.Any(c => c.Value.ToLower() == groupID.ToLower()))
      {
        return true;
      }
      return false;
    }
  }
}
