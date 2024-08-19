using Microsoft.AspNetCore.Mvc;
using Nice_Admin_1.Models;
using System.Data;
using System.Data.SqlClient;

namespace Nice_Admin_1.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult DisplayUser()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_User_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult UserDelete(int UserID)
        {
            string connectionString = this._configuration.GetConnectionString("connectionString");
            SqlConnection connection = new SqlConnection( connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_DeleteByPK";
            command.Parameters.Add("@UserID",SqlDbType.Int).Value = UserID;
            command.ExecuteNonQuery();
            return RedirectToAction("DisplayUser");
        }
        public IActionResult FormUser(int UserID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_User_SelectByPK";
            command1.Parameters.AddWithValue("@UserID", UserID);
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            connection1.Close();
            UserModel userModel = new UserModel();
            foreach (DataRow dataRow in dataTable1.Rows)
            {
                userModel.UserName = dataRow["UserName"].ToString();
                userModel.Email = dataRow["Email"].ToString();
                userModel.Password = dataRow["Password"].ToString();
                userModel.MobileNo = dataRow["MobileNo"].ToString();
                userModel.Address = dataRow["Address"].ToString();
                userModel.IsActive = Convert.ToBoolean(dataRow["IsActive"]);
            }
            return View("FormUser", userModel);
        }
        public IActionResult UserSave(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (userModel.UserID == null)
                {
                    command.CommandText = "PR_User_Insert";
                }
                else
                {
                    command.CommandText = "PR_User_UpdateByPK";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = userModel.UserID;
                }
                command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userModel.UserName;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = userModel.Email;
                command.Parameters.Add("@Password", SqlDbType.VarChar).Value = userModel.Password;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userModel.MobileNo;
                command.Parameters.Add("@Address", SqlDbType.VarChar).Value = userModel.Address;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = userModel.IsActive;
                command.ExecuteNonQuery();
                return RedirectToAction("DisplayUser");
            }
            return View("FormUser", userModel);
        }
    }
}
