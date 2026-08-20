using SilentMoonApp.WebAPI;
using SilentMoonApp.Application;
using SilentMoonApp.Infrastructure;
using SilentMoonApp.WebAPI.Extensions;
using SilentMoonApp.Infrastructure.Persistence.Seed;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddWebAPILayer(builder.Configuration)
				.AddInfrastructureLayer(builder.Configuration)
				.AddApplicationLayer();



var app = builder.Build();


await app.Services.SeedDefaultSchemaAsync();


app.UserLocalization();

app.UseExceptionHandler();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

app.UseSwagger();

app.UseSwaggerUI(options =>
{
	options.DisplayRequestDuration();
});


app.UseHttpsRedirection();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
