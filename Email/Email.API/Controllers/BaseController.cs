using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using Email.API.BRL.Command;
using Email.API.BRL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Email.API.Controllers
{
    public class BaseController : Controller
    {
        private const string CLAIM_UNIQUE_NAME = "unique_name";
        private const string CLAIM_EMAIL = "email";
        private const string CLAIM_NAME_ID = "nameid";
        private const string CLAIM_USER_GUID = "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsuserclaim";

        private IEmailService _emailService;



        public IEmailService EmailService
        {
            get => _emailService ??
                   (_emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>());
            set => _emailService = value;
        }

        private string GetUser(string propertyName)
        {
            var authToken = Request.Headers["Authorization"][0].Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(authToken);

            if (token != null) return token.Payload[propertyName].ToString();

            return "No user information";
        }

        private string GetLoginWithoutDomain(string propertyName)
        {
            string userName = GetUser(propertyName);
            return Path.GetFileNameWithoutExtension(userName).ToLower();
        }

        public string UserName => GetUser(CLAIM_UNIQUE_NAME);

        public string UserLogin => GetUser(CLAIM_NAME_ID);

        public string UserGuid => GetUser(CLAIM_USER_GUID);

        public string UserEmail => GetUser(CLAIM_EMAIL);
        //CLAIM_EMAIL

        public string UserLoginWithoutDomain => GetLoginWithoutDomain(CLAIM_NAME_ID);

        //protected void LogError(ILogger<IController> logger, Exception ex)
        protected void LogError(ILogger<IController> logger, Exception ex, object parameter = null)
        {
            var stringBuilder = new StringBuilder();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Development") return;

            //EmailService.SendEmail(
            //    "Solar Administration Error [" + env + "]",
            //    BuildExceptionText(stringBuilder, "<h1>Solar Administration Error</h1>",
            //        GetUser(CLAIM_UNIQUE_NAME) + " | " + GetUser(CLAIM_NAME_ID),
            //        ex).ToString(),
            //    true
            //);


            //*********************************************************
            //App Insights log - turned off for now
            //*********************************************************
            //var customData = new List<KeyValuePair<string, object>>();
            //customData.Add(new KeyValuePair<string, object>("UserName", GetUser(CLAIM_UNIQUE_NAME)));
            //customData.Add(new KeyValuePair<string, object>("UserId", GetUser(CLAIM_NAME_ID)));

            //if (parameter != null) customData.Add(new KeyValuePair<string, object>("Parameter",JsonConvert.SerializeObject(parameter)));

            //logger.Log(
            //    LogLevel.Error,
            //    1,
            //    customData,
            //    ex,
            //    (s, e) => e.Message);

        }

        protected void LogClientSideError(ILogger<IController> logger, LogItem logItem)
        {
            //*********************************************************
            //App Insights log - turned off for now
            //*********************************************************
            //var customData = new List<KeyValuePair<string, object>>();
            //customData.Add(new KeyValuePair<string, object>("UserName", GetUser(CLAIM_UNIQUE_NAME)));
            //customData.Add(new KeyValuePair<string, object>("UserId", GetUser(CLAIM_NAME_ID)));
            //customData.Add(new KeyValuePair<string, object>("Url", logItem.Url));
            ////customData.Add(new KeyValuePair<string, object>("Parameter", logItem.Parameter));
            //customData.Add(new KeyValuePair<string, object>("StackTrace", logItem.Stack));

            //var exception = new ClientSideException(logItem.Stack);
            //var level = (LogLevel)Enum.Parse(typeof(LogLevel), logItem.Level);

            //logger.Log(
            //    level,
            //    1,
            //    customData,
            //    exception,
            //    (s, e) => "Angular Error : " + logItem.Message
            //);

        }

        private StringBuilder BuildExceptionText(StringBuilder stringBuilder, string title, string user, Exception exception)
        {
            stringBuilder.Append(title).Append("<h2>").Append(exception.Message).Append("</h2><br/>")
                .Append(exception.Source ?? "").Append("<hr/>");

            stringBuilder.Append(user).Append("<hr/>");

            if (exception.StackTrace != null)
            {
                stringBuilder.Append("<h3>Stack trace: </h3><br/>").Append(exception.StackTrace.Replace(Environment.NewLine, "<br/>"));
            }

            if (exception.InnerException != null)
            {
                BuildExceptionText(stringBuilder, "<h2>Inner exception </h2>", "", exception.InnerException);
            }

            return stringBuilder;
        }

    }
}