using Cocona;
using M5x.Cocona;

var builder = CoCli.CreateAppBuilder();

var app = builder.Build();

app.AddCommand("parse", (string name, int age) =>
{
    Console.WriteLine($"Hi there, {name}. You are {age} years old.");
});

app.Run();
