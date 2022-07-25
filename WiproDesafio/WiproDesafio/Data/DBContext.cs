using Microsoft.EntityFrameworkCore;
using WiproDesafio.Models;

namespace WiproDesafio.Data {
    public class DBContext : DbContext {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; } = null!;
    }
}
