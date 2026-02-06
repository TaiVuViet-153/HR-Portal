var builder = DistributedApplication.CreateBuilder(args);

// Add Backend Service
var authService = builder
        .AddProject<Projects.Api>("Auth");

var requestService = builder
        .AddProject<Projects.RequestApi>("Request");

var userService = builder
        .AddProject<Projects.EmployeeApi>("Employee");


// Add FrontEnd Vite
var frontEnd = builder
        .AddViteApp("FrontEnd", "../../FrontEnd")
        .WithHttpsEndpoint(port: 5127, isProxied: false, env: "PORT") // AppHost auto generate port for FrontEnd
                                                                      // Map to backend api
        .WithUrl("/login", "Login")
        .WithReference(authService)
        .WithReference(requestService)
        // Map endpoint to VITE_* env in front end
        .WithEnvironment("VITE_AUTH_BASE_URL", authService.GetEndpoint("https"))
        .WithEnvironment("VITE_REQUEST_BASE_URL", requestService.GetEndpoint("https"))
        .WithEnvironment("VITE_EMPLOYEE_BASE_URL", userService.GetEndpoint("https"));

builder.Build().Run();
