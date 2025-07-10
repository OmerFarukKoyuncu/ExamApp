namespace BAExamApp.MVC.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Kelimeleri başlık tipine formatlar.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Formatlanmış string veri döndürür</returns>
    public static string TitleFormat(this string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        return name;
    }

    /// <summary>
    /// Converts a string to Pascal Case with spaces between words.
    /// </summary>
    /// <param name="input">The input string to be converted.</param>
    /// <returns>The converted string in Pascal Case with spaces.</returns>
    public static string ToPascalCaseWithSpaces(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < words.Length; i++)
        {
            words[i] = words[i].Substring(0, 1).ToUpper() + words[i].Substring(1).ToLower();
        }

        return string.Join(" ", words);
    }

    /// <summary>
    /// Capitalizes the first letter of a string and converts the rest to lowercase.
    /// </summary>
    /// <param name="input">The input string to be converted.</param>
    public static string CapitalizeFirstLetter(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input.Substring(0, 1).ToUpper() + input.Substring(1).ToLower();
    }
    /// <summary>
    /// when applied to a string which happens to be a name of a property type 
    /// returns the normie version of the name, when applied to a string
    /// which happens NOT to be a name of a property type returns itself
    /// </summary>
    /// <param name="typeName">The input string to be normied.</param>
    public static string GetCustomTypeName(this string typeName)
    {
        return typeName switch
        {
            "String" => "Text",
            "Int32" => "Number",
            "Boolean" => "Yes/No",
            _ => typeName // Default to original type name
        };
    }

}
