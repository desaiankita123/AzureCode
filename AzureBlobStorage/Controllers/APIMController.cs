using AzureBlobStorage.Model;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorage.Controllers
{
    [ApiController]
    
    public class APIMController : Controller
    {
      
        [Route("api/GetProducts")]
        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                return Ok(new string[]
            {
                "Product 1",
                "Product 2",
                "Product 3"
            });
            }
            catch (Exception)
            {

                return StatusCode(500, new
                {
                    Message = "Failed to retrieve products!",
                    Error = "An error occurred while processing your request."
                });
            }
            
        }
        // Simple in-memory list (no database needed)
        private static List<Product> products = new List<Product>
    {
        new Product { Id = 1, Name = "Product1", Price = 100 },
        new Product { Id = 2, Name = "Product2", Price = 200 }
    };
        // GET
        [HttpGet("GetProductsList")]
        public IActionResult GetProductsList()
        {
            return Ok(products);
        }
        // POST
        [HttpPost("AddProduct")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            product.Id = products.Count + 1;
            products.Add(product);
            return Ok("Product added successfully!");
        }

        // PUT
        [HttpPut("UpdateProduct/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            var existing = products.FirstOrDefault(p => p.Id == id);
            if (existing == null)
                return NotFound("Product not found");

            existing.Name = product.Name;
            existing.Price = product.Price;
            return Ok("Product updated successfully!");
        }

        // DELETE
        [HttpDelete("DeleteProduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var existing = products.FirstOrDefault(p => p.Id == id);
            if (existing == null)
                return NotFound("Product not found");

            products.Remove(existing);
            return Ok("Product deleted successfully!");
        }
    }
}
