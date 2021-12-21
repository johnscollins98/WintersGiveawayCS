using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WintersGiveaway.Interfaces
{
    public interface IApiRequester
    {
        Task<T> MakeRequestAsync<T>(HttpRequestMessage message) where T : class;
    }
}
