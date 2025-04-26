// Database Access Layer
using FreschOne.Models;
using Microsoft.Data.SqlClient;


// Database Access Layer
public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // New ExecuteQuery Method
    public List<T> ExecuteQuery<T>(string query, object parameters = null)
    {
        var result = new List<T>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            if (parameters != null)
            {
                foreach (var param in parameters.GetType().GetProperties())
                {
                    cmd.Parameters.AddWithValue("@" + param.Name, param.GetValue(parameters));
                }
            }

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // Create an instance of T and populate it with the data from the reader
                var obj = Activator.CreateInstance<T>();
                foreach (var property in obj.GetType().GetProperties())
                {
                    if (reader[property.Name] != DBNull.Value)
                    {
                        property.SetValue(obj, reader[property.Name]);
                    }
                }
                result.Add(obj);
            }
        }
        return result;
    }


public FoUser AuthenticateUser(string username, string password)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM foUsers WHERE UserName = @username AND Password = @password AND Active = 1";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new FoUser { ID = (long)reader["ID"], UserName = (string)reader["UserName"], Admin = (bool)reader["Admin"], Active = (bool)reader["Active"] };
            }
        }
        return null;
    }

    public long? GetUserID(string username)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "SELECT ID FROM foUsers WHERE userName = @username";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != DBNull.Value ? (long?)result : null;
        }
    }

    public void LogUserLogin(long userId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO foUserLogin (UserID, LoginDateTime) VALUES (@userId, @dateTime)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@dateTime", DateTime.Now);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public bool CheckPasswordReset(long userId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "SELECT ResetWithNextLogin FROM foUserPasswordReset WHERE UserID = @userId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null && (bool)result;
        }
    }

    public void UpdateUserPassword(long userId, string newPassword)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "UPDATE foUsers SET Password = @newPassword WHERE ID = @userId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@newPassword", newPassword);
            conn.Open();
            cmd.ExecuteNonQuery();

            query = "UPDATE foUserPasswordReset set ResetWithNextLogin = 0 WHERE UserID = @userId";
            cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.ExecuteNonQuery();
        }
    }
    public bool IsUserAdmin(long userId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "SELECT Admin FROM foUsers WHERE ID = @userId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null && (bool)result;
        }
    }
    public bool CheckUserAccess(int userid, string tablename)
    {
        bool hasAccess = false;

        string query = @"
        SELECT COUNT(*)
        FROM foUserTable 
        WHERE UserID = @UserID 
          AND TableName = @TableName 
          AND Active = 1"; // Ensure the access is active

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@TableName", tablename);

                var result = cmd.ExecuteScalar();
                hasAccess = Convert.ToInt32(result) > 0; // If count is greater than 0, the user has access
            }
        }

        return hasAccess;
    }

    public string CheckUserTableAccessRights(long userid, string tablename)
    {
        string readwriteaccess = "";

        string query = @"
        SELECT ReadWriteAccess
        FROM foUserTable 
        WHERE UserID = @UserID 
        AND TableName = @TableName 
        AND Active = 1"; // Ensure the access is active

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userid);
                cmd.Parameters.AddWithValue("@TableName", tablename);

                var result = cmd.ExecuteScalar();

                if (result != null)
                {
                    readwriteaccess = result.ToString(); // Safe conversion if result is not null
                }
            }
        }

        return readwriteaccess;
    }

    public void EnsureAuditFieldsExist(string tableName)
    {
        var requiredColumns = new Dictionary<string, string>
    {
        { "Active", "BIT DEFAULT(1)" },
        { "CreatedUserID", "BIGINT" },
        { "CreatedDate", "DATETIME" },
        { "ModifiedUserID", "BIGINT" },
        { "ModifiedDate", "DATETIME" },
        { "DeletedUserID", "BIGINT" },
        { "DeletedDate", "DATETIME" }
    };

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            // Get existing columns in the specified table
            string checkColumnsQuery = @"
            SELECT COLUMN_NAME 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = @tableName";

            HashSet<string> existingColumns = new HashSet<string>();

            using (SqlCommand cmd = new SqlCommand(checkColumnsQuery, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingColumns.Add(reader["COLUMN_NAME"].ToString());
                    }
                }
            }

            // Prepare ALTER TABLE script
            List<string> alterStatements = new List<string>();
            foreach (var column in requiredColumns)
            {
                if (!existingColumns.Contains(column.Key))
                {
                    alterStatements.Add($"{column.Key} {column.Value}");
                }
            }

            // Apply ALTER TABLE script if needed
            if (alterStatements.Count > 0)
            {
                string alterQuery = $"ALTER TABLE {tableName} ADD {string.Join(", ", alterStatements)};";
                using (SqlCommand alterCmd = new SqlCommand(alterQuery, conn))
                {
                    alterCmd.ExecuteNonQuery();
                }
            }
        }
    }


}