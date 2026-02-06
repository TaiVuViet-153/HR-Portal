- Replace Sdk source
  <Project Sdk="Aspire.AppHost.Sdk/13.1.0"></Project>

- Add Hosting.Javascript package
  dotnet add package Aspire.Hosting
  dotnet add package Aspire.Hosting.Javascript
  At least version 13.0.0 to use IDistributedApplicationBuilder.AddJavaScriptApp in Program.cs
