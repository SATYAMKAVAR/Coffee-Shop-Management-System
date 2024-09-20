using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nice_Admin_1.Models;
using OfficeOpenXml.Style;
using System.Data;
using System.Data.SqlClient;

namespace Nice_Admin_1.Controllers
{
    public class OrderDetailController : Controller
    {
        #region Configuration
        private IConfiguration _configuration;
        public OrderDetailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion
        
        #region Display Order Detail
        public IActionResult DisplayOrderDetail()
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
            command.CommandText = "PR_OrderDetail_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion
        
        #region Order Detail Delete
        public IActionResult OrderDetailDelete(int OrderDetailid)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_OrderDetail_DeleteByPK";
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailid;
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
            return RedirectToAction("DisplayOrderDetail");
        }
        #endregion
        
        #region Form Order Detail
        public IActionResult FormOrderDetail(int OrderDetailID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_Product_DropDown";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            List<ProductDropDownModel> productList = new List<ProductDropDownModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                ProductDropDownModel productDropDownModel = new ProductDropDownModel();
                productDropDownModel.ProductID = Convert.ToInt32(row["ProductID"]);
                productDropDownModel.ProductName = row["ProductName"].ToString();
                productList.Add(productDropDownModel);
            }
            ViewBag.ProductList = new SelectList(productList, "ProductID", "ProductName");
            //--------------------------------------------------------------------------------------------
            command.CommandText = "PR_User_DropDown";
            SqlDataReader reader2 = command.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<OrderDetailUserDropDownModel> userList = new List<OrderDetailUserDropDownModel>();
            foreach (DataRow dataRow in dataTable2.Rows)
            {
                OrderDetailUserDropDownModel UserDropDownModel = new OrderDetailUserDropDownModel();
                UserDropDownModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                UserDropDownModel.UserName = dataRow["UserName"].ToString();
                userList.Add(UserDropDownModel);
            }
            ViewBag.UserList = new SelectList(userList, "UserID", "UserName");
            //----------------------------------------------------------------------------------------------
            //command.CommandText = "PR_Cascading_Order_DropDown";
            //OrderDetailModel orderDetailModel = new OrderDetailModel();
            //command.Parameters.AddWithValue("@UserID", orderDetailModel.UserID);
            //SqlDataReader reader1 = command.ExecuteReader();
            //DataTable dataTable1 = new DataTable();
            //dataTable1.Load(reader1);
            //List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();
            //foreach (DataRow row in dataTable1.Rows)
            //{
            //    OrderDropDownModel orderDropDownModel = new OrderDropDownModel();
            //    orderDropDownModel.OrderID = Convert.ToInt32(row["OrderID"]);
            //    orderDropDownModel.OrderNumber = Convert.ToInt32(row["OrderNumber"]);
            //    orderList.Add(orderDropDownModel);
            //}
            //ViewBag.OrderList = new SelectList(orderList, "OrderID", "OrderNumber");
            //--------------------------------------------------------------------------------------------
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_OrderDetail_SelectByPK";
            command1.Parameters.AddWithValue("@OrderDetailID", OrderDetailID);
            SqlDataReader reader3 = command1.ExecuteReader();
            DataTable dataTable3 = new DataTable();
            dataTable3.Load(reader3);
            connection1.Close();
            OrderDetailModel OrderDetailModel = new OrderDetailModel();
            foreach (DataRow dataRow in dataTable3.Rows)
            {
                OrderDetailModel.OrderDetailID = Convert.ToInt32(dataRow["OrderDetailID"]);
                OrderDetailModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                OrderDetailModel.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                OrderDetailModel.Quantity = Convert.ToInt32(dataRow["Quantity"]);
                OrderDetailModel.Amount = Convert.ToDouble(dataRow["Amount"]);
                OrderDetailModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                OrderDetailModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View("FormOrderDetail", OrderDetailModel);
        }
        #endregion
        
        #region Order Detail Save
        public IActionResult OrderDetailSave(OrderDetailModel orderDetailModel)
        {
            if (orderDetailModel.UserID <= 0)
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
                if (orderDetailModel.OrderDetailID == null)
                {
                    command.CommandText = "PR_OrderDetail_Insert";
                }
                else
                {
                    command.CommandText = "PR_OrderDetail_UpdateByPK";
                    command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetailModel.OrderDetailID;
                }
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderDetailModel.OrderID;
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = orderDetailModel.ProductID;
                command.Parameters.Add("@Quantity", SqlDbType.Int).Value = orderDetailModel.Quantity;
                command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = orderDetailModel.Amount;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderDetailModel.TotalAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = orderDetailModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("DisplayOrderDetail");
            }
            return View("FormOrderDetail", orderDetailModel);
        }
        #endregion

        #region GtOrderByUser
        public IActionResult GetOrdersByUser(int userId)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Cascading_Order_DropDown";
            command.Parameters.AddWithValue("@UserID", userId);

            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                OrderDropDownModel order = new OrderDropDownModel
                {
                    OrderID = Convert.ToInt32(row["OrderID"]),
                    OrderNumber = Convert.ToInt32(row["OrderNumber"])
                };
                orderList.Add(order);
            }
            //ViewBag.OrderList = new SelectList(orderList, "OrderID", "OrderNumber");
            return Json(orderList); // Return the list as JSON
        }

        #endregion
    }
}
