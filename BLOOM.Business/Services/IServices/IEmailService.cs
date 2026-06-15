using System;
using System.Collections.Generic;
using System.Text;

namespace BLOOM.Business.Services.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent);

        Task<bool> SendOrderConfirmationEmailAsync(string toEmail, int OrderId, decimal orderTotal);
    }
}
