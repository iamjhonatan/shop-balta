using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{
    public class DataContext : DbContext
    {
        // DbContext é a representação do Banco de Dados em memória
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        // DbSet é a representação das tabelas em memória
        // Permitirão fazer o 'CRUD' em cima dessas instâncias no banco
        // Esse mapeamento sempre funcionará porque o modelo está sendo feito primeiro, o banco será feito posteriormente
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
    }
}