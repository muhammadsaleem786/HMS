#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HMS.Entities.CustomModel;

#endregion

namespace HMS.Service
{
    public interface IWhatsAppService
    {
         Task SendAppointmentMessage(string phoneNumber, string body,string PhoneNumberId,string AccessToken);
    }
}