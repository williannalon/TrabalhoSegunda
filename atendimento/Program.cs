using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;



public class Solicitacao
{
    public int SolicitacaoId { get; set; }
    public String? Prioridade { get; set; }
    public string? Setor { get; set; }

    public String? MotivoSolicitacao { get; set; }
    public int CodigoDeAcessoMaquina { get; set; } //para o suporte acessa ex:anydesk

    public ICollection<Suporte>? Suportes { get; set; }
}
public class Suporte
{
    public int SuporteId { get; set; }
    public string? Nome { get; set; }
    public string? Nivel { get; set; }
    public bool Ativo { get; set; }
    public ICollection<Solicitacao>? Solicitacoes { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Solicitacao>? Solicitacoes { get; set; }
    public DbSet<Suporte>? Suportes { get; set; }

    //Reforso do mapeamento
    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Suporte>().HasKey(s => s.SuporteId);
        mb.Entity<Solicitacao>().HasKey(s => s.SolicitacaoId);
    }


}
class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

         //builder.Services.AddDbContext<AppDbContext>(options =>
          //  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        var app = builder.Build();

        //endpoints Suporte
        //adicionando sup
        app.MapPost("/suportes", async (Suporte suporte,AppDbContext db ) => {
            db.Suportes.Add(suporte);
            await db.SaveChangesAsync();

            return Results.Created($"/suportes/{suporte.SuporteId}",suporte);
        });

        //retorna lista de suporte
        app.MapGet("/suportes", async (AppDbContext db) => await db.Suportes.ToListAsync());

        //Para Obter um suporte pelo seu id 
        app.MapGet("/suportes/{id:int}",async (int id, AppDbContext db) => 
        {
            return await db.Suportes.FindAsync(id)
            is Suporte suporte ? Results.Ok(suporte) : Results.NotFound();
        });

        //Atualizar um Suporte pelo ID
        app.MapPut("/suportes/{id:int}",async (int id,Suporte suporte, AppDbContext db) =>
        {
            if(suporte.SuporteId != id){
                return Results.BadRequest();
            }
            var suporteDB = await db.Suportes.FindAsync(id);
            if (suporteDB is null) return Results.NotFound();
            
            suporteDB.Nome = suporte.Nome;
            suporteDB.Nivel = suporte.Nivel;
            suporteDB.Ativo = suporte.Ativo;

            await db.SaveChangesAsync();
            return Results.Ok(suporteDB);
        });

        //Deletar pelo ID
        app.MapGet("/suportes/{id:int}",async (int id, AppDbContext db) => 
        {  
            var suporte = await db.Suportes.FindAsync(id);
            if(suporte is null) return Results.NotFound();

            db.Suportes.Remove(suporte);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });


        //adicionando solicitacao
        app.MapPost("/solicitacao", async (Solicitacao solicitacao,AppDbContext db ) => {
        db.Solicitacoes.Add(solicitacao);
        await db.SaveChangesAsync();

        return Results.Created($"/suportes/{solicitacao.SolicitacaoId}", solicitacao);
        });

        //retorna lista de suporte
        app.MapGet("/solicitacao", async (AppDbContext db) => await db.Solicitacoes.ToListAsync());

        //Para Obter um suporte pelo seu id 
        app.MapGet("/solicitacao/{id:int}",async (int id, AppDbContext db) => 
        {
            return await db.Solicitacoes.FindAsync(id)
            is Solicitacao solicitacao ? Results.Ok(solicitacao) : Results.NotFound();
        });
        
        //atualizar solicitação
        app.MapPut("/solicitacao/{id:int}",async (int id,Solicitacao solicitacao, AppDbContext db) =>
        {
            if(solicitacao.SolicitacaoId != id){
                return Results.BadRequest();
            }
            var solicitacaoDB = await db.Solicitacoes.FindAsync(id);
            if (solicitacaoDB is null) return Results.NotFound();
            
            solicitacaoDB.Prioridade = solicitacao.Prioridade;
            solicitacaoDB.Setor = solicitacao.Setor;
            solicitacaoDB.MotivoSolicitacao = solicitacao.MotivoSolicitacao;
            solicitacaoDB. CodigoDeAcessoMaquina = solicitacao. CodigoDeAcessoMaquina;

            await db.SaveChangesAsync();
            return Results.Ok(solicitacaoDB);
        });

        app.MapGet("/solicitacao/{id:int}",async (int id, AppDbContext db) => 
        {  
            var solicitacao= await db.Solicitacoes.FindAsync(id);
            if(solicitacao is null) return Results.NotFound();

            db.Solicitacoes.Remove(solicitacao);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        

        app.Run();
    }
}