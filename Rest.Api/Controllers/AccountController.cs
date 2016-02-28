using Facebook;
using LinkenLabs.Market.Core;
using LinkenLabs.Market.RestApi.Core;
using LinkenLabs.Market.RestApi.Filters;
using Newtonsoft.Json;
using PWDTK_DOTNET451;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Http;
using Image = System.Drawing.Image;

namespace LinkenLabs.Market.RestApi.Controllers
{
    public class AccountController : ApiController
    {
        // GET api/<controller>
        [BasicAuthentication]
        [AcceptVerbs("GET")]
        [Route("~/v1/account")]
        public AccountInfo Get()
        {
            Account account = GetAccountByUsername(User.Identity.Name);
            return ConvertToAccountInfo(account);
        }

        internal AccountInfo ConvertToAccountInfo(Account account)
        {
            string[] roles = JsonConvert.DeserializeObject<string[]>(account.Roles);

            return new AccountInfo
            {
                Guid = account.Guid,
                Username = account.Username,
                FacebookUserId = account.FacebookUserId,
                Phone = account.Phone,
                Email = account.Email,
                Role = roles[0]
            };
        }

        [BasicAuthentication]
        public AccountInfo Get(Guid id)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var account = (from a in context.Accounts
                               where a.Guid == id
                               select a).FirstOrDefault();

                if (account == null)
                {
                    return null;
                }

