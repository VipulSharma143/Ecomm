using Ecomm.DataAccess.Repository.IRepository;
using Ecomm.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Base;
using Twilio.Rest.Api.V2010.Account;

namespace Ecomm.DataAccess.Repository
{
    public class SMSSenderService : ISMSSenderService
    {
        private readonly TwilioSettings _settings;
        public SMSSenderService(Microsoft.Extensions.Options.IOptions<TwilioSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendSmsAsync(string number, string message)
        {
            TwilioClient.Init(_settings.AccountSId, _settings.AuthToken);
            await MessageResource.CreateAsync(
                to: number,
                from: _settings.FromPhoneNumber,
                body: message);
        }
    }
}

