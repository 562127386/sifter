using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;


namespace Sifter.Terms {

//TODO this currently has no support for OR/combining statements, maybe add later
    internal class FilterTerm {

        public string Identifier { get; }

        private static readonly MethodInfo stringEquals =
            typeof(string).GetMethods().First(m => m.Name == "Equals" && m.GetParameters().Length == 3);

        private static readonly Expression ignoreCaseExpr =
            Expression.Constant(StringComparison.InvariantCultureIgnoreCase);

        private static readonly Expression caseExpr =
            Expression.Constant(StringComparison.InvariantCulture);



        public FilterTerm(string str) {
            var match = Regexes.FILTER_TERM_REGEX.Match(str);
            Identifier = match.Groups["identifier"].Value;
            Operator = match.Groups["operator"].Value;
            Variable = match.Groups["variable"].Value;
        }



        public string Operator { get; }

        public string Variable { get; }



        public IQueryable<T> ApplyFilter<T>(IQueryable<T> query, PropertyInfo propertyInfo) {
            var propertyType = propertyInfo.PropertyType;

            if (!isSimpleType(propertyType)) {
                return query;
            }

            var parameterExpr = Expression.Parameter(typeof(T));
            var propertyExpr = createPropertyExpression(parameterExpr, Identifier);
            var memberAccess = Expression.MakeMemberAccess(propertyExpr, propertyInfo);
//            var classParamExpr = Expression.Parameter(propertyType.)
//            var propParamExpr = Expression.Parameter(propertyInfo.PropertyType);
//            var propertyExpr = createPropertyExpression(propParamExpr, Identifier);
            var convertedVar = convert(Variable, propertyType);
            var varExpr = Expression.Constant(convertedVar, convertedVar.GetType());
            var convertedMemberAccess = Expression.Convert(memberAccess, convertedVar.GetType());
            var whereExpr = getExpression(propertyType, convertedMemberAccess, varExpr);

            return query.Where(Expression.Lambda<Func<T, bool>>(whereExpr, parameterExpr));
        }



        private static object convert(string variable, Type type) {
            if (type == typeof(int) && variable.Contains('.')) {
                type = typeof(float);
            }

            return JsonConvert.DeserializeObject(variable);

            var converter = TypeDescriptor.GetConverter(type);//convert using json

            if (!converter.CanConvertFrom(typeof(string)) || !converter.CanConvertTo(type)) {
                throw new InvalidCastException(type.Name);

                //TODO replace with null after confirming that it works
            }

            return converter.ConvertFromInvariantString(variable);
        }



        private static bool isSimpleType(Type type) {
            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type.IsPrimitive ||
                   type.IsEnum;
        }



        private static Expression createPropertyExpression(Expression parameterExpr, string identifier) {
            if (!identifier.Contains('.')) {
                return parameterExpr;
            }

            var splits = identifier.Split('.');
            return splits.Aggregate(parameterExpr, Expression.Property);
        }



        private Expression getExpression(Type propertyType, Expression propExpr, Expression varExpr) {
            return Operator switch {
                "==" => Expression.Equal(propExpr, varExpr),
                "!=" => Expression.NotEqual(propExpr, varExpr),
                _ => _typeSpecific()
            };


            Expression _typeSpecific() {
                if (propertyType == typeof(string)) {
                    return Operator switch {
                        "==*" => Expression.Call(
                            stringEquals,
                            propExpr,
                            varExpr,
                            ignoreCaseExpr
                        ),
                        "!=*" => Expression.Call(
                            stringEquals,
                            propExpr,
                            varExpr,
                            caseExpr
                        )
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

                return Operator switch {
                    ">" => Expression.GreaterThan(propExpr, varExpr),
                    "<" => Expression.LessThan(propExpr, varExpr),
                    ">=" => Expression.GreaterThanOrEqual(propExpr, varExpr),
                    "<=" => Expression.LessThanOrEqual(propExpr, varExpr),
                    _ => _throwException()
                };


                Expression _throwException() {
                    throw new ArgumentException(
                        $"Invalid operator {Operator} for type {propertyType.Name}",
                        nameof(Operator)
                    );
                }
            }
        }

    }

}