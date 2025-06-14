using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cardify.Core.Services
{
    public interface ILoginService
    {
        Task<bool> LoginAsync(string email, string password);
    }
}
