using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nice_Admin_1.Models;
using System.Data;
using System.Data.SqlClient;

namespace Nice_Admin_1.Controllers
{
    public class OrderController : Controller
    {
        private IConfiguration _configuration;
        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult DisplayOrder()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_Order_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult OrderDelete(int OrderID) {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection( connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_DeleteByPK";
            command.Parameters.Add("@OrderID",SqlDbType.Int).Value = OrderID;
            command.ExecuteNonQuery();
            return RedirectToAction("DisplayOrder");
        }
        public IActionResult FormOrder(int OrderID)
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
            List<OrderUserDropDownModel> userList = new List<OrderUserDropDownModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                OrderUserDropDownModel orderUserDropDownModel = new OrderUserDropDownModel();
                orderUserDropDownModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                orderUserDropDownModel.UserName = dataRow["UserName"].ToString();
                userList.Add(orderUserDropDownModel);
            }
            ViewBag.UserList = new SelectList(userList, "UserID", "UserName");
            command.CommandText = "PR_Customer_DropDown";
            SqlDataReader reader1 = command.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
            foreach (DataRow dataRow in dataTable1.Rows)
            {
                CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel();
                customerDropDownModel.CustomerID = Convert.ToInt32(dataRow["CustomerID"]);
                customerDropDownModel.CustomerName = dataRow["CustomerName"].ToString();
                customerList.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = new SelectList(customerList, "CustomerID", "CustomerName");
            //------------------------------
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Order_SelectByPK";
            command1.Parameters.AddWithValue("@OrderID", OrderID);
            SqlDataReader reader2 = command1.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            connection1.Close();
            OrderModel orderModel = new OrderModel();
            foreach (DataRow dataRow in dataTable2.Rows)
            {
                orderModel.OrderDate = DateTime.Parse(dataRow["OrderDate"].ToString());
                orderModel.CustomerID = Convert.ToInt32(dataRow["CustomerID"]);
                orderModel.PaymentMode = dataRow["PaymentMode"].ToString();
                orderModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                orderModel.ShippingAddress = dataRow["ShippingAddress"].ToString();
                orderModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View("FormOrder", orderModel);
        }
        public IActionResult OrderSave(OrderModel orderModel)
        {
            if (orderModel.UserID <= 0)
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
                if (orderModel.OrderID == null)
                {
                    command.CommandText = "PR_Order_Insert";
                }
                else
                {
                    command.CommandText = "PR_Order_UpdateByPK";
                    command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderModel.OrderID;
                }
                command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = orderModel.CustomerID;
                command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
                command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("DisplayOrder");
            }
            return View("FormOrder", orderModel);
        }
    }
}
