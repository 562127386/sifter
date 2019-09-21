using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Sifter.Terms {

//TODO this currently has no support for OR/combining statements, maybe add later
    internal class FilterTerm {

        public string Identifier { get; }

        private readonly string op;
        private readonly string variable;
        private readonly SimpleType? variableType;



        public FilterTerm(string str) {
            var match = Regexes.FILTER_TERM_REGEX.Match(str);
            Identifier = match.Groups["identifier"].Value;
            op = match.Groups["operator"].Value;
            variable = match.Groups["variable"].Value;

            variableType = match.GetSimpleType();
        }



        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, PropertyInfo propertyInfo) {
            var propertyType = propertyInfo.PropertyType;

            if (variableType == null || variableType != propertyType.ToSimpleType()) {
                return query;
            }

            var parameterExpr = Expression.Parameter(typeof(T));
            var propertyExpr = createPropertyExpression(parameterExpr, Identifier);
            var memberAccess = Expression.MakeMemberAccess(propertyExpr, propertyInfo);
            var convertedVar = JsonConvert.DeserializeObject(variable);
            var varExpr = Expression.Constant(convertedVar, convertedVar.GetType());
            var convertedMemberAccess = Expression.Convert(memberAccess, convertedVar.GetType());
            var whereExpr = getExpression(propertyType, convertedMemberAccess, varExpr);

            return whereExpr == null ? query : query.Where(Expression.Lambda<Func<T, bool>>(whereExpr, parameterExpr));
        }



        private static Expression createPropertyExpression(Expression parameterExpr, string identifier) {
            if (!identifier.Contains('.')) {
                return parameterExpr;
            }

            var splits = identifier.Split('.');
            return splits.Aggregate(parameterExpr, Expression.Property);
        }



        private static readonly MethodInfo likeMethodInfo =
            typeof(DbFunctionsExtensions).GetMethods().First(m => m.Name == "Like" && m.GetParameters().Length == 3);

        private static readonly Expression efFunctionsExpr = Expression.Constant(EF.Functions);



        [CanBeNull]
        private Expression getExpression(Type propertyType, Expression propExpr, Expression varExpr) {
            return op switch {
                "==" => Expression.Equal(propExpr, varExpr),
                "!=" => Expression.NotEqual(propExpr, varExpr),
                _ => _typeSpecificSwitch()
            };


            Expression _typeSpecificSwitch() {
                if (propertyType == typeof(string)) {
                    return op switch {
                        "==*" => Expression.Call(
                            likeMethodInfo,
                            efFunctionsExpr,
                            propExpr,
                            varExpr
                        ),
                        "!=*" => (Expression) Expression.Not(
                            Expression.Call( //TODO negate expr
                                likeMethodInfo,
                                efFunctionsExpr,
                                propExpr,
                                varExpr
                            )
                        ),
                        _ => null
                        //TODO add these
//                        "@" => Operator.CONTAINS,
//                        "!@" => Operator.DOES_NOT_CONTAIN,
//                        "^" => Operator.STARTS_WITH,
//                        "!^" => Operator.DOES_NOT_START_WITH,
//                        "$" => Operator.ENDS_WITH,
//                        "!$" => Operator.DOES_NOT_END_WITH,
//                        "@*" => Operator.CONTAINS_INSENSITIVE,
//                        "!@*" => Operator.DOES_NOT_CONTAIN_INSENSITIVE,
//                        "^*" => Operator.STARTS_WITH_INSENSITIVE,
//                        "!^*" => Operator.DOES_NOT_START_WITH_INSENSITIVE,
//                        "$*" => Operator.ENDS_WITH_INSENSITIVE,
//                        "!$*" => Operator.DOES_NOT_END_WITH_INSENSITIVE
                    };
                }

                return op switch {
                    ">" => Expression.GreaterThan(propExpr, varExpr),
                    "<" => Expression.LessThan(propExpr, varExpr),
                    ">=" => Expression.GreaterThanOrEqual(propExpr, varExpr),
                    "<=" => Expression.LessThanOrEqual(propExpr, varExpr),
                    _ => null
                };
            }
        }

    }


    internal enum SimpleType {

        STRING,
        NUMBER,
        BOOL,
        ENUM

    }


    internal static class VariableTypeExtensions {

        public static SimpleType? GetSimpleType(this Match match) {
            if (!string.IsNullOrEmpty(match.Groups["string"].Value)) {
                return SimpleType.STRING;
            }

            if (!string.IsNullOrEmpty(match.Groups["number"].Value)) {
                return SimpleType.NUMBER;
            }

            if (!string.IsNullOrEmpty(match.Groups["bool"].Value)) {
                return SimpleType.BOOL;
            }

            if (!string.IsNullOrEmpty(match.Groups["enum"].Value)) {
                //TODO add support for enum
                return SimpleType.ENUM;
            }
            //TODO add support for Date, DateTime, etc..

            return null;
        }



        public static SimpleType? ToSimpleType(this Type type) {
            if (type == typeof(string)) {
                return SimpleType.STRING;
            }

            if (type == typeof(bool)) {
                return SimpleType.BOOL;
            }

            if (type.IsPrimitive) {
                return SimpleType.NUMBER;
            }

            if (type.IsEnum) {
                return SimpleType.ENUM;
            }

            return null;
        }

    }

}