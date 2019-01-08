using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Email.API.BRL.Command;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Email.Test
{
    [TestClass]
    public class UnitTest1
    {
        //************************* UPDATE TOKEN ***********************************************************************************
        private string token = @"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJMQU5DU1JFXFxhbWl0Y2hlbGwiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3dpbmRvd3N1c2VyY2xhaW0iOiIyMDViYjRmZC1kOGQzLTQ3N2ItODVhNC00NTQxMWMwNDc0MTYiLCJlbWFpbCI6ImFuZHJldy5taXRjaGVsbEBsYW5jYXNoaXJlZ3JvdXAuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbG9jYWxpdHkiOiJMT04iLCJ1bmlxdWVfbmFtZSI6IkFuZHJldyBNaXRjaGVsbCIsInJvbGUiOlsiREVQVF9JVCIsIkRFUFRfTU9ERUxMSU5HIiwiRklOX0FETUlOX1BXUl9VU0VSIiwiSERfQURNSU4iLCJIRF9ERVBMT1lfSVQiLCJIRF9ERVBMT1lfSVRfREIiLCJIRF9ERVBMT1lfSVRfSFciLCJIRF9ERVBMT1lfT1BTIiwiSERfREVQTE9ZX1dJWkkiLCJIRF9JTVBMRU1FTlRfR1JDIiwiSERfSU1QTEVNRU5UX0lUIiwiSERfSU1QTEVNRU5UX0lUX0hXIiwiSERfSU1QTEVNRU5UX01PREVMIiwiSERfSU1QTEVNRU5UX09QUyIsIkhEX0lNUExFTUVOVF9XSVpJIiwiSVRfVVNFUl9BRE1JTiIsIk1XRl9BRE1JTiIsIk1XRl9VU0VSIiwiUklNQU5fVVNFUiIsIlRBU0tfQURNSU4iLCJVU0VSX1BIT05FX0FETUlOIiwiVVdfQ0xBSU1fQ0FTSF9NQVRDSCJdLCJuYmYiOjE1NDUzMjU2ODgsImV4cCI6MTU0NTMyOTI4OCwiaWF0IjoxNTQ1MzI1Njg4fQ.QygjKMPAVr_3_DkMUioTcmt81-MAHnVsJVBG2pEj-pI";
        //**********************************************************************************************************************

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(2, 1 +1);
            
        }

      //  [TestMethod]
        public async Task TestSend()
        {
            var email = new API.BRL.Command.Email()
            {
                To = new List<string>() {"yes@no.com"},
                From = "yes@no.com",
                Subject = "Unit test",
                Message = "Unit test",
                IsHtml = true
            };

            var postContent = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");


            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                token);

            using (HttpResponseMessage res = await client.PostAsync("http://localhost/Email.API/api/email", postContent))
            using (HttpContent content = res.Content)
            {
                string data = await content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<EmailResponse>(data);
            }
        }


     //   [TestMethod]
        public async Task TestSendFile()
        {
            byte[] b = System.IO.File.ReadAllBytes(@"C:\test.csv");
            var url = @"http://localhost/Email.API/api/emailattachment";

            ByteArrayContent bytes = new ByteArrayContent(b);

            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            
            multiContent.Add(bytes, "Attachment", "test.csv");

            multiContent.Add(new StringContent("yes@no.com"), "From");

            multiContent.Add(new StringContent("yes@no.com"), "To[0]");
            multiContent.Add(new StringContent("yes@no.com"), "To[1]");

            multiContent.Add(new StringContent("Test Email - Attachment-" + DateTime.Now.ToString()), "Subject");
            multiContent.Add(new StringContent("-- test email attachment --"), "Message");
            multiContent.Add(new StringContent("true"), "IsHtml");

            multiContent.Add(new StringContent("yes@no.com"), "Cc");

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("text/csv"));

                using (HttpResponseMessage res = await client.PostAsync(url, multiContent))
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<EmailResponse>(data);
                }
            }
            
        }

        private async Task<string> getToken()
        {
            HttpClient _httpClient = new HttpClient();

            var tokenServerUrl = @"http:/token-server/api/token";

            using (HttpResponseMessage res = await _httpClient.PostAsync(tokenServerUrl, null))
            using (HttpContent content = res.Content)
            {
                var data = await content.ReadAsStringAsync();

                var token = JsonConvert.DeserializeObject<Token>(data);

                return token.AccessToken;
            }
        }
    }

    public class Token
    {
        public string AccessToken { get; set; }
        public string Type { get; set; }
    }
}
