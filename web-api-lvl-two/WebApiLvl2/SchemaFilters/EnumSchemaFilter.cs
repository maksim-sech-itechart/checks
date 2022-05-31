namespace WebApiLvl2.SchemaFilters;

[UsedImplicitly]
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
        {
            return;
        }

        schema.Enum.Clear();
        foreach (var name in Enum.GetNames(context.Type))
        {
            schema.Enum.Add(new OpenApiString(name));
        }
    }
}