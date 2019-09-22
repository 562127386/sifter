using System;
using System.Text.RegularExpressions;


namespace Sifter.Models {

    internal enum SimpleType {

        STRING,
        NUMBER,
        BOOL,
        ENUM

    }


    internal static class SimpleTypeExtensions {

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