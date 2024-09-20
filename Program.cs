var builder = WebApplication.CreateBuilder(args);

//Login 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable session middleware
app.UseSession(); // Must be added before app.MapControllerRoute

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
// enables the authentication middleware in ASP.NET Core to handle user authentication for securing endpoints.​
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