                return ConvertToAccountInfo(account);
            }
        }

        [AcceptVerbs("GET")]
        [Route("~/v1/account/{accountGuid}/picture")]
        public HttpResponseMessage GetProfileImage(Guid accountGuid)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                if (accountGuid == Guid.Empty)
                {
                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/resources/cowLogo.jpg");
                    var response = new HttpResponseMessage { Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    return response;
                }

                Account account = (from a in context.Accounts.AsNoTracking()
                                   where a.Guid == accountGuid
                                   select a).FirstOrDefault();

                if (account == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                if (String.IsNullOrEmpty(account.FacebookUserId))
                {
                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/resources/avatar.jpg");
                    var response = new HttpResponseMessage { Content = new StreamContent(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    return response;
                }
                else
                {
                    string facebookProfileUrl = String.Format(@"https://graph.facebook.com/{0}/picture", account.FacebookUserId);
                    byte[] imageBytes = GetImageFromUrl(facebookProfileUrl);

                    HttpResponseMessage response = new HttpResponseMessage
                    {
                        Content = new StreamContent(new MemoryStream(imageBytes))
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    return response;
                }

            }
        }

        private static byte[] GetImageFromUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            using (WebResponse result = request.GetResponse())
            {
                using (Stream stream = result.GetResponseStream())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        return memoryStream.GetBuffer();
                    }
                }
            }
        }

        internal static Account GetAccountByUsername(string username)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                //try searching for username
                var result = (from a in context.Accounts
                              where a.Username == username || a.FacebookUserId == username
                              && a.IsActive
                              select a).FirstOrDefault();

                return result;
            }
        }

        internal Account GetAccountByPhoneNumber(string phoneNumber)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                //try searching for email first
                var result = (from a in context.Accounts
                              where a.Phone == phoneNumber
                              && a.IsActive
                              select a).FirstOrDefault();

                return result;
            }
        }


        [AcceptVerbs("GET")]
        [Route("~/v1/account/getAccounts")]
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IEnumerable<AccountInfo> GetAccounts()
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                return (from a in context.Accounts.AsNoTracking()
                        where a.IsActive
                        select a).ToList().ConvertAll(a => new AccountInfo
                        {
                            Guid = a.Guid,
                            Username = a.Username,
                            FacebookUserId = a.FacebookUserId,
                            Phone = a.Phone,
                            Email = a.Email
                        });
            }
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/account/phoneExists")]
        public bool PhoneExists(PhoneExistsRequest model)
        {
            if (model == null || String.IsNullOrEmpty(model.PhoneNumber))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string pattern = @"^\+(853|852)[0-9]{8}$";
            if (!Regex.IsMatch(model.PhoneNumber, pattern))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            using (DatabaseContext context = new DatabaseContext())
            {
                var account = (from a in context.Accounts
                               where a.Phone == model.PhoneNumber
                               select a).FirstOrDefault();

                return account != null;
            }
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/account/userNameExists")]
        public bool UserNameExists(UserNameExistsRequest model)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var account = (from a in context.Accounts
                               where a.Username == model.UserName
                               select a).FirstOrDefault();
                return account != null;
            }
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/account/facebookUserIdExists")]
        public bool FacebookUserIdExists(FacebookUserIdExistsRequest request)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var account = (from a in context.Accounts
                               where a.FacebookUserId == request.UserId
                               select a).FirstOrDefault();

                return account != null;
            }
        }

        [AcceptVerbs("POST")]
        [Route("~/v1/account/verifyCredentials")]
        public bool VerifyCredentials(VerifyCredentialsRequest model)
        {
            if (model == null || String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            using (DatabaseContext context = new DatabaseContext())
            {
                Account account = (from a in context.Accounts
                                   where model.Username == a.Username || model.Username == a.FacebookUserId
                                   select a).FirstOrDefault();

                if (account == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                //test access token login
                if (model.Username == account.FacebookUserId)
                {
                    return FacebookVerifyCredentials(account.FacebookUserId, model.Password);
                }

                //user has facebook account, but attempting password.
                if (model.Username == account.Username && !String.IsNullOrEmpty(account.FacebookUserId))
                {
                    return false;
                }

                var saltBytes = PWDTK.HashHexStringToBytes(account.Salt);
                var passwordBytes = PWDTK.HashHexStringToBytes(account.PasswordHash);
                return PWDTK.ComparePasswordToHash(saltBytes, model.Password, passwordBytes);
            }
        }

        static bool FacebookVerifyCredentials(string userId, string accessToken)
        {
            try
            {
                var client = new FacebookClient(accessToken);
                dynamic response = client.Get("me", new { fields = "id" });
                if (!String.Equals(response.id, userId))
                {
                    return false;
                }
                return true;
            }
            catch (FacebookOAuthException)
            {
                return false;
            }
        }

        [AcceptVerbs("PUT")]
        [Route("~/v1/account/{id}/deleteaccount")]
        [BasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public bool DeleteAccount(Guid id)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var account = (from a in context.Accounts where a.Guid == id select a).FirstOrDefault();
                if (account == null)
                {
                    return false;
                }

                account.IsActive = false;

                var orders = (from o in context.Orders
                              where o.CreatedByAccountGuid == id
                              select o).ToList();

                orders.ForEach(o =>
                {
                    o.IsCancelled = true;
                    o.LastModified = DateTime.UtcNow;
                });

                context.SaveChanges();
                return true;
            }
        }
        // POST api/<controller>

        [AcceptVerbs("POST")]
        [Route("~/v1/account")]
        public Guid Post(AccountCreateInfo model)
        {
            string[] languageCodes = new string[] { "en-US", "zh-TW" };
            if (!languageCodes.Contains(model.LanguageCode))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Phone))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (String.IsNullOrEmpty(model.Password) && String.IsNullOrEmpty(model.FacebookUserId)) //needs to have username password, or facebook userId
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            //confirm mobile
            if (!new SmsConfirmController().Verify(new VerifyMobileRequest { MobileNumber = model.Phone, Code = model.SmsCode }))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (UserNameExists(new UserNameExistsRequest { UserName = model.Username }))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return CreateAccount(model);
        }

        internal Guid CreateAccount(AccountCreateInfo model, bool isAdmin = false)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                string passwordHash = "";
                string salt = "";
                if (String.IsNullOrEmpty(model.FacebookUserId)) //if not a facebook user, hex password.
                {
                    salt = PWDTK.GetRandomSaltHexString();
                    byte[] saltBytes = PWDTK.HashHexStringToBytes(salt);
                    passwordHash = PWDTK.PasswordToHashHexString(saltBytes, model.Password);
                }

                string role = isAdmin ? "Administrator" : "User";

                Account account = new Account
                {
                    Guid = Guid.NewGuid(),
                    Username = model.Username,
                    FacebookUserId = model.FacebookUserId,
                    Salt = salt,
                    PasswordHash = passwordHash,
                    Roles = JsonConvert.SerializeObject(new string[] { role }),
                    Phone = model.Phone,
                    LanguageCode = model.LanguageCode,
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow
                };

                context.Accounts.Add(account);
                context.SaveChanges();
                return account.Guid;
            }
        }

        // PUT api/<controller>/5
        [BasicAuthentication]
        public void Put(Guid id, AccountInfo model)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var account = (from a in context.Accounts
                               where a.Guid == id
                               select a).FirstOrDefault();

                if (!User.IsInRole("Administrator") &&
                    !(String.Equals(account.Username, User.Identity.Name) || String.Equals(account.FacebookUserId, User.Identity.Name)))
                {

                    return;
                }

                account.Email = model.Email;
                account.Phone = model.Phone;
                context.SaveChanges();
            }
        }

        //[BasicAuthentication]
        //public void Put(AccountInfo model)
        //{
        //    using (DatabaseContext context = Util.CreateContext())
        //    {
        //        if (!String.Equals(User.Identity.Name, model.Name))
        //        {
        //            return;
        //        }

        //        string salt = PWDTK.GetRandomSaltHexString();
        //        byte[] saltBytes = PWDTK.HashHexStringToBytes(salt);

        //        string passwordHash = PWDTK.PasswordToHashHexString(saltBytes, model.Password);

        //        var account = (from a in context.Accounts
        //                       where a.Email == model.Name || a.Phone == model.Name
        //                       select a).FirstOrDefault();

        //        account.Salt = salt;
        //        account.PasswordHash = passwordHash;

        //        context.SaveChanges();
        //    }
        //}

        [AcceptVerbs("PUT")]
        [Route("~/v1/account/resetpassword")]
        [NoAuthentication]
        public bool Put(AccountPasswordResetModel model)
        {
            using (DatabaseContext context = Util.CreateContext())
            {
                var smsConfirmationCode = (from c in context.SmsConfirmationCodes
                                           where c.MobileNumber == model.MobileNumber
                                           orderby c.Created descending
                                           select c).FirstOrDefault();

                if (smsConfirmationCode == null || smsConfirmationCode.ConfirmationCode != model.Code)
                {
                    return false;
                }

                string salt = PWDTK.GetRandomSaltHexString();
                byte[] saltBytes = PWDTK.HashHexStringToBytes(salt);

                string passwordHash = PWDTK.PasswordToHashHexString(saltBytes, model.Password);

                var account = (from a in context.Accounts
                               where a.Email == model.MobileNumber || a.Phone == model.MobileNumber
                               select a).FirstOrDefault();

                if (account == null)
                {
                    return false;
                }

                account.Salt = salt;
                account.PasswordHash = passwordHash;
                context.SmsConfirmationCodes.Remove(smsConfirmationCode);
                context.SaveChanges();
                return true;
            }
        }

    }
}