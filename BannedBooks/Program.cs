using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BannedBooks.Data;


var builder = WebApplication.CreateBuilder(args);
// Use the MySQL connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


// 2. Register your DbContext with MySQL
builder.Services.AddDbContext<BannedBooksContext>(options =>
    options.UseMySql(
        connectionString,
        // Adjust the version to match your local MySQL server version
        new MySqlServerVersion(new Version(8, 0, 41))
    )
);

// 3. Configure Identity to use your DbContext
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Example setting: require email confirmation
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<BannedBooksContext>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Books");
});

// Register AiService using HttpClient
builder.Services.AddHttpClient<BannedBooks.Services.AiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();


