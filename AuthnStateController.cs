using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Okta.Auth.Sdk;

namespace OktaDemo
{
    /// <summary>
    /// Defines the <see cref="AuthnStateController" />
    /// </summary>
    public class AuthnStateController
    {
        /// <summary>
        /// Defines the RegistryOfflineProperty
        /// </summary>
        public bool RegisterOfflineProperty;

        public AuthenticationResponse AuthenticationResponse;


        /// <summary>
        /// Defines the authnClient
        /// </summary>
        private readonly AuthenticationClient authnClient;

        /// <summary>
        /// Defines the appSettingsHelper
        /// </summary>
        private readonly IAppSettingsHelper appSettingsHelper;

        /// <summary>
        /// Defines the SignOnPolicyRegistryHelper
        /// </summary>
        private SignOnPolicyRegistryHelper signOnPolicyRegistryHelper;

        /// <summary>
        /// Defines the username
        /// </summary>
        public readonly string username;

        private bool isUserEnrolledForTotp;

        public bool isUserEnrolledForU2f;

        public bool isUserEnrolledForOktaU2f;


        /// <summary>
        /// Initializes a new instance of the <see cref="AuthnStateController"/> class.
        /// </summary>
        public AuthnStateController(AuthenticationClient client, IAppSettingsHelper appSettingsHelper, string userName)
        {
            this.authnClient = client;
            this.appSettingsHelper = appSettingsHelper;
            this.username = userName;
            this.signOnPolicyRegistryHelper = new SignOnPolicyRegistryHelper();
        }

        /// <summary>
        /// The StateChangeHandler
        /// </summary>
        /// <param name="e">The e<see cref="string"/></param>
        public delegate void StateChangeHandler(string e);

        /// <summary>
        /// The onCanSkip
        /// </summary>
        public delegate void OnCanSkip();

        /// <summary>
        /// Defines the onStateChange
        /// </summary>
        public event StateChangeHandler OnStateChange = delegate { };

