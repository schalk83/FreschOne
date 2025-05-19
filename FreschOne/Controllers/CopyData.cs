using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FreschOne.Controllers
{
    public class DataCopyController : Controller
    {
        // ✅ Hardcoded connection strings with Windows Authentication
        private string SourceConnectionString =>
                   "Server=CPTFINDB01;Database=dbJumpman;Trusted_Connection=True;TrustServerCertificate=True;";

        private string TargetConnectionString =>
            "Server=DUBAOLSTN01;Database=dbJumpman;Trusted_Connection=True;TrustServerCertificate=True;";

        private readonly Dictionary<string, string> queries = new()
        {
            // ["account_balance_snapshot_1_2"] = @"SELECT [DateKey], [account_id], [real_balance], [bonus_balance], [snapshot_timestamp], [currency], [snapshot_date], [ModifiedDate], [Operation] FROM account_balance_snapshot_1_2 with (nolock) WHERE DateKey >= 20250201",
            ["slots_latest_1_0"] = @"SELECT [DateKey], [id], [name], [fixed_name], [network], [sub_network], [gameid], [gameid_low], [bonus_gameid], [bonus_mobile_gameid], [mobile_gameid], [mobile_gameid_low], [minigame], [freespins], [freespins_first], [game_type], [leaderboard], [status], [date_added], [wager_requirement], [snapshot_timestamp], [ModifiedDate], [Operation] FROM slots_latest_1_0 WHERE DateKey >= 20250101 AND DateKey < 20250201",
            ["player_event_dims_1_0"] = @"SELECT [DateKey], [user_account], [Opened], [Active], [FirstPlayed], [ModifiedDate], [Operation] FROM player_event_dims_1_0 WHERE DateKey >= 20250101 AND DateKey < 20250201 ",
            ["account_1_2"] = @"SELECT [DateKey], [id], [address_country], [address_region], [registration_date], [currency], [deposit_limit_daily], [deposit_limit_weekly], [deposit_limit_monthly], [deposit_limit_set], [withdraw_limit_daily], [withdraw_limit_weekly], [withdraw_limit_monthly], [subaff_id], [ftd_date], [brand_name], [brand_type], [brand_internal_brand], [snapshot_timestamp], [inserted_on], [ModifiedDate], [Operation] FROM account_1_2 WHERE DateKey >= 20250101 AND DateKey < 20250201",
            ["psp_transaction_1_0"] = @"SELECT [DateKey], [transaction_date], [transaction_timestamp], [user_account], [currency], [description], [category], [transaction_value], [jms_transaction_id], [ledger_transaction_guid], [ModifiedDate], [Operation] FROM psp_transaction_1_0 WHERE DateKey >= 20250101 AND DateKey < 20250201",
            ["psp_brand_transaction_1_0"] = @"SELECT [DateKey], [transaction_sale_date], [sale_date], [request_date], [id], [transaction_type], [user_id], [brand], [currency], [amount], [bonus], [status], [OrderIDReference], [Processor], [ModifiedDate], [Operation] FROM psp_brand_transaction_1_0 WHERE DateKey >= 20250101 AND DateKey < 20250201",
            ["revenue_summary_hourly_1_0"] = @"SELECT [reporting_date], [reporting_datetime], [user_account], [currency], [account_type], [description], [category], [brand], [network], [gamecode], [total_value], [total_transactions], [Datekey], [ModifiedDate], [Operation] FROM revenue_summary_hourly_1_0 WHERE DateKey >= 20250101 AND DateKey < 20250201",
            ["adjustments_summary_hourly_1_0"] = @"SELECT [DateKey], [reporting_date], [user_account], [currency], [account_type], [description], [category], [total_value], [total_transactions], [ModifiedDate], [Operation] FROM adjustments_summary_hourly_1_0 WHERE DateKey >= 20250101 AND DateKey < 20250201",
            ["Gamesmapping"] = @"SELECT [UniqueID], [gamecode], [id], [name], [network], [game_type], [FromDate], [ToDate], [ModifiedDate], [Operation], [sub_network] FROM Gamesmapping ",
            ["Player"] = @"SELECT [id], [address_country], [address_region], [CleanRegion], [RegionCode], [registration_date], [currency], [brand_name], [brand_type], [Category], [FromDate], [ToDate] FROM Player ",
            ["CurrencyConversion"] = @"SELECT DateKey,SourceCurrencyID,DestinationCurrencyID,SourceDestinationExchRate FROM dbMasterData..CurrencyConversion WHERE DateKey >= 20250101 AND DateKey < 20250201",
            ["brand_1_1"] = @"SELECT [DateKey], [id], [name], [email], [url], [company_name], [address], [theme], [path], [affiliate_portal], [paysafe_id], [internal_brand], [cloudflare_zone], [minify], [date_added], [type], [altglobal], [sms_sender_name], [domain_name], [reg_sms], [status], [aff_email], [cashback_sms], [trophy_freespins_sms], [account_reopen_sms], [withdrawal_sms], [welcome_offer_theme], [istaging], [region_id], [snapshot_timestamp], [inserted_on], [ModifiedDate], [Operation] FROM brand_1_1",
            ["PSP_OrderID_Lookup"] = @"SELECT id, user_id , transaction_type, OrderIDReference, Processor, DateKey,ModifiedDate, Operation, Brand from PSP_OrderID_Lookup where DateKey >= 20250101 AND DateKey < 20250501"
        };

        // 🔹 View with table buttons
        public IActionResult Index()
        {
            var tableInfo = new List<(string TableName, int SourceCount, int TargetCount)>();

            using var sourceConn = new SqlConnection(SourceConnectionString);
            using var targetConn = new SqlConnection(TargetConnectionString);
            sourceConn.Open();
            targetConn.Open();

            foreach (var kvp in queries)
            {
                int sourceCount = 0, targetCount = 0;

                try
                {
                    var sourceCmd = new SqlCommand($"SELECT COUNT(*) FROM ({kvp.Value}) AS src WHERE DateKey >= 20250101 AND DateKey < 20250201", sourceConn);
                    sourceCount = (int)sourceCmd.ExecuteScalar();
                }
                catch { sourceCount = -1; }

                try
                {
                    var targetCmd = new SqlCommand($"SELECT COUNT(*) FROM [{kvp.Key}] WHERE DateKey >= 20250101 AND DateKey < 20250201", targetConn);
                    targetCount = (int)targetCmd.ExecuteScalar();
                }
                catch { targetCount = -1; }

                tableInfo.Add((kvp.Key, sourceCount, targetCount));
            }

            ViewBag.TableInfo = tableInfo;
            return View();
        }


        // 🔸 Copy a single table via query string
        public IActionResult CopyTable(string table)
        {
            if (!queries.ContainsKey(table))
                return Content($"❌ Table '{table}' not found.");

            try
            {
                Console.WriteLine($"⏳ Starting copy for table: {table}");

                using var sourceConn = new SqlConnection(SourceConnectionString);
                using var targetConn = new SqlConnection(TargetConnectionString);
                using var selectCmd = new SqlCommand(queries[table], sourceConn);
                using var bulkCopy = new SqlBulkCopy(targetConn)
                {
                    DestinationTableName = table,
                    BulkCopyTimeout = 0
                };

                sourceConn.Open();
                targetConn.Open();

                // 🔍 Inspect source columns
                using var reader = selectCmd.ExecuteReader();
                var sourceColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    sourceColumns.Add(name);
                    bulkCopy.ColumnMappings.Add(name, name);
                }


                bulkCopy.WriteToServer(reader);

                return Content($"✅ Table '{table}' copied successfully.");
            }
            catch (Exception ex)
            {
                return Content($"❌ Error copying table '{table}': {ex.Message}");
            }
        }
    }
}
