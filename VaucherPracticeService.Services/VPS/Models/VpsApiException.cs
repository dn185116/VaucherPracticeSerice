using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaucherPracticeService.Services.VPS.Models
{
    public class VpsApiException : Exception
    {
        public VpsApiException()
        {
        }

        public VpsApiException(string message) : base(message)
        {
        }

        public VpsApiException(string message, Exception inner) : base(message, inner)
        {
        }
    }

}
