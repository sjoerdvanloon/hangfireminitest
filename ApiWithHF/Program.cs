using Hangfire;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseInMemoryStorage());
builder.Services.AddHangfireServer();

var app = builder.Build();
app.UseRouting();
app.UseHangfireDashboard();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHangfireDashboard();
});
app.MapGet("/", () => "Hello World!");
app.MapPost("/hugs",  (Hug hug) =>
    {
        var backgroundJobClient = app.Services.GetService<IBackgroundJobClient>();
        for (int i = 0; i < 10; i++)
        {
            var waitForMs = Random.Shared.NextInt64(4000) ;
            Console.WriteLine($"Wait for {waitForMs} ms");
         // await  Task.Delay(TimeSpan.FromMilliseconds(waitForMs));
          
          backgroundJobClient.Schedule(() => Console.WriteLine($"Hello, {i} waited for {waitForMs} ms"), TimeSpan.FromMilliseconds((waitForMs)));

        }
        // backgroundJobClient.Create(
        //       "myrecurringjob",
        //       () => Console.WriteLine("Recurring!"),
        //       Cron.Minutely);
        

        
        return Results.Ok(new Hugged(hug.Name, "Side Hug"));
    }
);
app.Run();



var backgroundJobClient = app.Services.GetService<IBackgroundJobClient>();
backgroundJobClient.Enqueue(() => Console.WriteLine("Hello, world! 222"));
public record Hug(string Name);
public record Hugged(string Name, string Kind);

