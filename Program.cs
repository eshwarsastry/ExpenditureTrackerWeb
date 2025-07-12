using ExpenditureTrackerWeb.Shared.DbContexts;
using ExpenditureTrackerWeb.Shared.Mappers;
using ExpenditureTrackerWeb.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//Enable Cross origin resource sharing.
builder.Services.AddCors(options =>
{

    options.AddPolicy("MyAllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // Specify allowed origins
                   .AllowAnyHeader()                   // Allow any header
                   .AllowAnyMethod();                  // Allow any method (GET, POST, etc.)
        });
});

//Adding Jwt token authetication configuration here.


// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionCategoryService, TransactionCategoryService>();
builder.Services.AddScoped<ITransactionTypeService, TransactionTypeService>();
builder.Services.AddScoped<IExpensesService, ExpensesService>();
builder.Services.AddScoped<IImportDataService, ImportDataService>();
// Add mappers to the container.
builder.Services.AddScoped<IUsersMapper, UsersMapper>();
builder.Services.AddScoped<ICategoriesMapper, CategoriesMapper>();
builder.Services.AddScoped<IExpensesMapper, ExpensesMapper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Context class to the container.
builder.Services.AddDbContext<ApplicationDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("ETAppDbConnectionString")));

var app = builder.Build();

app.UseCors("MyAllowSpecificOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
