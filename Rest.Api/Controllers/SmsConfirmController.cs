using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using LinkenLabs.Market.RestApi.Core;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class SmsConfirmController : ApiController
    {
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        [AcceptVerbs("GET")]
        [Route("~/v1/smsconfirm")]
        public IEnumerable<SmsConfirmationCode> Get()
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                return (from s in context.SmsConfirmationCodes
                        orderby s.Created descending
                        select s).ToList().Take(10);
            }
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/smsconfirm")]
        public void Post(SmsConfirmRequest model)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                SmsConfirmationCode code = new SmsConfirmationCode
                {
                    Guid = Guid.NewGuid(),
                    ConfirmationCode = Util.GenerateNumericCode(6),
                    Created = DateTime.UtcNow,
                    MobileNumber = model.MobileNumber
                };

                context.SmsConfirmationCodes.Add(code);
                context.SaveChanges();

                //send SMS
                string message = String.Format("SMS code {0}", code.ConfirmationCode);
                if (model.LanguageCode == "zh-TW")
                {
                    message = String.Format("短訊代碼 {0}", code.ConfirmationCode);
                }
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Code: " + code.ConfirmationCode);
#endif
                PostOffice.PostSmsPort.Post(new SmsMessage
                {
                    PhoneNumber = model.MobileNumber,
                    Message = message,
                    SenderId = "mooketplace"
                });


            }
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/smsconfirm/verify")]
        public bool Verify(VerifyMobileRequest model)
        {
            using (DatabaseContext context = Util.CreateContext())
            {

                var result = (from c in context.SmsConfirmationCodes
                              where c.MobileNumber == model.MobileNumber
                              orderby c.Created descending
                              select c).FirstOrDefault();

                if (result == null)
                {
                    return false;
                }
                return result.ConfirmationCode == model.Code;
            }
        }
    }
}