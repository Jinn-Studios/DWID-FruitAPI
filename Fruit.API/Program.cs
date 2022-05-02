namespace FruitUI.API
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            var app = builder.Build();
            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();
        }
    }
}