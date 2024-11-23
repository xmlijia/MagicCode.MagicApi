using MagicCode.MagicApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using MagicCode.MagicSir.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace MagicCode.MagicApi.Conventions
{
    public class MagicApiApplicationModelConvention : IApplicationModelConvention
    {
        IServiceCollection services;
        public MagicApiApplicationModelConvention(IServiceCollection services)
        {
            this.services = services;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (typeof(IMagicApi).IsAssignableFrom(controller.ControllerType) || controller.ControllerType.GetCustomAttribute<MagicApiAttribute>() != null)
                {
                    ConfigureApplicationService(controller);
                }
            }
        }

        private void ConfigureApplicationService(ControllerModel controller)
        {
            // ConfigureApiExplorer(controller);
            ConfigureSelector(controller);
            ConfigureParameters(controller);

        }

        private void ConfigureApiExplorer(ControllerModel controller)
        {
            if (!controller.ApiExplorer.IsVisible.HasValue)
            {
                controller.ApiExplorer.IsVisible = true;
            }

            foreach (var action in controller.Actions)
            {
                if (!action.ApiExplorer.IsVisible.HasValue)
                {
                    action.ApiExplorer.IsVisible = true;
                }
            }
        }
        public void ConfigureSelector(ControllerModel controller)
        {
            RemoveEmptySelectors(controller.Selectors);
            if (controller.Selectors.Any(temp => temp.AttributeRouteModel != null))
            {
                return;
            }

            foreach (var action in controller.Actions)
            {
                ConfigureActionSelector(action);
            }
        }
        private void ConfigureParameters(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                foreach (var parameter in action.Parameters)
                {
                    if (parameter.BindingInfo != null)
                    {
                        continue;
                    }

                    if (parameter.ParameterType.IsClass &&
                        parameter.ParameterType != typeof(string) &&
                        parameter.ParameterType != typeof(IFormFile))
                    {
                        var httpMethods = action.Selectors.SelectMany(temp => temp.ActionConstraints).OfType<HttpMethodActionConstraint>().SelectMany(temp => temp.HttpMethods).ToList();
                        if (httpMethods.Contains("GET") ||
                            httpMethods.Contains("DELETE") ||
                            httpMethods.Contains("TRACE") ||
                            httpMethods.Contains("HEAD"))
                        {
                            continue;
                        }

                        parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                    }
                }
            }
        }
        private void ConfigureActionSelector(ActionModel action)
        {

            var routeSelector = action.Selectors.FirstOrDefault();//.FirstOrDefault(s=>s.AttributeRouteModel != null || s.ActionConstraints.Any(a=>a is HttpMethodActionConstraint));

            if (routeSelector == null)
            {
                routeSelector = new SelectorModel();
                action.Selectors.Add(routeSelector);
            }

            if (routeSelector.AttributeRouteModel == null)
            {
                routeSelector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(GenerateRouteTemplate(action)));
            }
            if (routeSelector.ActionConstraints.Any(a => a is HttpMethodActionConstraint) == false)
            {
                routeSelector.ActionConstraints.Add(DefineHttpMethod(action));

            }

        }
        Dictionary<string, string[]> preWords = new Dictionary<string, string[]>
        {
            {"GET",new string[]{"get"} },
            {"POST",new string[]{ "post", "create", "add", "insert"} } ,
            {"PUT",new string[]{ "put", "update", "save" } },
            {"DELETE",new string[]{ "delete", "remove" } },
            {"PATCH",new string[]{ "patch" } }

        };

        private string GenerateRouteTemplate(ActionModel action)
        {
            var defaultRouteModel = action.Selectors.Where(s => s.AttributeRouteModel != null);
            //var defaultConstraints = action.Selectors.Where(s => s.ActionConstraints != null);
            if (defaultRouteModel.Any(rm => rm.AttributeRouteModel.Template?.Length > 0))
            {
                return defaultRouteModel.FirstOrDefault(rm => rm.AttributeRouteModel.Template?.Length > 0)?.AttributeRouteModel.Template;
            }

            string template = "";
            // 控制器名称部分

            var match = Regex.Match(action.Controller.ControllerName, @"^(?<controllerName>[\w]+)(?:((Application|App)Services?))$", RegexOptions.IgnoreCase);

            var controllerName = match.Success ? match.Groups["controllerName"].Value : action.Controller.ControllerName;
            string actionName = "";



            if (preWords.Any(i => i.Value.Contains(action.ActionName.ToLower())))
            {

                actionName = "";

            }
            else
            {
                var words = from item in preWords
                            from word in item.Value
                            select word;

                ;
                match = Regex.Match(action.ActionName, $"^({string.Join('|', words.Distinct())})?(?<actionName>[\\w]+)(async)?$", RegexOptions.IgnoreCase);
                actionName = match.Groups["actionName"].Value;
            }
            if (string.IsNullOrEmpty(actionName))
            {
                template = controllerName.FromCamelCase("-");
            }
            else
            {

                template = $"{controllerName.FromCamelCase("-")}/{actionName.FromCamelCase("-")}";
            }
            var parameters = action.Parameters.Where(p => p.Attributes?.Any() == false || p.Attributes.Any(a => a is FromRouteAttribute));
            if (parameters.Any())
            {
                template = $"{template}/{string.Join('/', parameters.Select(p => $"{{{p.Name.FromCamelCase("-")}}}").Distinct())}";
            }

            return $"api/{template}";
        }
        public HttpMethodActionConstraint DefineHttpMethod(ActionModel action)
        {
            var constraint = (HttpMethodActionConstraint)action.Selectors.FirstOrDefault(s => s.ActionConstraints.Any(a => a is HttpMethodActionConstraint && ((HttpMethodActionConstraint)a).HttpMethods.Any()))?.ActionConstraints.FirstOrDefault();
            if (constraint != null)
            {
                return constraint;
            }
            string method = preWords.FirstOrDefault(p => p.Value.Contains(action.ActionName.ToLower())).Key ?? "";
            if (string.IsNullOrEmpty(method))
            {
                foreach (var item in preWords)
                {
                    var match = Regex.Match(action.ActionName, $"^({string.Join('|', item.Value.Distinct())})([\\w]+)$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        method = item.Key;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(method))
                    method = "POST";

            }

            return new HttpMethodActionConstraint(new string[] { method });
        }
        private void RemoveEmptySelectors(IList<SelectorModel> selectors)
        {
            selectors = selectors.Where(s => s.AttributeRouteModel != null || s.ActionConstraints?.Any() == true || s.EndpointMetadata?.Any() == true).Distinct().ToList();

            //for (var i = selectors.Count - 1; i >= 0; i--)
            //{
            //    var selector = selectors[i];
            //    if (selector.AttributeRouteModel == null &&
            //        (selector.ActionConstraints == null || selector.ActionConstraints.Count <= 0) &&
            //        (selector.EndpointMetadata == null || selector.EndpointMetadata.Count <= 0))
            //    {
            //        selectors.Remove(selector);
            //    }
            //}
        }

    }
}
