using System.Reflection;
using NJsonSchema.Generation;

namespace Api.Etc.NSwag;

public class RequiredSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;
        var contextualType = context.ContextualType;

        if (contextualType?.Type == null || !contextualType.Type.IsClass)
            return;

        // Get all properties of the type
        var properties = contextualType.Type.GetProperties(
            BindingFlags.Public | BindingFlags.Instance
        );

        foreach (var property in properties)
        {
            var propertyName = GetJsonPropertyName(property);

            // Skip if property doesn't exist in schema
            if (!schema.Properties.ContainsKey(propertyName))
                continue;

            var isNullable = IsPropertyNullable(property);

            // If the property is NOT nullable, mark it as required
            if (!isNullable)
            {
                if (!schema.RequiredProperties.Contains(propertyName))
                {
                    schema.RequiredProperties.Add(propertyName);
                }
            }
        }
    }

    private static string GetJsonPropertyName(PropertyInfo property)
    {
        // Check for JsonPropertyName attribute
        var jsonPropertyAttr =
            property.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>();
        if (jsonPropertyAttr != null)
            return jsonPropertyAttr.Name;

        // Default to camelCase (NSwag's default behavior)
        return char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
    }

    private static bool IsPropertyNullable(PropertyInfo property)
    {
        var propertyType = property.PropertyType;

        // Check if it's Nullable<T> (value types)
        if (
            propertyType.IsGenericType
            && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
        )
            return true;

        // For value types that are not Nullable<T>, they're not nullable
        if (propertyType.IsValueType)
            return false;

        // For reference types, check nullable annotations
        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(property);

        return nullabilityInfo.WriteState == NullabilityState.Nullable;
    }
}
