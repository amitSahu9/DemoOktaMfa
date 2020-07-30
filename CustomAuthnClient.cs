using System;
using System.Threading;
using System.Threading.Tasks;
using Okta.Auth.Sdk;
using Okta.Auth.Sdk.Models;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;

namespace OktaDemo
{
    public class CustomAuthnClient : AuthenticationClient
    {

        public CustomAuthnClient(IDataStore dataStore, OktaClientConfiguration clientConfiguration, RequestContext requestContext)
            : base(dataStore, clientConfiguration, requestContext)
        {
        }

        public async Task<IAuthenticationResponse> AuthenticateAsync(AuthenticateOptions authenticateOptions, CancellationToken cancellationToken = default(CancellationToken))
        {
            var authenticationRequest = new AuthenticationRequest()
            {
                Username = authenticateOptions.Username,
                Password = authenticateOptions.Password,
                Audience = authenticateOptions.Audience,
                RelayState = authenticateOptions.RelayState,
                StateToken = authenticateOptions.StateToken,
                Options = new AuthenticationRequestOptions()
                {
                    MultiOptionalFactorEnroll = authenticateOptions.MultiOptionalFactorEnroll,
                    WarnBeforePasswordExpired = authenticateOptions.WarnBeforePasswordExpired,
                },

                Context = new AuthenticationRequestContext()
                {
                    DeviceToken = authenticateOptions.DeviceToken,
                },
            };

            var request = new HttpRequest
            {
                Uri = "/api/v1/authn",
                Payload = authenticationRequest,
            };
            request.Headers["X-Forwarded-For"] = this.GetXForwadedIP();

            return await this.PostAsync<AuthenticationResponse>(
                request, cancellationToken).ConfigureAwait(false);
        }

        private string GetXForwadedIP()
        {
            string xForwadedIP;
            xForwadedIP = "9.8.9.8";
            return xForwadedIP;
        }
    }
}
