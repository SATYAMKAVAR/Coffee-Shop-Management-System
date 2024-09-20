using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nice_Admin_1.Models;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;
//using System.Web.Mvc;

namespace Nice_Admin_1.Controllers
{
    public class ProductController : Controller
    {
        #region Configuration
        private IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion
        
        #region Display Product
        public IActionResult DisplayProduct()
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
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion
        
        #region Product Delete
        public IActionResult ProductDelete(int ProductID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Product_DeleteByPK";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
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
            return RedirectToAction("DisplayProduct");
        }
        #endregion
        
        #region Form Product
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
            ViewBag.UserList = new SelectList(userList, "UserID", "UserName");
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
            foreach (DataRow dataRow in dataTable1.Rows)
            {
                ProductModel.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                ProductModel.ProductName = dataRow["ProductName"].ToString();
                ProductModel.ProductCode = dataRow["ProductCode"].ToString();
                ProductModel.ProductPrice = Convert.ToDouble(dataRow["ProductPrice"]);
                ProductModel.Description = dataRow["Description"].ToString();
                ProductModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View("FormProduct", ProductModel);
        }
        #endregion
        
        #region Product Save
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
                    command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productModel.ProductID;
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
        #endregion
     
        #region Export To Excel
        public IActionResult ExportToExcel()
        {
            // Sample data for the table
            //var data = new List<YourModel>
            //{
            //    new YourModel { Id = 1, Name = "Item 1", Description = "Description 1" },
            //    new YourModel { Id = 2, Name = "Item 2", Description = "Description 2" },
            //    // Add more items as needed
            //};
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Add the headers
                worksheet.Cells[1, 1].Value = "ProductID";
                worksheet.Cells[1, 2].Value = "ProductName";
                worksheet.Cells[1, 3].Value = "ProductPrice";
                worksheet.Cells[1, 4].Value = "ProductCode";
                worksheet.Cells[1, 5].Value = "Description";
                worksheet.Cells[1, 6].Value = "UserId";
                worksheet.Cells[1, 7].Value = "UserName";
                worksheet.Cells[1, 8].Value = "Email";

                // Add the data
                int rowNumber = 0;
                foreach (DataRow row in table.Rows)
                {
                    worksheet.Cells[rowNumber + 2, 1].Value = row["ProductID"];
                    worksheet.Cells[rowNumber + 2, 2].Value = row["ProductName"];
                    worksheet.Cells[rowNumber + 2, 3].Value = row["ProductPrice"];
                    worksheet.Cells[rowNumber + 2, 4].Value = row["ProductCode"];
                    worksheet.Cells[rowNumber + 2, 5].Value = row["Description"];
                    worksheet.Cells[rowNumber + 2, 6].Value = row["UserId"];
                    worksheet.Cells[rowNumber + 2, 7].Value = row["UserName"];
                    worksheet.Cells[rowNumber + 2, 8].Value = row["Email"];
                    rowNumber++;
                }
                //for (int i = 0; i < data.Count; i++)
                //{
                //    worksheet.Cells[i + 2, 1].Value = data[i].Id;
                //    worksheet.Cells[i + 2, 2].Value = data[i].Name;
                //    worksheet.Cells[i + 2, 3].Value = data[i].Description;
                //}

                // Prepare the response
                var fileBytes = package.GetAsByteArray();
                var fileName = "ProductData.xlsx";

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        #endregion
    }
}
