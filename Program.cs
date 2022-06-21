// See https://aka.ms/new-console-template for more information
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;

Console.WriteLine("Hello, World!");

var serializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
var settings = new JsonSchemaGeneratorSettings
{
    SerializerSettings = serializerSettings,
    SchemaType = SchemaType.OpenApi3
};

var types = new List<Type> { typeof(Pagination), typeof(Page) };

var document = new OpenApiDocument()
{
    OpenApi = "3.0.0",
    Info = new OpenApiInfo
    {
        Title = "Test",
        Version = "1.0.0"
    }
};
var schema = new JsonSchema();
var resolver = new JsonSchemaResolver(schema, settings);
var generator = new JsonSchemaGenerator(settings);

// foreach (var t in types)
// {
//     var nestedTypes = t.GetProperties().Select(p => p.PropertyType).Where(t => !IsSimple(t)).Distinct();
//     foreach (var nt in nestedTypes)
//     {
//         schema = new JsonSchema();
//         generator.Generate(schema, nt, resolver);
//         document.Components.Schemas.Add(nt.Name, schema);
//     }
//     schema = new JsonSchema();
//     generator.Generate(schema, t, resolver);
//     document.Components.Schemas.Add(t.Name, schema);
// }
var typeSet = new HashSet<Type>();
foreach (var t in types)
{
    AddTypeToSchema(t, typeSet, generator, resolver, document);
}

var json = document.ToJson(SchemaType.OpenApi3, Formatting.None);

Console.WriteLine(json);

bool IsSimple(Type type)
{
    return type.IsPrimitive
      || type.Equals(typeof(string))
      || typeof(IEnumerable).IsAssignableFrom(type);
}


void AddTypeToSchema(Type t, HashSet<Type> typeSet, JsonSchemaGenerator generator, JsonSchemaResolver resolver, OpenApiDocument document)
{
    var nestedTypes = t.GetProperties().Select(p => p.PropertyType).Where(t => !IsSimple(t)).Distinct();
    nestedTypes = nestedTypes.Where(nt => JsonSchema.FromType(nt).Type.ToString() == "Object").ToHashSet();
    var typesToAdd = nestedTypes.Except(typeSet);
    foreach (var nt in typesToAdd)
    {
        typeSet.Add(nt);
        AddTypeToSchema(nt, typeSet, generator, resolver, document);
    }
    schema = new JsonSchema();
    generator.Generate(schema, t, resolver);
    document.Components.Schemas.Add(t.Name, schema);
}