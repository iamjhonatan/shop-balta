using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shop.Data;

namespace Shop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            // Comprimindo o JSON antes de mandar pra tela | o HTML descompacta na tela
            services.AddResponseCompression(options =>
            {
                // comprimindo todos os arquivos que são 'application/json'
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });
            // services.AddResponseCaching();
            services.AddControllers();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            //services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
            /*
            Injeção de dependência: o Controller é dependente da conexão com o banco de dados para retornar o que
            o frontend requisitou. Por isso, uma conexão com o banco é aberta para que o Controller busque o que 
            necessita. E essa conexão deve ser fechada, para que não haja conexões sem uso abertas e o banco caia
            por estar com o limite de conexões esgotado. Assim, na linha de baixo, essa injeção de dependência é feita.
            As dependência do .NetCore são tratadas pelo 'AddScoped', 'AddTransient' e 'AddSingleton'. 
            O 'AddScoped' vai garantir que só haja um DataContext por requisição. Toda vez que acontecer uma requisição para
            a aplicação, ele vai criar um 'DataContext' na memória. Toda vez que o Controller pedir um DataContext, ele 
            vai mandar o mesmo DataContext da memória. Toda vez que um DataContext é criado, ele abre uma conexão com o banco.
            Toda vez que a requisição acaba, o 'AddScoped' automaticamente destrói o DataContext, que destrói a conexão com o banco.
            */

            // trabalhando com documentação
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop v1"));
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1");
            });
            app.UseRouting();

            // permissões do localhost
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
