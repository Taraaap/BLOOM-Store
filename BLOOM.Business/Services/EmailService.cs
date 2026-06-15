using BLOOM.Business.Services.IServices;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BLOOM.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _SecretKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["Mailjet:ApiKey"];
            _SecretKey = _configuration["Mailjet:SecretKey"];
            _senderEmail = _configuration["Mailjet:SenderEmail"];
            _senderName = _configuration["Mailjet:SenderName"];

        }

       public async Task<bool> SendEmailAsync (string toEmail, string subject, string htmlContent)
        {
            try
            {
                MailjetClient client = new MailjetClient(_apiKey, _SecretKey);

                var email = new TransactionalEmailBuilder()
                    .WithFrom(new SendContact(_senderEmail, _senderName))
                    .WithTo(new SendContact(toEmail))
                    .WithSubject(subject)
                    .WithHtmlPart(htmlContent)
                    .Build();

                var response = await client.SendTransactionalEmailAsync(email);

                if(response.Messages != null && response.Messages.Length > 0)
                {
                    var message= response.Messages[0];
                    if (message.Status == "success")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public async Task<bool> SendOrderConfirmationEmailAsync(string toEmail, int OrderId, decimal orderTotal)
        {
            var subject = $"Order Confirmation #{OrderId}-Bloom";

            //Simple html email to demonstrate email functionality

            var htmlContent = $@"
            <h1>Thank you for your order!</H1>
             <p> Your Order has been placed succesfully.</p>
            <hr>
            <p><strong> Order Number:</strong>{OrderId}</p>
            <p><strong> Order Date:</strong>{DateTime.Now:MMM dd, yyy}</p>
            <p><strong> Total Amount:</strong>{orderTotal} Rs</p>
            <hr>
            <p> Thank you for shopping with Bloom Store!</p>
            <p>- The Bloom Store Team</P> ";

            return await SendEmailAsync(toEmail, subject, htmlContent);
        }

       
    }
}
