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
           var produtos = _context.Produtos.ToList(); // Busca todos os produtos e transforma em lista
            return Ok(produtos);
        }


        [HttpGet]
        [Route("{id}")] 
        public ActionResult<ProdutoModel> BuscarProdutoPorId(int id) 
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null) // Não encontrado
            {
                return NotFound("Registro não localizado"); // Erro 404
            }

            return Ok(produto);
        }


        [HttpPost]
        public ActionResult<ProdutoModel> CriarProduto(ProdutoModel produtoModel)
        {
            if (produtoModel == null) // Verifica se o valor está vazio
            {
                return BadRequest("Produto inválido"); // Erro 400
            }

            _context.Produtos.Add(produtoModel); // Adiciona o produto no banco de dados
            _context.SaveChanges(); // IMPORTANTE* Salva as alterações no banco de dados 

            return CreatedAtAction(nameof(BuscarProdutoPorId), new { id = produtoModel.Id }, produtoModel); // Retorna o produto criado com o status 201 (Created)
        }
    }
}
