using BLOOM.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLOOM.Business.Services.IServices
{
    public interface IApplicationUserService
    {
        Task<ApplicationUser?> GetUserByIdAsync(string userId);

    }
}
