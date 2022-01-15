using Dog2BoneBackend.Services.Implementations;
using Dog2BoneBackend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args)
        ;

//Allow Cors
builder.Services.AddCors(corsConfig => corsConfig.AddPolicy("_default", b =>
{
    b.AllowAnyOrigin()
     .AllowAnyHeader()
     .AllowAnyMethod();
}));

//Register Singleton Services
builder.Services.AddSingleton<IDog2Bone, Dog2BoneService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
