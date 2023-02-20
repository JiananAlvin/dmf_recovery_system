// See https://aka.ms/new-console-template for more information
using ExecutionEngine;
using Newtonsoft.Json;
using System.IO;

// Read the JSON file contents into a string
string json = File.ReadAllText(@"../../../platform640v2.json");

// Deserialize the JSON string into a C# object
dynamic obj = JsonConvert.DeserializeObject(json);

// Access object properties
int sizeX = obj.information.sizeX;
int sizeY = obj.information.sizeY;

// Print object properties
Console.WriteLine($"sizeX: {sizeX}");