using System.Threading;
using System.Threading.Tasks;

namespace FreschOne.Models
{
    public interface IFormRefresher
    {
        Task RefreshAsync(FormTable table, string changedField, CancellationToken token);
    }
}
