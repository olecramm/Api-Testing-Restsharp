using System.Collections.Generic;
using RestSharp;
using RestSharp.Serialization.Json;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using SpecflowFirst.Model;
using System.Threading.Tasks;
using System;

namespace SpecflowFirst
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts/{postid}", Method.GET);

            request.AddUrlSegment("postid", 1);

            var response = client.Execute(request);

            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];

            JObject obs = JObject.Parse(response.Content);

            Assert.That(obs["author"].ToString(), Is.EqualTo("karthik KK"), "Authos is not correct");
        }

        [TestCase ("teste")]
        public void PostWithAnonymousBody(string teste)
        {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts/{postid}/profile", Method.POST);

            request.RequestFormat = DataFormat.Json;

            request.AddBody(new { name = "Marcelo" });
            request.AddUrlSegment("postid", 1);      

            var response = client.Execute(request);

            var deserialize = new JsonDeserializer();
            var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            var result = output["name"];

            Assert.That(result, Is.EqualTo("Marcelo"), "Name is different");
        }
        [Test]
        public void PostWithTypeClassBody()
        {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts", Method.POST);

            request.RequestFormat = DataFormat.Json;

            request.AddBody(new Posts() { id ="17", title= "Teste Automation New", author="Marcelo Teste" });
            request.AddUrlSegment("postid", 1);

            var response = client.Execute(request);

            var deserialize = new JsonDeserializer();
            var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            var result = output["author"];

            Assert.That(result, Is.EqualTo("Marcelo Teste"), "Name is different");
        }

        [Test]
        public void PostWithTypeClassBodyWithGenerics()
        {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts", Method.POST);

            request.RequestFormat = DataFormat.Json;

            request.AddBody(new Posts() { id = "20", title = "Teste Automation New", author = "Marcelo Teste 2" });
            request.AddUrlSegment("postid", 1);

            var response = client.Execute<Posts>(request);

            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];

            Assert.That(response.Data.author, Is.EqualTo("Marcelo Teste 2"), "Name is different");
        }
        [Test]
        public void PostWithTypeClassBodyWithAsysc()
        {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts", Method.POST);

            request.RequestFormat = DataFormat.Json;

            request.AddBody(new Posts() { id = "25", title = "Teste Automation New", author = "Marcelo Teste 4" });
            request.AddUrlSegment("postid", 1);

            //var response = client.ExecuteAsync<Posts>(request);

            var result = ExecuteAsyncRequest<Posts>(client, request).GetAwaiter().GetResult();

            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];

            Assert.That(result.Data.author, Is.EqualTo("Marcelo Teste 4"), "Name is different");
        }

        private async Task<IRestResponse<T>> ExecuteAsyncRequest<T>(RestClient client, IRestRequest request) where T: class, new()
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();

            client.ExecuteAsync<T>(request, restResponse =>
            {
                if (restResponse.ErrorException != null)
                {
                    const string message = "Error Retrieving response.";
                    throw new ApplicationException(message, restResponse.ErrorException);
                }

                taskCompletionSource.SetResult(restResponse);
            });

            return await taskCompletionSource.Task;
        }
    }
}