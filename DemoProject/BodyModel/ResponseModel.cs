using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoProject.BodyModel
{
    public class ResponseModel
    {
        public bool DidError { get; set; } = false;
        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
        public dynamic? Model { get; set; }
    }
}
