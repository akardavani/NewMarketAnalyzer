using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace MarketAnalyzer.Abstractions.Routes
{
    public class RouteDescriptor
    {
        public RouteDescriptor(ControllerActionDescriptor actionDescriptor)
        {
            Controller = GetControllerName(actionDescriptor);
            Action = GetActionName(actionDescriptor);
            Path = $"{actionDescriptor.ControllerName}/{actionDescriptor.ActionName}";
            HashCode = ComputeHash(Path.ToLower());

            var roles = GetRequiredRoles(actionDescriptor);
            RequiredRoles = roles.RequiredRules;
            AuthorizedRoles = roles.AuthorizedRoles;
        }

        public RouteDescriptor(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            Path = path;
            HashCode = ComputeHash(path.ToLower());
        }

        public string Controller { get; }

        public string Action { get; }

        public string Path { get; }

        public string HashCode { get; }

        public IReadOnlyCollection<string> RequiredRoles { get; }

        public IReadOnlyCollection<string> AuthorizedRoles { get; }

        private static string GetControllerName(ControllerActionDescriptor actionDescriptor)
        {
            return actionDescriptor.ControllerTypeInfo
                .GetCustomAttribute<RouteInfoAttribute>()?.DisplayName ??
                actionDescriptor.ControllerName;
        }

        private static string GetActionName(ControllerActionDescriptor actionDescriptor)
        {
            var displayName = actionDescriptor
                .MethodInfo
                .GetCustomAttribute<RouteInfoAttribute>()?
                .DisplayName;

            if (!string.IsNullOrEmpty(displayName))
            {
                return displayName;
            }

            return actionDescriptor.ActionName switch
            {
                "GetAll" => "مشاهده همه",
                "Get" => "مشاهده",
                "Create" => "ایجاد",
                "Update" => "ویرایش",
                "Remove" => "حذف",
                "Print" => "چاپ",
                _ => actionDescriptor.ActionName,
            };
        }

        private static Roles GetRequiredRoles(ControllerActionDescriptor actionDescriptor)
        {
            var attribute = actionDescriptor
               .MethodInfo
               .GetCustomAttribute<RouteInfoAttribute>();

            if (attribute is null)
            {
                return new Roles()
                {
                    AuthorizedRoles = [],
                    RequiredRules = []
                };
            }
            else
            {
                return new Roles()
                {
                    RequiredRules = attribute?.RequiredRoles?.ToList() ?? [],
                    AuthorizedRoles = attribute?.AuthorizedRoles?.ToList() ?? []
                };
            }
        }

        private static string ComputeHash(string input)
        {
            var buffer = MD5.HashData(System.Text.Encoding.UTF8.GetBytes(input));
            return string.Concat(buffer.Select(x => x.ToString("x2")));
        }

        private class Roles
        {
            public IReadOnlyCollection<string> RequiredRules { get; init; }
            public IReadOnlyCollection<string> AuthorizedRoles { get; init; }
        }
    }
}
