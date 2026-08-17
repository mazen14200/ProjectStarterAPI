using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class FullNameMinPartsAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly int _minParts;
    private readonly string _pattern;

    public Type ErrorMessageResourceType { get; set; }
    public string ErrorMessageResourceName { get; set; }

    public FullNameMinPartsAttribute(int minParts, string pattern)
    {
        _minParts = minParts;
        _pattern = pattern;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        var str = value.ToString();

        str = Regex.Replace(str.Trim(), @"\s+", " ");

        if (!Regex.IsMatch(str, _pattern))
            return new ValidationResult(GetErrorMessage());

        var parts = str.Split(' ');
        if (parts.Length < _minParts)
            return new ValidationResult(GetErrorMessage());

        var prop = validationContext.ObjectType.GetProperty(validationContext.MemberName);
        prop?.SetValue(validationContext.ObjectInstance, str);

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-fullname", GetErrorMessage());
        MergeAttribute(context.Attributes, "data-val-fullname-minparts", _minParts.ToString());
        MergeAttribute(context.Attributes, "data-val-fullname-pattern", _pattern);
    }

    private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (attributes.ContainsKey(key))
            return false;

        attributes.Add(key, value);
        return true;
    }

    private string GetErrorMessage()
    {
        if (ErrorMessageResourceType != null && !string.IsNullOrEmpty(ErrorMessageResourceName))
        {
            var prop = ErrorMessageResourceType.GetProperty(ErrorMessageResourceName);
            return prop?.GetValue(null)?.ToString() ?? "Invalid value";
        }

        return ErrorMessage ?? "Invalid value";
    }
}