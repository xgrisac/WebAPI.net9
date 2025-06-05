using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.net9.Data;
using WebAPI.net9.Models;

namespace WebAPI.net9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdutoController(AppDbContext context)
        {
            _context = context; // Me da acesso ao contexto do banco de dados (DbContext)
        }

        [HttpGet]
        public ActionResult<List<ProdutoModel>> BuscarProdutos() // Pega todos os produtos cadastrados no DB e retorna em lista na API
        {
           
        }
    }
}
