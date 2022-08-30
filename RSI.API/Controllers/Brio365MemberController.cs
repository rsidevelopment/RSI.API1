using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AccessDevelopment.Services.Models._ViewModels;
using Legacy.Services.Interfaces;
using Legacy.Services.Models._ViewModels.Member;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RSI.API.Controllers
{
    [Route("api/[controller]")]
    public class Brio365MemberController : Controller
    {
        private readonly IMemberService _context;

        public Brio365MemberController(IMemberService context)
        {
            _context = context;
        }

        

        private async Task<JObject> _ParseSend(MemberViewModel model)
        {
            (bool isSuccess, string message, int accessDevelopmentId) returnObj = (false, "", 0);
            dynamic objReturn = new JObject();

            try
            {
                returnObj = await _context.AddAccessDevelopmentAudit(model);

                objReturn.is_success = returnObj.isSuccess;
                objReturn.message = returnObj.message;
                objReturn.access_development_id = returnObj.accessDevelopmentId;
            }
            catch (Exception ex)
            {
                objReturn.is_success = false;
                objReturn.message = ex.Message;
                objReturn.access_development_id = 0;
            }

            return objReturn;
        }

        // GET api/<controller>/5
        [HttpGet("{id}/{type?}")]
        public async Task<JObject> Get(int id, string type = "shopping")
        {
            dynamic objReturn = new JObject();
            string shoppingToken = "d41d64b3158ad709fa37fb1a3f5928a4cd921ea0fc48bc0dbce59dd334ba5205";
            string diningToken = "139a3635a78eabc414bd318cc529199048a863b641bed45c64cb4578be71a663";
            string shoppingProgramId = "200938";
            string diningProgramId = "200939";
            string organizationId = "2002598";

            try
            {
                MemberViewModel model = new MemberViewModel();

                MemberInfoViewModel m = await _context.GetMemberAsync(id);

                if (m != null && m.PrimaryMember.RSIId > 0)
                {
                    MemberViewModel member = new MemberViewModel()
                    {
                        MemberStatus = "OPEN",
                        LastName = m.PrimaryMember.LastName,
                        FirstName = m.PrimaryMember.FirstName,
                        Email = m.PrimaryMember.Email,
                        Address1 = m.PrimaryMember.Address1,
                        Address2 = m.PrimaryMember.Address2,
                        City = m.PrimaryMember.City,
                        Country = m.PrimaryMember.Country,
                        MiddleName = m.PrimaryMember.MiddleName,
                        OrganizationCustomerIdentifier = organizationId,
                        Phone = m.PrimaryMember.HomePhone != null && m.PrimaryMember.HomePhone.Length > 0 ? m.PrimaryMember.HomePhone : m.PrimaryMember.MobilePhone,
                        PostalCode = m.PrimaryMember.PostalCode,
                        ProgramCustomerIdentifier = type == "shopping" ? shoppingProgramId : diningProgramId,
                        RSIId = id,
                        RecordIdentifier = null,
                        State = m.PrimaryMember.State
                    };

                    objReturn = await _ParseSend(member);

                    var isSuccessTmp = objReturn.is_success;

                    if (isSuccessTmp != null && isSuccessTmp.ToString().Length > 0 && bool.TryParse(isSuccessTmp.ToString(), out bool isSuccess))
                    {
                        string url = "";
                        string nonHashedToken = "";

                        if (type == "shopping")
                        {
                            nonHashedToken = $"{organizationId}-{shoppingProgramId}-{id}";
                            url = "https://brio365.brioresorts.com/?cvt=";
                        }
                        else
                        {
                            nonHashedToken = $"{organizationId}-{diningProgramId}-{id}";
                            url = "https://dining.travnow.com/home/?cvt=";
                        }

                        StringBuilder hashString = new StringBuilder();
                        using (SHA1Managed sha1 = new SHA1Managed())
                        {
                            byte[] bytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(nonHashedToken));
                            for (var i = 0; i < bytes.Length; i++)
                                hashString.Append(bytes[i].ToString("x2"));

                        }
                            
                        
                        //string hashString = Encoding.Default.GetString(bytes);
                        //StringBuilder hashString = new StringBuilder();
                        /*for(int i = 0; i < bytes.Length; i++)
                        {
                            hashString.Append(bytes[i].ToString());
                        }*/

                        objReturn.url = url + hashString;
                        objReturn.is_success = true;
                        objReturn.message = "Success";
                    }
                    else
                    {
                        objReturn.url = "";
                        objReturn.is_success = false;
                        objReturn.message = objReturn.ToString();
                    }
                }
                else
                {
                    objReturn.url = "";
                    objReturn.is_success = false;
                    objReturn.message = "Member not found";
                }
                
            }
            catch (Exception ex)
            {
                objReturn.url = "";
                objReturn.is_success = false;
                objReturn.message = ex.Message;
            }

            return objReturn;
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<JObject> Post([FromBody]JObject value)
        {
            (bool isSuccess, string message, int accessDevelopmentId) returnObj = (false, "", 0);
            dynamic objReturn = new JObject();

            try
            {
                MemberViewModel model = value.ToObject<MemberViewModel>();

                objReturn = await _ParseSend(model);
                
            }
            catch (Exception ex)
            {

                objReturn.is_success = false;
                objReturn.message = ex.Message;
                objReturn.access_development_id = 0;

                
            }

            return objReturn;
        }

        
    }
}
