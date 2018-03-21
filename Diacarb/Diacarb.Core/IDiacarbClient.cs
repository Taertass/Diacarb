using System;
using System.Threading.Tasks;

namespace Diacarb.Core
{
    public interface IDiacarbClient
    {
        IDietaryResult GetDietaryResult(string quantity, string unit, string foodName);
        Task<IDietaryResult> GetDietaryResultAsync(string quantity, string unit, string foodName);
        Task<IDietaryResult> GetDietaryResultAsync(string query);
    }
}
