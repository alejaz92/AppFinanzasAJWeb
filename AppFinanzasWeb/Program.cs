using AppFinanzasWeb.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
builder.Services.AddTransient<IRepositorioTarjetas, RepositorioTarjetas>();
builder.Services.AddTransient<IRepositorioTiposActivo, RepositorioTiposActivo>();
builder.Services.AddTransient<IRepositorioActivos, RepositorioActivos>();
builder.Services.AddTransient<IRepositorioClaseMovimientos, RepositorioClaseMovimientos>();
builder.Services.AddTransient<IRepositorioMovimientos, RepositorioMovimientos>();
builder.Services.AddTransient<IRepositorioMovTarjetas, RepositorioMovTarjetas>();
builder.Services.AddTransient<IRepositorioCotizacionesActivos, RepositorioCotizacionActivo>();
builder.Services.AddTransient<IRepositorioPagosTarjeta, RepositorioPagoTarjeta>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
