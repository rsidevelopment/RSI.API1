using AccessDevelopment.Services.Interfaces;
using AccessDevelopment.Services.Models._ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AccessDevelopment.Services
{
    public class AccessDevelopmentService : IAccessDevelopmentService
    {
        private readonly string _token = "d41d64b3158ad709fa37fb1a3f5928a4cd921ea0fc48bc0dbce59dd334ba5205";
        private readonly string _diningToken = "139a3635a78eabc414bd318cc529199048a863b641bed45c64cb4578be71a663";
        private readonly string _programId = "200938";
        private readonly string _diningProgramId = "200939";
        private readonly string _organizationId = "2002598";
        private readonly string _url = "https://amt.accessdevelopment.com/api/v1/";
        //private string _url = "https://amt-demo.accessdevelopment.com/api/v1/";

        public async Task<ADListReturnViewModel> AddMemberAsync(List<MemberViewModel> model)
        {
            string url = $"{_url}imports";
            
            ADListReturnViewModel adReturnObj = new ADListReturnViewModel();
            try
            {
                string jsonString = "{\"import\":{ \"members\": ";
                jsonString += JsonConvert.SerializeObject(model, Formatting.Indented);
                jsonString += "}}";

                string token = _token;

                if (model[0].ProgramCustomerIdentifier == _diningProgramId)
                    token = _diningToken;

                (string results, bool isSuccess) = await SendPost(url, jsonString, null, token);

                if (isSuccess)
                {
                    adReturnObj = JsonConvert.DeserializeObject<ADListReturnViewModel>(results);
                    adReturnObj.Message = "Success";
                }
                else
                {
                    dynamic errorTmp = JObject.Parse(results);

                    var msgTmp = errorTmp.message;
                    if (msgTmp != null && msgTmp.ToSting().Length > 0)
                        adReturnObj.Message = $"Error: {msgTmp.ToString()}";
                    else
                        adReturnObj.Message = "Error: No reason provided";
                }
            }
            catch (Exception ex)
            {
                adReturnObj.Message = $"Error: {ex.Message}";
            }

            return adReturnObj;
        }

        #region Public Properties

        public string RawResponse { get; set; }
        public string RawRequest { get; set; }

        #endregion

        #region Private Send Methods
        private async Task<(string result, bool isSuccess)> SendGET(string uri, string token = null)
        {
            (string result, bool isSuccess) = ("Error: not implemented", false);

            if (token == null)
                token = _token;

            using (HttpClient client = new HttpClient() { BaseAddress = new Uri(uri) })
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Content-type", "application/json");
                client.DefaultRequestHeaders.Add("Access-Token", $"{token}");

                using (HttpResponseMessage response = await client.GetAsync(uri))
                using (HttpContent content = response.Content)
                {
                    // ... Read the string.
                    result = await content.ReadAsStringAsync();
                    RawResponse = result;

                    if (response.IsSuccessStatusCode)
                        isSuccess = true;
                    else
                        isSuccess = false;
                    // ... Display the result.

                }
            }

            return (result, isSuccess);
        }

        private async Task<(string result, bool isSuccess)> SendPost(string uri, string dataToPost, string methodOverride = null, string token = null)
        {
            (string result, bool isSuccess) = ("Error: not implemented", false);

            if (token == null)
                token = _token;
           
            try
            {
                using (var client = new HttpClient() { BaseAddress = new Uri(uri) })
                {
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _key);

                    if (!String.IsNullOrEmpty(methodOverride))
                    {
                        client.DefaultRequestHeaders.Add("X-HTTP-Method-Override", methodOverride);
                    }

                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    //client.DefaultRequestHeaders.Add("Content-type", "application/json");
                    client.DefaultRequestHeaders.Add("Access-Token", $"{token}");

                    HttpContent contentToPost = new StringContent(dataToPost, System.Text.Encoding.UTF8, "application/json");
                    using (HttpResponseMessage response = await client.PostAsync(uri, contentToPost))
                    using (HttpContent content = response.Content)
                    {
                        result = await content.ReadAsStringAsync();
                        RawResponse = result;

                        if (response.IsSuccessStatusCode)
                            isSuccess = true;
                        else
                            isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = $"Error: {ex.Message}";
            }

            return (result, isSuccess);
        }

        #endregion
    }
}
