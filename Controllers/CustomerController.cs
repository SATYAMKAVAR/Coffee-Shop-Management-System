using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nice_Admin_1.Models;
using System.Data;
using System.Data.SqlClient;

namespace Nice_Admin_1.Controllers
{
    public class CustomerController : Controller
    {
        private IConfiguration _configuration;
        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult DisplayCustomer()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_Customer_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult CustomerDelete(int CustomerID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection( connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Customer_DeleteByPK";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
            command.ExecuteNonQuery();
            return RedirectToAction("DisplayCustomer");
        }
        public IActionResult FormCustomer(int CustomerID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_User_DropDown";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<CustomerUserDropDownModel> userList = new List<CustomerUserDropDownModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                CustomerUserDropDownModel userDropDownModel = new CustomerUserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                userDropDownModel.UserName = dataRow["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = new SelectList(userList, "UserID", "UserName");
            //----------------------------------------------------------------------------------------------------------------------------
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Customer_SelectByPK";
            command1.Parameters.AddWithValue("@CustomerID", CustomerID);
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            connection1.Close();
            CustomerModel customerModel = new CustomerModel();
            foreach (DataRow dataRow in dataTable1.Rows)
            {
                customerModel.CustomerName = dataRow["CustomerName"].ToString();
                customerModel.HomeAddress = dataRow["HomeAddress"].ToString();
                customerModel.Email = dataRow["Email"].ToString();
                customerModel.MobileNo= dataRow["MobileNo"].ToString();
                customerModel.GSTNO= dataRow["GSTNO"].ToString();
                customerModel.CityName= dataRow["CityName"].ToString();
                customerModel.PinCode= dataRow["PinCode"].ToString();
                customerModel.NetAmount = Convert.ToDouble(dataRow["NetAmount"]);
                customerModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View("FormCustomer", customerModel);
        }
        public IActionResult CustomerSave(CustomerModel customerModel)
        {
            if (customerModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if (ModelState.IsValid) 
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (customerModel.CustomerID == null)
                {
                    command.CommandText = "PR_Customer_Insert";
                }
                else
                {
                    command.CommandText = "PR_Customer_UpdateByPK";
                    command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerModel.CustomerID;
                }
                command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = customerModel.CustomerName;
                command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = customerModel.HomeAddress;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = customerModel.Email;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = customerModel.MobileNo;
                command.Parameters.Add("@GSTNO", SqlDbType.VarChar).Value = customerModel.GSTNO;
                command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = customerModel.CityName;
                command.Parameters.Add("@PinCode", SqlDbType.VarChar).Value = customerModel.PinCode;
                command.Parameters.Add("@NetAmount", SqlDbType.VarChar).Value = customerModel.NetAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = customerModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("DisplayCustomer");
            }
            return View("FormCustomer", customerModel);
        }
    }
}
