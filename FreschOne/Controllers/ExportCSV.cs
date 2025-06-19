using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;

namespace FreschOne.Controllers
{
    public class ExportCSVController : BaseController
    {
        private readonly string _exportDirectory = @"C:\Users\Schalk.vandermerwe\OneDrive - Digital Outsource Services\Documents\ExportCSV\SixGaming";

        public ExportCSVController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        //private readonly string[] _tables = new[]
        //{
        //    "Audit_Adjustments",
        //    "Audit_BonusAmounts",
        //    "Audit_Cashbacks",
        //    "Audit_CashIns",
        //    "Audit_DepositsWithdrawals",
        //    "Audit_IncomePayouts",
        //    "Audit_Withdrawals_TransactionID_InvoiceID_Mapping",
        //    "player_balances_closing",
        //    "player_balances_closing_pending_withdrawals",
        //    "player_balances_opening",
        //    "player_balances_opening_pending_withdrawals"
        //};

        private readonly string[] _tables = new[]
        {
            "player_balances_closing_jan",
            "player_balances_closing_pending_withdrawals_jan",
            "player_balances_opening_jan",
            "player_balances_opening_pending_withdrawals_jan",
            "player_balances_closing_feb",
            "player_balances_closing_pending_withdrawals_feb",
            "player_balances_opening_feb",
            "player_balances_opening_pending_withdrawals_feb",
        };

        public IActionResult ExportAllTablesToCSV()
        {
            using var connection = GetConnection();
            connection.Open();

            foreach (var table in _tables)
            {
                var command = new SqlCommand($"SELECT * FROM {table}", connection);
                var adapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                var csvPath = Path.Combine(_exportDirectory, $"{table}.csv");
                using var writer = new StreamWriter(csvPath, false, Encoding.UTF8);

                // Write headers
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    writer.Write(dataTable.Columns[i]);
                    if (i < dataTable.Columns.Count - 1)
                        writer.Write(",");
                }
                writer.WriteLine();

                // Write rows
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        var value = row[i]?.ToString()?.Replace("\"", "\"\"");
                        writer.Write($"\"{value}\"");
                        if (i < dataTable.Columns.Count - 1)
                            writer.Write(",");
                    }
                    writer.WriteLine();
                }
            }

            return Ok("All tables exported successfully.");
        }
    }
}
