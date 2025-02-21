namespace BackendRaith.Helper
{
    public static class ExtentionMethods
    {
        public static T CopyPropertiesFrom<T>(this T target, object source) => target.CopyPropertiesFrom(source, null);
        public static T CopyPropertiesFrom<T>(this T target, object source, string[]? ignoreProperties)
        {
            if (target == null) return target;
            ignoreProperties ??= Array.Empty<string>();

            var propsSource = source.GetType().GetProperties()
                .Where(x => x.CanRead && !ignoreProperties.Contains(x.Name));

            var propsTarget = target.GetType().GetProperties()
                .Where(x => x.CanWrite);

            foreach (var prop in propsTarget.Where(targetProp => propsSource.Any(sourceProp => sourceProp.Name == targetProp.Name)))
            {
                try
                {
                    var propSource = propsSource.First(x => x.Name == prop.Name);
                    if (propSource.PropertyType.IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(target, propSource.GetValue(source));
                    }
                    else
                    {
                        Console.WriteLine($"Typenmissmatch: {prop.Name} kann nicht von {propSource.PropertyType} nach {prop.PropertyType} konvertiert werden.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Kopieren der Eigenschaft '{prop.Name}': {ex.Message}");
                }
            }

            return target;
        }
    }
}