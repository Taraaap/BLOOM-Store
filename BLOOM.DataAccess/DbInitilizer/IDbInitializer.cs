using System;
using System.Collections.Generic;
using System.Text;

namespace BLOOM.DataAccess.DbInitilizer
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
