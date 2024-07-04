using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomm.DataAccess.Repository.IRepository
{
    public interface ISMSSenderService
    {
        Task SendSmsAsync(string number, string message);
    }
}
