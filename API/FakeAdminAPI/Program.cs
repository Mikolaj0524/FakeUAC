
namespace FakeAdminAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseAuthorization();

            app.MapControllers();

            app.Run("http://localhost:9876");
        }
    }
}
