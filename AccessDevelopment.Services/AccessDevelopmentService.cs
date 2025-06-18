using AccessDevelopment.Services.Data;
using AccessDevelopment.Services.Interfaces;
using AccessDevelopment.Services.Models._ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public AccessDevelopmentService()
        {
            // Default constructor
        }

        public async Task<bool>HasAccessDevelopment(int rsiId)
        {
            bool hasAccess = false;

            try
            {
                using var conn = new SqlConnection(SqlHelper.GetConnectionString());
                
                var parameters = new[]
                {
                        new SqlParameter("@RSIId", rsiId),
                        new SqlParameter("@BenefitId", 89),
                };

                var rdr = SqlHelper.ExecuteReader(
                       conn,
                       CommandType.StoredProcedure,
                       "[dbo].[HasBenefit]",
                       parameters);

                if (rdr.HasRows)
                {
                    await rdr.ReadAsync();
                    int i = !rdr.IsDBNull(0) ? rdr.GetInt32(0) : 0;
                    if(i > 0)
                    {
                        hasAccess = true;
                    }
                    
                }
            }
            catch (Exception)
            {

                throw;
            }

            return hasAccess;
        }

        private async Task<List<MemberViewModel>>FilterList(List<MemberViewModel> model)
        {
            List<MemberViewModel> newList = new();

            try
            {
                foreach(var row in model)
                {
                    if (await HasAccessDevelopment(row.RSIId))
                        newList.Add(row);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return newList;
        }

        public async Task<ADListReturnViewModel> AddMemberAsync(List<MemberViewModel> model)
        {
            model = await FilterList(model);

            string url = $"{_url}imports";
            
            ADListReturnViewModel adReturnObj = new ADListReturnViewModel();
            try
            {
                if (model != null && model.Count > 0)
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
                else { adReturnObj.Message = "Error: No members to add"; }
            }
            catch (Exception ex)
            {
                adReturnObj.Message = $"Error: {ex.Message}";
            }

            return adReturnObj;
        }

        public async Task<ListViewModel<ADCSVFileViewModel>> GetCSV(string url, string token = null)
        {
            ListViewModel<ADCSVFileViewModel> model = new();

            try
            {
                token ??= _token;

                (string results, bool isSuccess) = await SendGET(url, token);
                if (isSuccess && !results.Contains("message", StringComparison.CurrentCulture))
                {
                    string[] rowsTmp = results.Split("\n");
                    string[] parseTmp = rowsTmp[1].Split(',');

                    model.Items.Add(new()
                    {
                        RecordIdentifier = parseTmp[0],
                        RecordType = parseTmp[1],
                        OrganizationCustomerIdentifier = parseTmp[2],
                        ProgramCustomerIdentifier = parseTmp[3],
                        MemberCustomerIdentifier = parseTmp[4],
                        PreviousMemberCustomerIdentifier = parseTmp[5],
                        MemberStatus = parseTmp[6],
                        FullName = parseTmp[7],
                        FirstName = parseTmp[8],
                        MiddleName = parseTmp[9],
                        LastName = parseTmp[10],
                        StreetLine1 = parseTmp[11],
                        StreetLine2 = parseTmp[12],
                        City = parseTmp[13],
                        State = parseTmp[14],
                        PostalCode = parseTmp[15],
                        Country = parseTmp[16],
                        PhoneNumber = parseTmp[17],
                        EmailAddress = parseTmp[18],
                        MembershipRenewalDate = parseTmp[19],
                        ProductIdentifier = parseTmp[20],
                        ProductTemplateField1 = parseTmp[21],
                        ProductTemplateField2 = parseTmp[22],
                        ProductTemplateField3 = parseTmp[23],
                        ProductTemplateField4 = parseTmp[24],
                        ProductTemplateField5 = parseTmp[25],
                        ProductRegistrationKey = parseTmp[26],
                        Message = "Success",
                        IsSuccess = true
                    });

                }
                else
                {
                    dynamic errorTmp = JObject.Parse(results);

                    string msgTmp = errorTmp.message;
                    if (msgTmp != null && msgTmp.Length > 0)
                    {
                        model.Message = $"Error: {msgTmp}";
                    }
                    else
                    {
                        model.Message = "Error: No reason provided";
                    }
                }

            }
            catch (Exception ex)
            {
                model.Message = ex.Message;
                model.IsSuccess = false;
            }

            return model;
        }

        public async Task<ADListReturnViewModel> GetMembers(int pageNumber = 1, int recordsPerPage = 10, string token = null)
        {
            string url = $"{_url}imports?page={pageNumber}&per_page={recordsPerPage}";
            ADListReturnViewModel adReturnObj = new ADListReturnViewModel();
            try
            {
                token ??= _token;

                (string results, bool isSuccess) = await SendGET(url, token);

                if(isSuccess && !results.Contains("message", StringComparison.CurrentCulture))
                {
                    adReturnObj = JsonConvert.DeserializeObject<ADListReturnViewModel>(results);
                    adReturnObj.Message = "Success";
                }
                else
                {
                    dynamic errorTmp = JObject.Parse(results);

                    string msgTmp = errorTmp.message;
                    if (msgTmp != null && msgTmp.Length > 0)
                        adReturnObj.Message = $"Error: {msgTmp}";
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
        private async Task<(string results, bool isSuccess)> SendGET(string uri, string token = null)
        {
            (string results, bool isSuccess) = ("Error: not implemented", false);

            if (token == null)
                token = _token;

            using (HttpClient client = new HttpClient() { BaseAddress = new Uri(uri) })
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                //client.DefaultRequestHeaders.Add("Content-type", "application/json");
                client.DefaultRequestHeaders.Add("Access-Token", $"{token}");

                using (HttpResponseMessage response = await client.GetAsync(uri))
                using (HttpContent content = response.Content)
                {
                    // ... Read the string.
                    results = await content.ReadAsStringAsync();
                    RawResponse = results;

                    if (response.IsSuccessStatusCode)
                        isSuccess = true;
                    else
                        isSuccess = false;
                    // ... Display the result.

                }
            }

            return (results, isSuccess);
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
