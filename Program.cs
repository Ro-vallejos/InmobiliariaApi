using inmobiliariaApi.Repositorios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using inmobiliariaApi;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

var cs = builder.Configuration.GetConnectionString("Mysql");
builder.Services.AddDbContext<DataContext>(opt =>
{
	var serverVersion = ServerVersion.AutoDetect(cs);
    opt.UseMySql(cs, serverVersion).UseSnakeCaseNamingConvention();
});


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllLocal", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed(_ => true)
        .DisallowCredentials()); // para API stateless; si usás cookies en otro origen, creá otra policy
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Usuario/Login";
		options.LogoutPath = "/Usuarios/Logout";
		options.AccessDeniedPath = "/Home/Restringido";
	})
	.AddJwtBearer(options =>//la api web valida con token
	{
		var secreto = configuration["TokenAuthentication:SecretKey"];
		if (string.IsNullOrEmpty(secreto))
			throw new Exception("Falta configurar TokenAuthentication:Secret");
		options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = configuration["TokenAuthentication:Issuer"],
			ValidAudience = configuration["TokenAuthentication:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secreto)),
		};
		options.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				var accessToken = context.Request.Query["access_token"];
				var path = context.HttpContext.Request.Path;
				if (!string.IsNullOrEmpty(accessToken) &&
					(path.StartsWithSegments("/chatsegurohub") ||
					path.StartsWithSegments("/api/propietarios/reset") ||
					path.StartsWithSegments("/api/propietarios/token")))
				{//reemplazar las urls por las necesarias ruta ⬆
					context.Token = accessToken;
				}
				return Task.CompletedTask;
			},
			OnTokenValidated = context =>
			{
				Console.WriteLine("Token válido para el usuario: " + context?.Principal?.Identity?.Name);
				return Task.CompletedTask;
			},
			OnAuthenticationFailed = context =>
			{
				Console.WriteLine("Error en la autenticación del token: " + context.Exception.Message);
				return Task.CompletedTask;
			}
		};
	});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Admin"));
});




builder.Services.AddTransient<IRepositorioInmueble, RepositorioInmueble>();
builder.Services.AddTransient<IRepositorioPropietario, RepositorioPropietario>();
//builder.Services.AddTransient<IRepositorioPago, RepositorioPago>();
//builder.Services.AddTransient<IRepositorioContrato, RepositorioContrato>();

builder.WebHost.UseUrls("http://0.0.0.0:5145");


var app = builder.Build();
app.MapGet("/", () => Results.Ok("API OK"));
app.MapGet("/ping", () => "pong");




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAllLocal");

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();