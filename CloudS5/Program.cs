var builder = WebApplication.CreateBuilder(args);

// Ajouter les services nécessaires pour la session et l'injection de dépendances
builder.Services.AddDistributedMemoryCache(); // Utilisation de la mémoire pour stocker les données de session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5); // Durée configurable pour la session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Ajouter le service PinService pour l'injection de dépendances
builder.Services.AddSingleton<PinService>(); // PinService est maintenant disponible pour l'injection de dépendances
builder.Services.AddHttpContextAccessor(); // Ajout de l'accès au contexte HTTP (pour gérer la session)

// Ajouter les contrôleurs avec les vues
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<EmailService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Ajouter les services Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurer le pipeline de traitement des requêtes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Utiliser la session avant la gestion de l'autorisation
app.UseSession();

app.UseAuthorization();

// Configurer Swagger dans le pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Documentation v1");
    options.RoutePrefix = "swagger";  // Utiliser le chemin par défaut pour Swagger UI

});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
