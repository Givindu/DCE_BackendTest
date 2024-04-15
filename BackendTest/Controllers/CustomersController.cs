using BackendTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BackendTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CustomersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("GetAllCustomers")]
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Customer", con);
                    {
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                List<Customer> customers = new List<Customer>();
                foreach (DataRow dr in dt.Rows)
                {
                    customers.Add(new Customer
                    {
                        UserId = Guid.Parse(dr["UserId"].ToString()),
                        UserName = dr["UserName"].ToString(),
                        Email = dr["Email"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        CreatedOn = Convert.ToDateTime(dr["CreatedOn"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"])
                    });

                }
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred.");
            }
        }


        [Route("GetCustomerById/{userId}")]
        [HttpGet]
        public async Task<IActionResult> GetCustomerById(Guid userId)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Customer WHERE UserId = @UserId", con);
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        con.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    return NotFound("Customer not found");
                }

                DataRow dr = dt.Rows[0];

                var customer = new
                {
                    UserId = Guid.Parse(dr["UserId"].ToString()),
                    UserName = dr["UserName"].ToString(),
                    Email = dr["Email"].ToString(),
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString(),
                    CreatedOn = Convert.ToDateTime(dr["CreatedOn"]),
                    IsActive = Convert.ToBoolean(dr["IsActive"])
                };

                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred.");
            }
        }


        [Route("CreateCustomer")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer obj)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                SqlCommand cmd = new SqlCommand("INSERT INTO Customer Values ('" + Guid.NewGuid() + "', '" + obj.UserName + "', '" + obj.Email + "', '" + obj.FirstName + "', '" + obj.LastName + "', '" + DateTime.Now + "', '" + obj.IsActive + "')", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return Ok("Customer Created successfully");
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, "An error occurred.");
            }
        }


        [Route("UpdateCustomer/{userId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateCustomer(Guid userId, [FromBody] JsonPatchDocument<UpdateCustomer> patchDocument)
        {
            try
            {
                // Retrieve connection string from appsettings
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                // Retrieve customer data from the database
                Customer existingCustomer;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand selectCommand = new SqlCommand("SELECT * FROM Customer WHERE UserId = @UserId", connection);
                    selectCommand.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = await selectCommand.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            return NotFound("Customer not found");
                        }

                        reader.Read();
                        existingCustomer = new Customer
                        {
                            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        };
                    }
                }

                // Apply the patch document to the existing customer
                var customerToUpdate = new UpdateCustomer
                {
                    UserName = existingCustomer.UserName,
                    Email = existingCustomer.Email,
                    FirstName = existingCustomer.FirstName,
                    LastName = existingCustomer.LastName,
                    IsActive = existingCustomer.IsActive
                };
                patchDocument.ApplyTo(customerToUpdate, ModelState);

                // Validate the patched model
                if (!TryValidateModel(customerToUpdate))
                {
                    return BadRequest(ModelState);
                }

                // Update customer fields in the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand updateCommand = new SqlCommand("UPDATE Customer SET UserName = @UserName, Email = @Email, FirstName = @FirstName, LastName = @LastName, IsActive = @IsActive WHERE UserId = @UserId", connection);
                    updateCommand.Parameters.AddWithValue("@UserId", userId);
                    updateCommand.Parameters.AddWithValue("@UserName", customerToUpdate.UserName);
                    updateCommand.Parameters.AddWithValue("@Email", customerToUpdate.Email);
                    updateCommand.Parameters.AddWithValue("@FirstName", customerToUpdate.FirstName);
                    updateCommand.Parameters.AddWithValue("@LastName", customerToUpdate.LastName);
                    updateCommand.Parameters.AddWithValue("@IsActive", customerToUpdate.IsActive);

                    int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        return NotFound("Customer not found");
                    }
                }

                return Ok("Customer updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred.");
            }
        }


        [Route("DeleteCustomer/{userId}")]
        [HttpDelete]
        public IActionResult DeleteCustomer(Guid userId)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Customer WHERE UserId = @UserId", con);
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        con.Close();

                        if (rowsAffected > 0)
                        {
                            return Ok("Customer with deleted successfully");
                        }
                        else
                        {
                            return NotFound("No customer found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred.");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }



        [Route("ActiveOrdersByCustomer/{userId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetActiveOrdersByCustomer(Guid userId)
        {
            List<Order> activeOrders = new List<Order>();

            try
            {
                SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                {
                    SqlCommand command = new SqlCommand(@"EXEC GetActiveOrdersByCustomer @UserId", connection);
                    command.Parameters.AddWithValue("@UserId", userId);

                    await connection.OpenAsync();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            Order order = new Order
                            {
                                OrderId = reader.GetGuid(0),
                                ProductId = reader.GetGuid(1),
                                OrderStatus = reader.GetInt32(2),
                                OrderType = reader.GetInt32(3),
                                OrderedOn = reader.GetDateTime(4),
                                ShippedOn = reader.GetDateTime(5),
                                ProductName = reader.GetString(6),
                                UnitPrice = reader.GetDecimal(7)
                            };

                            activeOrders.Add(order);
                        }
                    }
                }

                return activeOrders;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
