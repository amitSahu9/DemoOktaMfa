using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Okta.Auth.Sdk;
using Okta.Sdk.Abstractions;

namespace OktaDemo
{
    public interface IEnrollFactor
    {
        [JsonProperty("id")]
        string Id { get; set; }

        [JsonProperty("factorType")]
        string FactorType { get; set; }

        [JsonProperty("provider")]
        string Provider { get; set; }

        [JsonProperty("vendorName")]
        string VendorName { get; set; }

        [JsonProperty("profile")]
        dynamic Profile { get; set; }

        [JsonProperty("phoneNumber")]
        string PhoneNumber { get; set; }

        [JsonProperty("phoneExtension")]
        string PhoneExtension { get; set; }

        [JsonProperty("_links")]
        dynamic Links { get; set; }

        string FactorDisplayName { get; set; }

        Task SendAsync(string phoneExtension, string phoneNumber);

        Task VerifyAsync();

        Task VerifyAsync(string passCode);

        Task Resend();

        string GetFactorTitle();

        string GetFactorType();

        string GetFactorKey();
    }

    public class SmsFactorEnroll : IEnrollFactor
    {
        private readonly AuthenticationClient authnClient;
        private readonly AuthnStateController stateController;

        public SmsFactorEnroll(AuthenticationClient authnClient, AuthnStateController stateController)
        {
            this.authnClient = authnClient;
            this.stateController = stateController;
        }

        public string Id { get; set; }

        public string FactorType { get; set; }

        public string Provider { get; set; }

        public string VendorName { get; set; }

        public dynamic Profile { get; set; }

        public string PhoneNumber { get; set; }

        public string PhoneExtension { get; set; }

        public dynamic Links { get; set; }

        public string FactorDisplayName { get; set; }

        public string Enrollment { get; set; }

        public string Status { get; set; }

        public async Task SendAsync(string phoneExtension, string phoneNumber)
        {
            var enrollOptions = new EnrollSmsFactorOptions()
            {
                StateToken = this.stateController.StateToken,
                PhoneExtension = phoneExtension,
                PhoneNumber = phoneNumber,
                FactorId = "mblry4uxieTrPiOcp0h7",
                Provider = this.Provider,
            };

            var authResponse = await this.authnClient.EnrollFactorAsync(enrollOptions);
            this.stateController.ProcessAuthnResponse(authResponse);
        }

        public async Task VerifyAsync()
        {
            throw new NotImplementedException();
        }

        public async Task VerifyAsync(string passCode)
        {
            var activateFactorOptions = new ActivateFactorOptions()
            {
                FactorId = this.Id,
                PassCode = passCode,
                StateToken = this.stateController.StateToken,
            };

            var request = new HttpRequest()
            {
                Uri = "/api/v1/authn/factors?updatePhone=true",
                Payload = activateFactorOptions,
            };

            var authResponse = await this.authnClient.PostAsync<AuthenticationResponse>(request);
            this.stateController.ProcessAuthnResponse(authResponse);
        }

        public async Task Resend()
        {
            var enrollFactorOptions = new ResendChallengeOptions()
            {
                StateToken = this.stateController.StateToken,
                FactorId = this.Id,
            };
            var authResponse = await this.authnClient.ResendVerifyChallengeAsync(enrollFactorOptions);
            this.stateController.ProcessAuthnResponse(authResponse);
        }


        public string GetFactorTitle()
        {
            return this.FactorDisplayName;
        }

        public string GetFactorType()
        {
            return this.FactorType;
        }

        public string GetFactorKey()
        {
            return this.FactorType + ":" + this.Provider;
        }
    }
}
