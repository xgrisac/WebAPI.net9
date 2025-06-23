using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.net9.Data;
using WebAPI.net9.Models;

namespace WebAPI.net9.Controllers
{
    /// <summary>
    /// Controller para gerenciar produtos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto do banco de dados.
        /// </summary>
        /// <param name="context">Instância do AppDbContext.</param>
        public ProdutoController(AppDbContext context)
        {
            _context = context; 
        }

        /// <summary>
        /// Retorna todos os produtos cadastrados.
        /// </summary>
        /// <returns>Lista de produtos ou erro 500.</returns>
        [HttpGet("Estoque")]
        public ActionResult<List<ProdutoModel>> BuscarProdutos()
        {
            try
            {
                var produtos = _context.Produtos.ToList(); // Busca todos os produtos e transforma em lista.
                return Ok(produtos);
            }
           
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}"); // ex.Message retorna o erro captarado pela variável EX
            }
        }

        /// <summary>
        /// Busca um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Erro 404, produto encontrado ou erro 500.</returns>       
        [HttpGet("{id}")]
        public ActionResult<ProdutoModel> BuscarProdutoPorId(int id) 
        {
            try
            {
                var produto = _context.Produtos.Find(id); // Find busca dentro do banco de dados
                if (produto == null) 
                {
                    return NotFound("Registro não localizado"); // Erro 404
                }

                return Ok(produto);
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="produtoModel">Objeto do produto a ser criado.</param>
        /// <returns>Erro 400, produto encontrado ou erro 500.</returns>
        [HttpPost]
        public ActionResult<ProdutoModel> CriarProduto([FromBody] ProdutoModel produtoModel)
        {
            try
            {
                if (produtoModel == null) // Verifica se o valor está vazio
                {
                    return BadRequest("Produto inválido"); // Erro 400
                }

                if (produtoModel.Id != 0)
                { 
                    return BadRequest("O campo 'Id' não deve ser informado. Ele é gerado automaticamente, favor excluir o campo ID do seu JSON.");
                }

                _context.Produtos.Add(produtoModel); // Adiciona o produto no banco de dados
                _context.SaveChanges(); // Salva as alterações no banco de dados 

                return CreatedAtAction(nameof(BuscarProdutoPorId), new { id = produtoModel.Id }, produtoModel); // Retorna o produto criado com o status 201 (Created)
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}"); 
            }
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="produtoModel">Dados atualizados do produto.</param>
        /// <param name="id">ID do produto a ser atualizado.</param>
        /// <returns>Erro 404, produto encontrado ou erro 500.</returns>
        [HttpPut("{id}")]
        public ActionResult EditarProduto([FromBody] ProdutoModel produtoModel, int id)
        {
            try
            {
                var produto = _context.Produtos.Find(id); // find busca o elemento dentro da tabela produtos do DB

                if (produto == null)
                {
                    return NotFound("Registro não localizado");
                }
                // Atualizo o produto antigo com o novo conteúdo
                produto.Nome = produtoModel.Nome;
                produto.Descricao = produtoModel.Descricao;
                produto.Marca = produtoModel.Marca;
                produto.QuantidadeEstoque = produtoModel.QuantidadeEstoque;
                produto.CodigoDeBarras = produtoModel.CodigoDeBarras;

                _context.Produtos.Update(produto);
                _context.SaveChanges();

                return Ok(produto);
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}"); 
            }          
        }

        /// <summary>
        /// Deleta um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto a ser removido.</param>
        /// <returns>Erro 404, mensagem de sucesso ou erro 500.</returns>
        [HttpDelete("{id}")]
        public ActionResult DeletarProduto(int id)
        {
            try
            {
                var produto = _context.Produtos.Find(id);

                if (produto == null)
                {
                    return NotFound("Registro não localizado");
                }

                _context.Produtos.Remove(produto);
                _context.SaveChanges();

                return Ok($"Conteúdo do ID {id} deletado com sucesso!");
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}");                
            }            
        }

        /// <summary>
        /// Busca produtos pelo nome ou marca.
        /// </summary>
        /// <param name="nome">Nome do produto (opcional).</param>
        /// <param name="marca">Marca do produto (opcional).</param>
        /// <returns>Lista de produtos encontrados ou erro 500.</returns>
        [HttpGet("Buscar")] // Busca os produtos por nome
        public ActionResult<List<ProdutoModel>> BuscarPorNomeOuMarca(string? nome, string? marca)
        {
            try
            {
                var produtos = _context.Produtos
                .Where(p => (nome == null || p.Nome.Contains(nome)) &&
                            (marca == null || p.Marca.Contains(marca)))
                .ToList();

                return Ok(produtos);
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
    }
}
