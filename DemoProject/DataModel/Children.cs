using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.DataModel
{
    public class Children
    {
        public string FirstName { get; set; } = String.Empty;
        public string Gender { get; set; } = String.Empty;
        public int Grade { get; set; }
        public List<Pets> Pets { get; set; } = new List<Pets>();
    }

    public class Pets
    {
        [JsonProperty("givenName")]
        public string GivenName { get; set; } = String.Empty;
    }

    public class Family
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("lastName")]
        public string LastName { get; set; } = String.Empty;
        [JsonProperty("children")]
        public List<Children> Children { get; set; } = new List<Children>();
        [JsonProperty("parents")]
        public List<Parents> Parents { get; set; } = new List<Parents>();      
        
        [JsonProperty("address")]
        public Address Address { get; set; } = new Address();
        [JsonProperty("isRegistered")]
        public bool IsRegistered { get; set; }
        public DateTime InsertedOn { get; set; }
        public string UpdatedOn { get; set; } = "";
    }

    public class Parents
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; } = String.Empty;
    }

    public class Address
    {
        [JsonProperty("state")]
        public string State { get; set; } = String.Empty;
        [JsonProperty("city")]
        public string City { get; set; } = String.Empty;
        [JsonProperty("county")]
        public string County { get; set; } = String.Empty ;
    }
}
