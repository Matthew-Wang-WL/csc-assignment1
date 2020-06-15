using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Management;
using Task2.Models;

namespace Task2.Controllers
{
    public class ProductsController : ApiController
    {
        /*readonly IProductRepository _productRepository = new ProductRepository();

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }*/

        static readonly IProductRepository _productRepository = new ProductRepository();

        [HttpGet]
        [Route("api/products")]
        public IEnumerable<Product> GetAllProductsFromRepository()
        {
            return _productRepository.GetAll();
        }

        [HttpGet]
        [Route("api/products/{id:int:min(1)}", Name = "getProductById")]
        public Product retrieveProductfromRepository(int id)
        {
            Product item = _productRepository.Get(id);
            if (item == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "This item does not exist"));
            }
            return item;
        }


        [HttpGet]
        [Route("api/products", Name = "getProductByCategory")]
        public IEnumerable<Product> GetProductsByCategory(string category)
        {
            return _productRepository.GetAll().Where(
                p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        [HttpPost]
        [Route("api/products")]
        public HttpResponseMessage PostProduct(Product item)
        {
            if (ModelState.IsValid)
            {
                item = _productRepository.Add(item);
                var response = Request.CreateResponse<Product>(HttpStatusCode.Created, item);

                string uri = Url.Link("getProductById", new { id = item.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [HttpPut]
        [Route("api/products/{id:int}")]
        public HttpResponseMessage PutProduct(int id, Product product)
        {
            product.Id = id;
            if (!_productRepository.Update(product))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Selected item does not exist"));
            } else
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Item " + id + " was updated successfully.");
            }
        }

        [HttpDelete]
        [Route("api/products/{id:int}")]
        public HttpResponseMessage DeleteProduct(int id)
        {
            _productRepository.Remove(id);
            return Request.CreateResponse(HttpStatusCode.OK, "Deleted item " + id + " successfully.");
        }
    }
}
