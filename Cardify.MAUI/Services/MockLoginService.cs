using Cardify.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardify.MAUI.Services
{
    public class MockLoginService : ILoginService
    {
        public Task<bool> LoginAsync(string email, string password)
        {
            // Doing this for now as we are not connected to the API
            bool isValid = email == "john.doe@example.com" && password == "password";
            return Task.FromResult(isValid);
        }
    }

}
