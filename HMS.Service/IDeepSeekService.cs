using HMS.Entities.CustomModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Service
{
    public interface IDeepSeekService
    {
        Task<string> GeneratePrescription(DeepSeekRequest model);
    }
}
