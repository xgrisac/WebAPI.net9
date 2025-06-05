namespace WebAPI.net9.Models // Modelo da tabela do DB
{
    public class ProdutoModel
    {   // Propriedades que permitem a criação e alteração do produto
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int QuantidadeEstoque { get; set; }
        public string CodigoDeBarras { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;

    }
}
