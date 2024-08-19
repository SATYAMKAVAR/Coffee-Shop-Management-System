using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nice_Admin_1.Models;
using System.Data;
using System.Data.SqlClient;

namespace Nice_Admin_1.Controllers
{
    public class BillsController : Controller
    {
        private IConfiguration _configuration;
        public BillsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult DisplayBills()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_Bill_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult BillDelete(int BillID)
        {
            Console.WriteLine(BillID);
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection( connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bill_DeleteByPK";
            command.Parameters.Add("@BillID",SqlDbType.Int).Value = BillID;
            command.ExecuteNonQuery();
            return RedirectToAction("DisplayBills");
        }
        public IActionResult FormBills(int BillID)
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
            List<BillsUserDropDownModel> userList = new List<BillsUserDropDownModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                BillsUserDropDownModel userDropDownModel = new BillsUserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                userDropDownModel.UserName = dataRow["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = new SelectList(userList, "UserID", "UserName");
            //----------------------------------------------------------------------------------------------------
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Bill_SelectByPK";
            command1.Parameters.AddWithValue("@BillID", BillID);
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            connection1.Close();
            BillsModel billsModel = new BillsModel();
            foreach (DataRow dataRow in dataTable1.Rows)
            {
                billsModel.BillNumber = dataRow["BillNumber"].ToString();
                //billsModel.BillDate = dataRow["BillsCode"].ToString();
                billsModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                billsModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                billsModel.Discount = Convert.ToDouble(dataRow["Discount"]);
                billsModel.NetAmount = Convert.ToDouble(dataRow["NetAmount"]);
                billsModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View("FormBills", billsModel);
        }
        public IActionResult BillsSave(BillsModel billsModel)
        {
            if (billsModel.UserID <= 0)
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
                if (billsModel.BillID == null)
                {
                    command.CommandText = "PR_Bill_Insert";
                }
                else
                {
                    command.CommandText = "PR_Bill_pdateByPK";
                    command.Parameters.Add("@BillID", SqlDbType.Int).Value = billsModel.BillID;
                }
                command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billsModel.BillNumber;
                command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = billsModel.BillDate;
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billsModel.OrderID;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billsModel.TotalAmount;
                command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billsModel.Discount;
                command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billsModel.NetAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = billsModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("DisplayBills");
            }
            return View("FormBills", billsModel);
        }
    }
}
