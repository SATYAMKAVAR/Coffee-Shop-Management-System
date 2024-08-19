using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nice_Admin_1.Models;
using System.Data;
using System.Data.SqlClient;

namespace Nice_Admin_1.Controllers
{
    public class ProductController : Controller
    {
        private IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult DisplayProduct()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult ProductDelete(int ProductID)
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_DeleteByPK";
            command.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
            command.ExecuteNonQuery();
            return RedirectToAction("DisplayProduct");
        }
        public IActionResult FormProduct(int ProductId)
        {
            //------------------------
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_DropDown";
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            connection.Close();
            List<ProductUserDropDownModel> userList = new List<ProductUserDropDownModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                ProductUserDropDownModel userDropDownModel = new ProductUserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                userDropDownModel.UserName = dataRow["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            //ProductModel productModel = new ProductModel();
            ViewBag.UserList = new SelectList(userList,"UserID","UserName");
            //-----------------------
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Product_SelectByPK";
            command1.Parameters.AddWithValue("@ProductID", ProductId);
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            connection1.Close();
            ProductModel ProductModel = new ProductModel();
            foreach (DataRow  dataRow in dataTable1.Rows)
            {
                ProductModel.ProductName = dataRow["ProductName"].ToString();
                ProductModel.ProductCode= dataRow["ProductCode"].ToString();
                ProductModel.ProductPrice= Convert.ToDouble(dataRow["ProductPrice"]);
                ProductModel.Description= dataRow["Description"].ToString();
                ProductModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View("FormProduct",ProductModel);
        }
        public IActionResult ProductSave(ProductModel productModel)
        {
            if (productModel.UserID <= 0)
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
                if (productModel.ProductID == null)
                {
                command.CommandText = "PR_Product_Insert";
                }
                else
                {
                    command.CommandText = "PR_Product_UpdateByPK";
                    command.Parameters.Add("@ProductID",SqlDbType.Int).Value = productModel.ProductID;
                }
                command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = productModel.ProductName;
                command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = productModel.ProductPrice;
                command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = productModel.ProductCode;
                command.Parameters.Add("@Description", SqlDbType.VarChar).Value = productModel.Description;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = productModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("DisplayProduct");
            }
            //if (!ModelState.IsValid)
            //{
            //    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            //    {
            //        Console.WriteLine(error.ErrorMessage);
            //    }
            //}
            return View("FormProduct", productModel);
        }
    }
}
