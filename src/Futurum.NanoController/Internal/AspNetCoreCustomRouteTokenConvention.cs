using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Futurum.NanoController.Internal;

internal class AspNetCoreCustomRouteTokenConvention : IApplicationModelConvention
{
    private readonly string _tokenRegex;
    private readonly Func<ControllerModel, string?> _valueGenerator;

    public AspNetCoreCustomRouteTokenConvention(string tokenName, Func<ControllerModel, string?> valueGenerator)
    {
        _tokenRegex = $@"(\[{tokenName}])(?<!\[\1(?=]))";
        _valueGenerator = valueGenerator;
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var tokenValue = _valueGenerator(controller);
            UpdateSelectors(controller.Selectors, tokenValue);
            UpdateSelectors(controller.Actions.SelectMany(a => a.Selectors), tokenValue);
        }
    }

    private void UpdateSelectors(IEnumerable<SelectorModel> selectors, string? tokenValue)
    {
        foreach (var selector in selectors.Where(s => s.AttributeRouteModel != null))
        {
            selector.AttributeRouteModel.Template = InsertTokenValue(selector.AttributeRouteModel.Template, tokenValue);
            selector.AttributeRouteModel.Name = InsertTokenValue(selector.AttributeRouteModel.Name, tokenValue);
        }
    }

    private string? InsertTokenValue(string? template, string? tokenValue) =>
        template is null 
            ? template 
            : Regex.Replace(template, _tokenRegex, tokenValue.ToSlugify());
}