using System.Threading;
using System.Threading.Tasks;

namespace FreschOne.Models
{
    public class DefaultFormRefresher : IFormRefresher
    {
        public Task RefreshAsync(FormTable table, string changedField, CancellationToken token)
        {
            if (table.TableName == "tbl_tran_student")
            {
                var first = table.Fields["FirstName"].Value?.ToString();

                if (first == "Schalk")
                {
                    table.Fields["LastName"].Visible = true;
                    table.Fields["LastName"].Value = "van der merwe";
                }
                else
                {
                    table.Fields["LastName"].Visible = false;
                    table.Fields["LastName"].Value = "";
                }
            }

            return Task.CompletedTask;
        }
    }
}
