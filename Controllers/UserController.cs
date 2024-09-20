using Microsoft.AspNetCore.Mvc;
using Nice_Admin_1.Models;
using System.Data;
using System.Data.SqlClient;

namespace Nice_Admin_1.Controllers
{
    public class UserController : Controller
    {
        #region Configuration
        private IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion
        
        #region Display User
        public IActionResult DisplayUser()
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }
            else
            {
                ViewBag.ErrorMessage = null; // Or you can omit this step
            }
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
        #endregion
        
        #region User Delete
        public IActionResult UserDelete(int UserID)
        {

            try{
                string connectionString = this._configuration.GetConnectionString("connectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_DeleteByPK";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                // Check if it's a foreign key constraint violation error
                if (ex.Number == 547) // 547 is the SQL Server error code for FK violation
                {
                    TempData["ErrorMessage"] = "Unable to delete product as it is referenced in another table.";
                }
                else
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("DisplayUser");
        }
        #endregion
        
        #region Form User
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
                userModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                userModel.UserName = dataRow["UserName"].ToString();
                userModel.Email = dataRow["Email"].ToString();
                userModel.Password = dataRow["Password"].ToString();
                userModel.MobileNo = dataRow["MobileNo"].ToString();
                userModel.Address = dataRow["Address"].ToString();
                userModel.IsActive = Convert.ToBoolean(dataRow["IsActive"]);
            }
            return View("FormUser", userModel);
        }
        #endregion
        
        #region User Save
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
        #endregion

        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this._configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                            TempData["UserName"] = HttpContext.Session.GetString("UserName");
                        }

                        return RedirectToAction("index", "Home");
                    }
                    if (dataTable.Rows.Count == 0)
                    {
                        TempData["ErrorMessage"] = "Invalid username or password.";
                        return RedirectToAction("Login", "User");
                    }
                    else
                    {
                        return RedirectToAction("Login", "User");
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }

        public IActionResult UserRegister(UserRegisterModel userRegisterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this._configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Register";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userRegisterModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userRegisterModel.Password;
                    sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = userRegisterModel.Email;
                    sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userRegisterModel.MobileNo;
                    sqlCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = userRegisterModel.Address;
                    sqlCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = 1;
                    sqlCommand.ExecuteNonQuery();
                    return RedirectToAction("Login", "User");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Register");
            }
            return RedirectToAction("Register");
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult UserProfile()
        {
            return View();
        }
    }
}
