using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.BodyModel
{
    public class UsersViewModel
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string UserName { get; set; } = "";
        public string FullName { get; set; } = "";
        public string EmployeeId { get; set; } = string.Empty;
        public int AdminType { get; set; }
        public string CreatedOn { get; set; } = string.Empty;
        public string UpdatedOn { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
