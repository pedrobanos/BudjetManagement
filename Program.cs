using BudjetManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<ITypesAccountsRepo, TypesAccountsRepo>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAccountsRepo, AccountsRepo>();
builder.Services.AddTransient<ICategoriesRepo, CategoriesRepo>();
builder.Services.AddTransient<ITransactionsRepo, TransactionsRepo>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IReportsService, ReportsService>();
builder.Services.AddAutoMapper(typeof(Program));


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
    pattern: "{controller=Transactions}/{action=Index}/{id?}");

app.Run();
