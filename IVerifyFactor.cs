using System;
using System.Drawing;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Okta.Auth.Sdk;

namespace OktaDemo 
{
    public interface IVerifyFactor
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

        [JsonProperty("_links")]
        dynamic Links { get; set; }

        string FactorDisplayName { get; set; }

        Task SendAsync();

        Task VerifyAsync();

        Task VerifyAsync(string passCode);

        Task Resend();

        string GetFactorTitle();

        string GetFactorType();

        string GetFactorKey();
    }
    public class SmsFactor : IVerifyFactor
    {
        private readonly AuthenticationClient authnClient;
        private readonly AuthnStateController stateController;

        public SmsFactor(AuthenticationClient authnClient, AuthnStateController stateController)
        {
            this.authnClient = authnClient;
            this.stateController = stateController;
        }

        public string Id { get; set; }

        public string FactorType { get; set; }

        public string Provider { get; set; }

        public string VendorName { get; set; }

        public dynamic Profile { get; set; }

        public dynamic Links { get; set; }

        public string FactorDisplayName { get; set; }

        public async Task SendAsync()
        {
            var verifyFactorOptions = new VerifySmsFactorOptions()
            {
                StateToken = this.stateController.StateToken,
                FactorId = this.Id,
            };

            var authResponse = await this.authnClient.VerifyFactorAsync(verifyFactorOptions);
            this.stateController.ProcessAuthnResponse(authResponse);
        }

        public async Task VerifyAsync()
        {
            throw new NotImplementedException();
        }

        public async Task VerifyAsync(string passCode)
        {
            var verifyFactorOptions = new VerifySmsFactorOptions()
            {
                StateToken = this.stateController.StateToken,
                FactorId = this.Id,
                PassCode = passCode,
            };
            var authResponse = await this.authnClient.VerifyFactorAsync(verifyFactorOptions);
            this.stateController.ProcessAuthnResponse(authResponse);
        }

        public async Task Resend()
        {
            var verifyFactorOptions = new ResendChallengeOptions()
            {
                StateToken = this.stateController.StateToken,
                FactorId = this.Id,
            };
            var authResponse = await this.authnClient.ResendVerifyChallengeAsync(verifyFactorOptions);
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

    public class EmailFactor : IVerifyFactor
    {
        private readonly AuthenticationClient authnClient;
        private readonly AuthnStateController stateController;

        public EmailFactor(AuthenticationClient authnClient, AuthnStateController stateController)
        {
            this.authnClient = authnClient;
            this.stateController = stateController;
        }

        public string Id { get; set; }

        public string FactorType { get; set; }

        public string Provider { get; set; }

        public string VendorName { get; set; }

        public dynamic Profile { get; set; }

        public dynamic Links { get; set; }

        public string FactorDisplayName { get; set; }

        public async Task SendAsync()
        {
            var verifyFactorOptions = new VerifySmsFactorOptions()
            {
                StateToken = this.stateController.StateToken,
                FactorId = this.Id,
            };

            var authResponse = await this.authnClient.VerifyFactorAsync(verifyFactorOptions);
            this.stateController.ProcessAuthnResponse(authResponse);
        }

        public async Task VerifyAsync()
        {
            throw new NotImplementedException();
        }

        public async Task VerifyAsync(string passCode)
        {
            var verifyFactorOptions = new VerifySmsFactorOptions()
            {
                StateToken = this.stateController.StateToken,
                FactorId = this.Id,
                PassCode = passCode,
            };
            var authResponse = await this.authnClient.VerifyFactorAsync(verifyFactorOptions);
            this.stateController.ProcessAuthnResponse(authResponse);
        }

        public async Task Resend()
        {
            var verifyFactorOptions = new ResendChallengeOptions()
            {
                StateToken = this.stateController.StateToken,
                FactorId = this.Id,
            };
            var authResponse = await this.authnClient.ResendVerifyChallengeAsync(verifyFactorOptions);
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
