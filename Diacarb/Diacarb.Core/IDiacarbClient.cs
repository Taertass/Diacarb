using System;
using System.Threading.Tasks;

namespace Diacarb.Core
{
    public interface IDiacarbClient
    {
        Task<IDietaryResult> GetDietaryResultAsync(string query);
    }
}
