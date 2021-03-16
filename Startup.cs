using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddControllers();
            services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));
            //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
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
            services.AddScoped<DataContext, DataContext>();
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