        /// <summary>
        /// Gets or sets the StateToken
        /// </summary>
        public string StateToken { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        public string Status { get; set; } = null;

        /// <summary>
        /// Gets or sets the SessionToken
        /// </summary>
        public string SessionToken { get; set; }

        /// <summary>
        /// Gets or sets the VerifyFactors
        /// </summary>
        public IDictionary<string, IVerifyFactor> VerifyFactors { get; set; } = new Dictionary<string, IVerifyFactor>();

        /// <summary>
        /// Gets or sets the EnrollFactors
        /// </summary>
        public IDictionary<string, IEnrollFactor> EnrollFactors { get; set; } = new Dictionary<string, IEnrollFactor>();

        /// <summary>
        /// Gets or sets the PolicyInfo
        /// </summary>
        public PolicyInfo policyInfo { get; set; }

        /// <summary>
        /// Gets or sets the PolicyExpiration
        /// </summary>
        public PolicyExpiration policyExpiration { get; set; }

        /// /// <summary>
        /// Gets or sets the PolicyComplexity
        /// </summary>
        public PolicyComplexity policyComplexity { get; set; }

        /// <summary>
        /// Gets or sets the PolicyAge
        /// </summary>
        public PolicyAge policyAge { get; set; }

        /// <summary>
        /// Defines the DonotChallengeCheckBoxState
        /// CheckBoxState
        /// </summary>
        public bool DonotChallengeCheckBoxState { get; set; } = true;

        internal int OktaFactorCount;

        /// <summary>
        /// Gets the StatusHistoryList
        /// </summary>
        public List<string> StatusHistoryList { get; } = new List<string>();

        /// <summary>
        /// The GetFactorKey
        /// </summary>
        /// <param name="factorObj">The factorObj<see cref="FactorInfo"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetFactorKey(FactorInfo factorObj)
        {
            string factorKey = factorObj.FactorType + ":" + factorObj.Provider;
            return factorKey;
        }

        /// <summary>
        /// The IsFactorRequired
        /// </summary>
        /// <param name="factorObj">The factorObj<see cref="FactorInfo"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string IsFactorRequired(FactorInfo factorObj)
        {
            string isFactorRequired = factorObj.Enrollment;
            return isFactorRequired;
        }

        /// <summary>
        /// The GetFactorEnrollStatus
        /// </summary>
        /// <param name="factorObj">The factorObj<see cref="FactorInfo"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetFactorEnrollStatus(FactorInfo factorObj)
        {
            string factorEnrollStatus = factorObj.Status;
            return factorEnrollStatus;
        }

        /// <summary>
        /// The GetFactorKey
        /// </summary>
        /// <param name="factorObj">The factorObj<see cref="IVerifyFactor"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetFactorKey(IVerifyFactor factorObj)
        {
            string factorKey = factorObj.FactorType + ":" + factorObj.Provider;
            return factorKey;
        }

        /// <summary>
        /// The GetFactorKey
        /// </summary>
        /// <param name="factorObj">The factorObj<see cref="IEnrollFactor"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetFactorKey(IEnrollFactor factorObj)
        {
            string factorKey = factorObj.FactorType + ":" + factorObj.Provider;
            return factorKey;
        }

        /// <summary>
        /// The GetFactorId
        /// </summary>
        /// <param name="factorType">The factorType<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string GetFactorId(string factorType)
        {
            if (this.VerifyFactors.ContainsKey(factorType))
            {
                return this.VerifyFactors[factorType].Id;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The GetFactorId
        /// </summary>
        /// <param name="factorType">The factorType<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string GetEnrollFactorId(string factorType)
        {
            if (this.EnrollFactors.ContainsKey(factorType))
            {
                return this.EnrollFactors[factorType].Id;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The GetVerifyFactorTypes
        /// </summary>
        /// <returns>The <see cref="List{string}"/></returns>
        public List<string> GetVerifyFactorTypes()
        {
            List<string> verifyFactors = new List<string>();
            foreach (KeyValuePair<string, IVerifyFactor> factor in this.VerifyFactors)
            {
                if (factor.Value.FactorType != null)
                {
                    verifyFactors.Add(factor.Value.FactorType);
                }
            }

            return verifyFactors;
        }

        /// <summary>
        /// The ProcessAuthnResponse
        /// </summary>
        /// <param name="authenticationResponse">The authenticationResponse<see cref="IAuthenticationResponse"/></param>
        public void ProcessAuthnResponse(IAuthenticationResponse authenticationResponse)
        {

            // this.logger.Debug("ProcessAuthnResponse sessionToken: " + authenticationResponse.SessionToken);
            this.AuthenticationResponse = (AuthenticationResponse)authenticationResponse;
            this.Status = authenticationResponse.AuthenticationStatus;
            this.SessionToken = authenticationResponse.SessionToken;
            this.StateToken = authenticationResponse.StateToken;

            this.SaveAuthenticationStatus(this.Status);

            if (this.Status == "MFA_REQUIRED")
            {
                List<IVerifyFactor> verifyFactorsList = this.PrepareVerifyFactors(authenticationResponse);
                this.AssignVerifyFactors(verifyFactorsList);
                PolicyInfo policyInfo = this.PreparePolicyInfo(authenticationResponse);
                this.AssignPolicyInfo(policyInfo);
            }
            else if (this.Status == "MFA_CHALLENGE")
            {
                var factorResponse = JsonConvert.SerializeObject(authenticationResponse.Embedded["factor"]);
                FactorInfo factorObj = JsonConvert.DeserializeObject<FactorInfo>(factorResponse);
                string factorKey = GetFactorKey(factorObj);
            }
            else if (this.Status == "MFA_ENROLL")
            {
                List<IEnrollFactor> enrollFactorsList = this.PrepareEnrollFactors(authenticationResponse);
                this.AssignEnrollFactors(enrollFactorsList);
            }
            else if (this.Status == "PASSWORD_WARN")
            {
                if (!this.DonotChallengeCheckBoxState)
                {
                    this.signOnPolicyRegistryHelper.AssignDeviceToken(this.username);
                }

                PolicyExpiration policyExpiration = this.PreparePolicyExpirationObject(authenticationResponse);
                this.AssignPolicyExpirationObj(policyExpiration);
                PolicyComplexity policyComplexity = this.PreparePolicyComplexityObject(authenticationResponse);
                this.AssignPolicyComplexityObj(policyComplexity);
                PolicyAge policyAge = this.PreparePolicyAgeObject(authenticationResponse);
                this.AssignPolicyAgeObj(policyAge);
            }
            else if (this.Status == "PASSWORD_EXPIRED")
            {
                if (!this.DonotChallengeCheckBoxState)
                {
                    this.signOnPolicyRegistryHelper.AssignDeviceToken(this.username);
                }

                PolicyComplexity policyComplexity = this.PreparePolicyComplexityObject(authenticationResponse);
                this.AssignPolicyComplexityObj(policyComplexity);
                PolicyAge policyAge = this.PreparePolicyAgeObject(authenticationResponse);
                this.AssignPolicyAgeObj(policyAge);
            }
            else if (this.Status == "SUCCESS")
            {
                //
            }

            // State changed
            this.OnStateChange(this.Status);
        }

        /// <summary>
        /// Adds a key value pair to the IDictionary<string, IVerifyFactor> VerifyFactors global object
        /// </summary>
        /// <param name="factorKey"></param>
        /// <param name="factorInfo"></param>
        private void AddVerifyFactor(string factorKey, FactorInfo factorInfo)
        {
            if (!this.VerifyFactors.ContainsKey(factorKey))
            {
                this.VerifyFactors.Add(factorKey, this.CreateFactorObject(factorInfo));
            }
        }

        private void SaveAuthenticationStatus(string status)
        {
            this.StatusHistoryList.Add(status);
        }

        private PolicyExpiration PreparePolicyExpirationObject(IAuthenticationResponse authenticationResponse)
        {
            var policyResponseObject = JsonConvert.SerializeObject(authenticationResponse.Embedded["policy"]);
            dynamic policyExpirationObj = JsonConvert.DeserializeObject<dynamic>(policyResponseObject).expiration;
            string policyExpirationString = JsonConvert.SerializeObject(policyExpirationObj);
            PolicyExpiration policyExpiration = JsonConvert.DeserializeObject<PolicyExpiration>(policyExpirationString);
            return policyExpiration;
        }

        private PolicyComplexity PreparePolicyComplexityObject(IAuthenticationResponse authenticationResponse)
        {
            var policyResponseObject = JsonConvert.SerializeObject(authenticationResponse.Embedded["policy"]);
            dynamic policyComplexityObj = JsonConvert.DeserializeObject<dynamic>(policyResponseObject).complexity;
            string passwordComplexityString = JsonConvert.SerializeObject(policyComplexityObj);
            PolicyComplexity passwordComplexityObj = JsonConvert.DeserializeObject<PolicyComplexity>(passwordComplexityString);
            return passwordComplexityObj;
        }

        private PolicyAge PreparePolicyAgeObject(IAuthenticationResponse authenticationResponse)
        {
            var policyResponseObject = JsonConvert.SerializeObject(authenticationResponse.Embedded["policy"]);
            dynamic policyAgeObj = JsonConvert.DeserializeObject<dynamic>(policyResponseObject).age;
            string policyAgeString = JsonConvert.SerializeObject(policyAgeObj);
            PolicyAge policyAge = JsonConvert.DeserializeObject<PolicyAge>(policyAgeString);
            return policyAge;
        }

        private void AssignPolicyAgeObj(PolicyAge policyAge)
        {
            this.policyAge = policyAge;
        }

        private void AssignPolicyComplexityObj(PolicyComplexity passwordComplexity)
        {
            this.policyComplexity = passwordComplexity;
        }

        private void AssignPolicyExpirationObj(PolicyExpiration policyExpiration)
        {
            this.policyExpiration = policyExpiration;
        }

        /// <summary>
        /// The GetFactorObjByType
        /// </summary>
        /// <param name="factorType">The factorType<see cref="string"/></param>
        /// <returns>The <see cref="IVerifyFactor"/></returns>
        public IVerifyFactor GetFactorObjByType(string factorType)
        {
            if (this.VerifyFactors.ContainsKey(factorType))
            {
                return this.VerifyFactors[factorType];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The CreateFactorObject
        /// </summary>
        /// <param name="factorInfo">The factorInfo<see cref="FactorInfo"/></param>
        /// <returns>The <see cref="IVerifyFactor"/></returns>
        public IVerifyFactor CreateFactorObject(FactorInfo factorInfo)
        {
            IVerifyFactor verifyFactor = null;
            if (factorInfo.FactorKey == "sms:OKTA")
            {
                SmsFactor smsFactor = new SmsFactor(this.authnClient, this)
                {
                    Id = factorInfo.Id,
                    Profile = factorInfo.Profile,
                    FactorType = factorInfo.FactorType,
                    Provider = factorInfo.Provider,
                    VendorName = factorInfo.VendorName,
                    Links = factorInfo.Links,
                    FactorDisplayName = "SMS Authentication",
                };
                verifyFactor = smsFactor;
            }
            else if (factorInfo.FactorKey == "email:OKTA")
            {
                EmailFactor emailFactor = new EmailFactor(this.authnClient, this)
                {
                    Id = factorInfo.Id,
                    Profile = factorInfo.Profile,
                    FactorType = factorInfo.FactorType,
                    Provider = factorInfo.Provider,
                    VendorName = factorInfo.VendorName,
                    Links = factorInfo.Links,
                    FactorDisplayName = "Email Authentication",
                };
                verifyFactor = emailFactor;
            }

            return verifyFactor;
        }

        /// <summary>
        /// The CreateEnrollFactorObject
        /// </summary>
        /// <param name="factorInfo">The factorInfo<see cref="FactorInfo"/></param>
        /// <returns>The <see cref="IEnrollFactor"/></returns>
        public IEnrollFactor CreateEnrollFactorObject(FactorInfo factorInfo)
        {
            IEnrollFactor enrollFactor = null;
            if (factorInfo.FactorKey == "sms:OKTA")
            {
                SmsFactorEnroll smsFactorEnroll = new SmsFactorEnroll(this.authnClient, this)
                {
                    Id = factorInfo.Id,
                    Profile = factorInfo.Profile,
                    FactorType = factorInfo.FactorType,
                    Provider = factorInfo.Provider,
                    VendorName = factorInfo.VendorName,
                    Links = factorInfo.Links,
                    FactorDisplayName = "SMS Authentication",
                    Enrollment = factorInfo.Enrollment,
                    Status = factorInfo.Status,
                };
                enrollFactor = smsFactorEnroll;
            }

            return enrollFactor;
        }

        /// <summary>
        /// The GetFactorDisplayNames
        /// </summary>
        /// <returns>The <see cref="List{String}"/></returns>
        public List<string> GetFactorDisplayNames()
        {
            List<string> factorTitles = new List<string>();
            foreach (KeyValuePair<string, IVerifyFactor> factor in this.VerifyFactors)
            {
                if (factor.Value.FactorDisplayName != null)
                {
                    factorTitles.Add(factor.Value.FactorDisplayName);
                }
            }
            return factorTitles;
        }

        /// <summary>
        /// The GetFactorTypeByDisplayName
        /// </summary>
        /// <param name="factorDisplayName">The FactorDisplayName<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string GetFactorTypeByDisplayName(string factorDisplayName)
        {
            string factorType = null;
            foreach (KeyValuePair<string, IVerifyFactor> factor in this.VerifyFactors)
            {
                if (factorDisplayName == factor.Value.FactorDisplayName)
                {
                    return factor.Value.FactorType;
                }
            }

            return factorType;
        }

        /// <summary>
        /// The GetFactorKeyByDisplayName
        /// </summary>
        /// <param name="factorDisplayName">The FactorDisplayName<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        public string GetFactorKeyByDisplayName(string factorDisplayName)
        {
            string factorKey = null;
            foreach (KeyValuePair<string, IVerifyFactor> factor in this.VerifyFactors)
            {
                if (factorDisplayName == factor.Value.FactorDisplayName)
                {
                    return string.Format("{0}:{1}", factor.Value.FactorType, factor.Value.Provider);
                }
            }

            return factorKey;
        }

        /// <summary>
        /// The GetFactorReference
        /// </summary>
        /// <param name="factorKey">The factorKey<see cref="string"/></param>
        /// <returns>The <see cref="IVerifyFactor"/></returns>
        public IVerifyFactor GetFactorReference(string factorKey)
        {
            if (this.VerifyFactors.ContainsKey(factorKey))
            {
                return this.VerifyFactors[factorKey];
            }

            return null;
        }

        /// <summary>
        /// The GetFactorReference
        /// </summary>
        /// <param name="factorKey">The factorKey<see cref="string"/></param>
        /// <returns>The <see cref="IVerifyFactor"/></returns>
        public IEnrollFactor GetEnrollFactorReference(string factorKey)
        {
            if (this.EnrollFactors.ContainsKey(factorKey))
            {
                return this.EnrollFactors[factorKey];
            }

            return null;
        }

        /// <summary>
        /// The GetVerifyFactorKeysList
        /// </summary>
        /// <returns>The <see cref="List{string}"/></returns>
        public List<string> GetVerifyFactorKeysList()
        {
            List<string> verifyFactorKeys = new List<string>();
            foreach (KeyValuePair<string, IVerifyFactor> factor in this.VerifyFactors)
            {
                if (factor.Value.FactorType != null)
                {
                    verifyFactorKeys.Add(string.Format("{0}:{1}", factor.Value.FactorType, factor.Value.Provider));
                }
            }

            return verifyFactorKeys;
        }

        /// <summary>
        /// The GetEnrollFactorKeysList
        /// </summary>
        /// <returns>The <see cref="List{string}"/></returns>
        public List<string> GetEnrollFactorKeysList()
        {
            List<string> enrollFactorKeys = new List<string>();
            foreach (KeyValuePair<string, IEnrollFactor> factor in this.EnrollFactors)
            {
                if (factor.Value.FactorType != null)
                {
                    enrollFactorKeys.Add(string.Format("{0}:{1}", factor.Value.FactorType, factor.Value.Provider));
                }
            }

            return enrollFactorKeys;
        }

        /// <summary>
        /// The PrepareVerifyFactors
        /// </summary>
        /// <param name="authenticationResponse">The authenticationResponse<see cref="IAuthenticationResponse"/></param>
        /// <returns>The <see cref="List{IVerifyFactor}"/></returns>
        private List<IVerifyFactor> PrepareVerifyFactors(IAuthenticationResponse authenticationResponse)
        {
            List<IVerifyFactor> verifyFactorsList = new List<IVerifyFactor>();
            //this.logger.Debug("ProcessAuthnResponse stateToken: " + authenticationResponse.StateToken);
            if (authenticationResponse.Embedded.GetData().ContainsKey("factors"))
            {
                string stringResponse = JsonConvert.SerializeObject(authenticationResponse.Embedded.GetData()["factors"]);
                List<object> factorsList = JsonConvert.DeserializeObject<List<object>>(stringResponse);
                this.OktaFactorCount = factorsList.Count;
                foreach (var factor in factorsList)
                {
                    string factorStringResponse = JsonConvert.SerializeObject(factor);
                    FactorInfo factorObj = JsonConvert.DeserializeObject<FactorInfo>(factorStringResponse);
                    string factorKey = GetFactorKey(factorObj);
                    factorObj.FactorKey = factorKey;
                    string[] supportedFactors = { "sms:OKTA", "email:OKTA" };
                    if (supportedFactors.Contains(factorKey))
                    {
                        IVerifyFactor verifyFactor = this.CreateFactorObject(factorObj);
                        verifyFactorsList.Add(verifyFactor);
                    }
                    else
                    {
                        this.OktaFactorCount -= 1;
                    }
                }

            }

            return verifyFactorsList;
        }

        private List<IEnrollFactor> PrepareEnrollFactors(IAuthenticationResponse authenticationResponse)
        {
            List<IEnrollFactor> enrollFactorsList = new List<IEnrollFactor>();
            if (authenticationResponse.Embedded.GetData().ContainsKey("factors"))
            {
                string stringResponse = JsonConvert.SerializeObject(authenticationResponse.Embedded.GetData()["factors"]);
                List<object> factorsList = JsonConvert.DeserializeObject<List<object>>(stringResponse);
                this.OktaFactorCount = factorsList.Count;
                foreach (var factor in factorsList)
                {
                    string factorStringResponse = JsonConvert.SerializeObject(factor);
                    FactorInfo factorObj = JsonConvert.DeserializeObject<FactorInfo>(factorStringResponse);
                    string factorKey = GetFactorKey(factorObj);
                    string isFactorRequired = IsFactorRequired(factorObj);
                    string factorEnrollmentStatus = GetFactorEnrollStatus(factorObj);
                    string factorId = GetEnrollFactorId(factorObj.FactorType);
                    factorObj.FactorKey = factorKey;
                    string[] supportedFactors = { "sms:OKTA" };
                    if (supportedFactors.Contains(factorKey) && isFactorRequired == "REQUIRED" && factorEnrollmentStatus == "NOT_SETUP")
                    {
                        IEnrollFactor enrollFactor = this.CreateEnrollFactorObject(factorObj);
                        enrollFactorsList.Add(enrollFactor);
                    }
                }

            }

            return enrollFactorsList;
        }

        /// <summary>
        /// The AssignVerifyFactors
        /// </summary>
        /// <param name="verifyFactorsList">The verifyFactorsList<see cref="List{IVerifyFactor}"/></param>
        private void AssignVerifyFactors(List<IVerifyFactor> verifyFactorsList)
        {
            foreach (var factor in verifyFactorsList)
            {
                string factorKey = GetFactorKey(factor);
                this.VerifyFactors.Add(factorKey, factor);
            }
        }

        /// <summary>
        /// The AssignEnrollFactors
        /// </summary>
        /// <param name="enrollFactorsList">The verifyFactorsList<see cref="List{IEnrollFactor}"/></param>
        private void AssignEnrollFactors(List<IEnrollFactor> enrollFactorsList)
        {
            foreach (var factor in enrollFactorsList)
            {
                string factorKey = GetFactorKey(factor);
                this.EnrollFactors.Add(factorKey, factor);
            }
        }

        /// <summary>
        /// The PreparePolicyInfo
        /// </summary>
        /// <param name="authenticationResponse">The verifyFactorsList<see cref="IAuthenticationResponse"/></param>
        private PolicyInfo PreparePolicyInfo(IAuthenticationResponse authenticationResponse)
        {
            var policyResponse = JsonConvert.SerializeObject(authenticationResponse.Embedded["policy"]);
            PolicyInfo policyObj = JsonConvert.DeserializeObject<PolicyInfo>(policyResponse);
            return policyObj;
        }

        /// <summary>
        /// The AssignPolicyInfo
        /// </summary>
        /// <param name="policyInfo">The verifyFactorsList<see cref="PolicyInfo"/></param>
        private void AssignPolicyInfo(PolicyInfo policyInfo)
        {
            this.policyInfo = policyInfo;
        }


    }

    /// <summary>
    /// Defines the <see cref="FactorInfo" />
    /// </summary>
    public class FactorInfo
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the factorType
        /// </summary>
        public string FactorType { get; set; }

        /// <summary>
        /// Gets or sets the factorKey
        /// </summary>
        public string FactorKey { get; set; }

        /// <summary>
        /// Gets or sets the provider
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the vendorName
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// Gets or sets the factorResult
        /// </summary>
        public string FactorResult { get; set; }

        /// <summary>
        /// Gets or sets the profile
        /// </summary>
        public dynamic Profile { get; set; }

        /// <summary>
        /// Gets or sets the _links
        /// </summary>
        public dynamic Links { get; set; }

        /// <summary>
        /// Gets or sets the factorDisplayName
        /// </summary>
        public string FactorDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the enrollment
        /// </summary>
        public string Enrollment { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="PolicyInfo" />
    /// </summary>
    public class PolicyInfo
    {
        /// <summary>
        /// Gets or sets the AllowRememberDevice
        /// </summary>
        public bool AllowRememberDevice { get; set; }

        /// <summary>
        /// Gets or sets the RememberDeviceLifeTimeInMinutes
        /// </summary>
        public int RememberDeviceLifeTimeInMinutes { get; set; }

        /// <summary>
        /// Gets or sets the RememberDeviceByDefault
        /// </summary>
        public bool RememberDeviceByDefault { get; set; }

        /// <summary>
        /// Gets or sets the RememberDeviceByDefault
        /// </summary>
        public object FactorsPolicyInfo { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="PolicyExpiration" />
    /// </summary>
    public class PolicyExpiration
    {
        /// <summary>
        /// Gets or sets the PasswordExpirationDays
        /// </summary>
        public int PasswordExpireDays { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="PolicyComplexity" />
    /// </summary>
    public class PolicyComplexity
    {
        /// <summary>
        /// Gets or sets the MinLength
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Gets or sets the MinLowerCase
        /// </summary>
        public int MinLowerCase { get; set; }

        /// <summary>
        /// Gets or sets the MinUpperCase
        /// </summary>
        public int MinUpperCase { get; set; }

        /// <summary>
        /// Gets or sets the MinNumber
        /// </summary>
        public int MinNumber { get; set; }

        /// <summary>
        /// Gets or sets the MinSymbol
        /// </summary>
        public int MinSymbol { get; set; }

        /// <summary>
        /// Gets or sets the ExcludeUsername
        /// </summary>
        public bool ExcludeUsername { get; set; }

        /// <summary>
        /// Gets or sets the ExcludeAttributes
        /// </summary>
        public object ExcludeAttributes { get; set; }
    }

    public class PolicyAge
    {
        /// <summary>
        /// Gets or sets the MinAgeMinutes
        /// </summary>
        public int MinAgeMinutes { get; set; }

        /// <summary>
        /// Gets or sets the HistoryCount
        /// </summary>
        public int HistoryCount { get; set; }
    }

}
