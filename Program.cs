using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            var app = serviceProvider.GetService<App>();
            app.Run();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyDB;Trusted_Connection=True;"));

            services.AddTransient<IRepository<Contact>, Repository<Contact>>();
            services.AddTransient<App>();

            return services.BuildServiceProvider();
        }
    }

    public class App
    {
        private readonly IRepository<Contact> _repository;

        public App(IRepository<Contact> repository)
        {
            _repository = repository;
        }

        public void Run()
        {
            Console.WriteLine("Running app...");
            _repository.Add(new Contact() { Name = "Daniel"});

            // Use repository to perform CRUD operations on MyEntity
        }
    }

    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        T GetById(int id);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            _dbContext.SaveChanges();
        }

        public void Remove(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<Contact> Contact { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
