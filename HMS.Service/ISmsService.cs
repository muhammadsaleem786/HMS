#region
using System.Collections.Generic;
using System.Threading.Tasks;
using HMS.Entities.CustomModel;

#endregion

namespace HMS.Service
{
    public interface ISmsService
    {
        Task AuthenticateAsync(string msisdn, string password, string mobile, string message, string mask,bool isUnicode);
        Task SendQuickMessageAsync(string sessionId, string recipients, string message, string mask, bool isUnicode);

    }
}