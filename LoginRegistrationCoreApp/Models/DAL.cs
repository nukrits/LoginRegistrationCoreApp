using System.Data;
using System.Data.SqlClient;
namespace LoginRegistrationCoreApp.Models
{
    public class DAL
    {
        public Response Registration(Users users, SqlConnection connection)
        {
            Response response = new Response();
            SqlCommand cmd = null;
            try
            {
                cmd = new SqlCommand("sp_register", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", users.Username);
                cmd.Parameters.AddWithValue("@Email", users.Email);
                cmd.Parameters.AddWithValue("@Password", users.Password);
                cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar, 200);
                cmd.Parameters["@ErrorMessage"].Direction = ParameterDirection.Output;
                connection.Open();
                int i = cmd.ExecuteNonQuery();
                connection.Close();
                string message = cmd.Parameters["@ErrorMessage"].Value?.ToString() ?? "Unknown error";
                if (i > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = message;
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = message;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "DAL Error: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                cmd?.Dispose();
            }
            return response;
        }
        public Response Login(Users users, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("sp_Login", connection);
                da.SelectCommand.Parameters.AddWithValue("@Email", users.Email);
                da.SelectCommand.Parameters.AddWithValue("@Password", users.Password);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "User is valid";
                }
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "User is invalid";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 100;
                response.StatusMessage = ex.Message;
            }
            return response;
        }
    }
}