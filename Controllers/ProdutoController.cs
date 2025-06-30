using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProdutoController> _logger; // Logger para registrar informações, avisos e erros

        /// <summary>
        /// Construtor que injeta o contexto do banco de dados e o logger.
        /// </summary>
        /// <param name="context">Instância do AppDbContext.</param>
        /// <param name="logger">Instância do ILogger</param>
        public ProdutoController(AppDbContext context, ILogger<ProdutoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retorna todos os produtos cadastrados.
        /// </summary>
        /// <returns>Lista de produtos ou erro 500.</returns>
        [HttpGet("Estoque")]
        public async Task<ActionResult<List<ProdutoModel>>> BuscarProdutos()
        {
            _logger.LogDebug("Iniciando requisição: BuscarProdutos");

            try
            {
                var produtos = await _context.Produtos.ToListAsync(); // Pega todos os produtos do banco de dados de forma assíncrona e transforma em lista
                _logger.LogInformation("Lista de produtos retornada com sucesso. Total {Total}", produtos.Count); // Log informativo
                return Ok(produtos);
            }
           
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto.");
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}"); // ex.Message retorna o erro captarado pela variável EX
            }
        }

        /// <summary>
        /// Busca um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Erro 404, produto encontrado ou erro 500.</returns>       
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoModel>> BuscarProdutoPorId(int id) 
        {
            _logger.LogDebug("Iniciando requisição: BuscarProdutosPorId");

            try
            {
                var produto = await _context.Produtos.FindAsync(id); // Find busca dentro do banco de dados
                if (produto == null) 
                {
                    _logger.LogWarning("Produto com ID {Id} não encontrado.", id);
                    return NotFound("Registro não localizado"); // Erro 404
                }
                _logger.LogInformation("Produto com ID {Id} encontrado com sucesso.", id);
                return Ok(produto);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto com ID {Id}", id);
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="produtoModel">Objeto do produto a ser criado.</param>
        /// <returns>Erro 400, produto encontrado ou erro 500.</returns>
        [HttpPost]
        public async Task<ActionResult<ProdutoModel>> CriarProduto([FromBody] ProdutoModel produtoModel)
        {
            _logger.LogDebug("Iniciando requisição: CriarProduto");

            try
            {
                if (produtoModel == null) // Verifica se o valor está vazio
                {
                    _logger.LogWarning("Tentativa de criar produto com valor nulo");
                    return BadRequest("Produto inválido"); // Erro 400
                }

                if (produtoModel.Id != 0)
                {   
                    _logger.LogWarning("Tentativa de criar produto com ID informado manualmente");
                    return BadRequest("O campo 'Id' não deve ser informado. Ele é gerado automaticamente, favor excluir o campo ID do seu JSON.");
                }

                _context.Produtos.Add(produtoModel); // Adiciona o produto no banco de dados
                await _context.SaveChangesAsync(); // Salva as alterações no banco de dados 

                _logger.LogInformation("Produto criado com sucesso. ID: {Id}", produtoModel.Id);
                return CreatedAtAction(nameof(BuscarProdutoPorId), new { id = produtoModel.Id }, produtoModel); // Retorna o produto criado com o status 201 (Created)
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar produto");
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
        public async Task<ActionResult> EditarProduto([FromBody] ProdutoModel produtoModel, int id)
        {
            _logger.LogDebug("Iniciando requisição: EditarProduto");

            try
            {
                var produto = await _context.Produtos.FindAsync(id); // find busca o elemento dentro da tabela produtos do DB

                if (produto == null)
                {
                    _logger.LogWarning("Produto com ID {Id} não localizado", id);
                    return NotFound("Registro não localizado");
                }
                // Atualizo o produto antigo com o novo conteúdo
                produto.Nome = produtoModel.Nome;
                produto.Descricao = produtoModel.Descricao;
                produto.Marca = produtoModel.Marca;
                produto.QuantidadeEstoque = produtoModel.QuantidadeEstoque;
                produto.CodigoDeBarras = produtoModel.CodigoDeBarras;

                _context.Produtos.Update(produto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Produto com ID {Id} atualizado com sucesso", id);
                return Ok(produto);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto com ID {Id}", id);
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}"); 
            }          
        }

        /// <summary>
        /// Deleta um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto a ser removido.</param>
        /// <returns>Erro 404, mensagem de sucesso ou erro 500.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarProduto(int id)
        {
            _logger.LogDebug("Iniciando requisição: DeletarProduto");

            try
            {
                var produto = await _context.Produtos.FindAsync(id);

                if (produto == null)
                {
                    _logger.LogWarning("Produto com ID {Id} não localizado", id);
                    return NotFound("Registro não localizado");
                }

                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Produto com ID {Id} deletado com sucesso", id);
                return Ok($"Conteúdo do ID {id} deletado com sucesso!");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar produto com ID {Id}", id);
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
        public async Task<ActionResult<List<ProdutoModel>>> BuscarPorNomeOuMarca(string? nome, string? marca)
        {
            _logger.LogDebug("Iniciando requisição: BuscarPorNomeOuMarca");

            try
            {
                var produtos = await _context.Produtos
                .Where(p => (nome == null || p.Nome.Contains(nome)) &&
                            (marca == null || p.Marca.Contains(marca)))
                .ToListAsync();

                _logger.LogInformation("Busca por produtos realizada com sucesso. Total {Total}", produtos.Count);
                return Ok(produtos);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos por nome ou marca. Nome: {Nome}, Marca: {Marca}", nome, marca);
                return StatusCode(500, $"Erro interno. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
    }
}
