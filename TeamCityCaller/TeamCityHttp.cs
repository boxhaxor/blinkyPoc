using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TeamCityCaller
{
    using System.Text;

    public class TeamCityResult
    {
        public TeamCityStatus? PreviousStatus { get; set; }
        public TeamCityStatus CurrentStatus { get; set; }
    }
    public enum TeamCityStatus
    {
        Building,
        Success,
        Failed
    }
    public class TeamCityHttp
    {
        private readonly string _urlForTeamcityAgent;
        private readonly string _username;
        private readonly string _password;
        private readonly string _projectToLookFor;

        public TeamCityHttp(string urlForTeamcityAgent, string username, string password, string projectToLookFor)
        {
            _urlForTeamcityAgent = urlForTeamcityAgent;
            _username = username;
            _password = password;
            _projectToLookFor = projectToLookFor;
        }

        public TeamCityResult GetTeamCityResult()
        {
            var returnResult = new TeamCityResult();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", this._username, this._password));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(byteArray));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string response = string.Empty;
                    Task.Run(async () =>
                    {
                        response = await httpClient.GetStringAsync(this._urlForTeamcityAgent);

                    }).Wait();

                }
            }
            catch (Exception c)
            {
                //hide
                Console.WriteLine(c);
            }
            return returnResult;
        }

    }
}
