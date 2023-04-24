using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace MinhaApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly string connectionString = "Host=localhost;Username=postgres;Password= 123;Database=postgres";

        [HttpPost]
        public IActionResult Autenticar([FromBody] AutenticacaoRequest request)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM usuarios WHERE usuario = @usuario AND senha = @senha";
                    cmd.Parameters.AddWithValue("usuario", request.Usuario);
                    cmd.Parameters.AddWithValue("senha", request.Senha);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Ok(new AutenticacaoResponse { Autenticado = true });
                        }
                        else
                        {
                            return Unauthorized(new AutenticacaoResponse { Autenticado = false });
                        }
                    }
                }
            }
        }
    }

    public class AutenticacaoRequest
    {
        public string Usuario { get; set; } = "";
        public string Senha { get; set; } = "";
    }

    public class AutenticacaoResponse
    {
        public bool Autenticado { get; set; }
    }
}


namespace Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
