using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.FollowUp.Job.Interfaces
{
    public interface ISmsService
    {
        Task AuthenticateAsync(string msisdn, string password, string mobile, string message, string mask, bool isUnicode);
        Task SendQuickMessageAsync(string sessionId, string recipients, string message, string mask, bool isUnicode);

    }
}
