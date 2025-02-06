using Microsoft.Data.SqlClient;
using System.Data;

public class DbUtil
{
    public static bool CheckDuplicate(string tableName, string whereClause, SqlConnection cn)
    {
        string query = "select * from " + tableName + " where " + whereClause;
        try
        {
            SqlDataAdapter da = new SqlDataAdapter(query, cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0) { return true; }
            return false;
        }
        catch (Exception) { return true; }
    }

    public static bool UpdateDB(string query, out string error, SqlConnection cn)
    {
        error = "";
        if (cn != null)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                cmd.ExecuteNonQuery();
                error = "";
                return true;
            }
            catch (Exception ex) { error = ex.Message + "**Error On**" + query; }
            finally { cn.Close(); }
        }
        return false;
    }

    private static string MakeQuery(string tableName, string valueColumn, string keyColumn, string whereClause)
    {
        string query = "select " + keyColumn + ", " + valueColumn + " from " + tableName;
        if (whereClause.Trim() != "") { query += " where " + whereClause; }
        return query;
    }

    public static DataRow GetDBRow(string tableName, string whereClause, SqlConnection cn)
    {
        return GetDBRow("select * from " + tableName + " where " + whereClause, cn);
    }

    public static DataRow GetDBRow(string query, SqlConnection cn)
    {
        SqlDataAdapter da = new SqlDataAdapter(query, cn);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0) { return dt.Rows[0]; }
        return null;
    }

    public static DataTable GetAllDBRows(string tableName, string whereClause, SqlConnection cn)
    {
        return GetAllDBRows("select * from " + tableName + " where " + whereClause, cn);
    }

    public static DataTable GetAllDBRows(string query, SqlConnection cn)
    {
        SqlDataAdapter da = new SqlDataAdapter(query, cn);
        DataTable dt = new DataTable();
        da.Fill(dt);
        return dt;
    }

    public static int GetMaxSeqId(string tableName, string columnName, SqlConnection cn)
    {
        string query = "select max(" + columnName + ") from " + tableName;
        DataRow dr = GetDBRow(query, cn);
        if (dr != null && dr[0].ToString() != "") { return Convert.ToInt32(dr[0].ToString()); }
        return 0;
    }

    public static int GetSum(string tableName, string columnName, string whereClause, SqlConnection cn)
    {
        string query = "select sum(" + columnName + ") from " + tableName + " where " + whereClause;
        DataRow dr = GetDBRow(query, cn);
        if (dr != null && dr[0].ToString() != "") { return Convert.ToInt32(dr[0].ToString()); }
        return 0;
    }

    public static string GetCountryName(string countryCode, SqlConnection cn)
    {
        DataRow dr = GetDBRow("Country", "CountryCode='" + countryCode + "'", cn);
        if (dr != null) { return dr["countryName"].ToString(); }
        return "";
    }

    public static bool AddBundleItemIdsInGenericAttribute(int productId = 0, int customerId = 0, string selectedBundleIds = "", SqlConnection cn = null)
    {
        string error = "";
        if (string.IsNullOrWhiteSpace(selectedBundleIds) || productId == 0 || customerId == 0) { return false; }
        var gaQuery = "select * from GenericAttribute where [Key] = 'BundleIds_" + productId + "' And EntityId=" + customerId;
        DataRow dr = GetDBRow(gaQuery, cn);
        if (dr == null)
        {
            return UpdateDB("Insert into GenericAttribute (EntityId,KeyGroup,[Key],Value,StoreId) values(" + customerId + ",'Customer','BundleIds_" + productId + "','" + selectedBundleIds + "','0')", out error, cn);
        }
        return false;
    }

    public static string GetBundleValueByBundleKey(string key, int customerId, SqlConnection cn)
    {
        DataRow dr = GetDBRow("Select * from GenericAttribute Where [Key]='" + key + "' And EntityId=" + customerId, cn);
        if (dr != null) { return dr["Value"].ToString(); }
        return "";
    }

    public static bool DeleteBundleValueByBundleKey(string key, int customerId, SqlConnection cn)
    {
        string error = "";
        return UpdateDB("Delete from GenericAttribute where [Key] = '" + key + "' and EntityId=" + customerId + "", out error, cn);
    }
}