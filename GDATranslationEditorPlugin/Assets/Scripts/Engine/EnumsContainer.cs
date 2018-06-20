using System.Collections.Generic;
using System;
using System.Linq;

namespace ForExperienceTools{
    public enum Languages
    {
        POLISH,
        ENGLISH,
        GERMAN,
        SPANISH
    }

    public enum TranslationOptions
    {
        Text,
        TMProText,
        VRTKTooltip
    }

    public enum Extensions
    {
        //XLS,
        //XLSX,
        TXT,
        CSV,
        DAT
    }

    public enum RenameType
    {
        Selection, 
        Update
    }

    public enum GDEFiledTypes
    {
        STRING,
        INT,
        FLOAT,
        DOUBLE
    }

    public static class EnumUtils
    {
        public static string GetSelectedEnum<T>(int index)
        {
            return Enum.GetName(typeof(T), index).ToLower();
        }

        public static string[] GetEnumNames<T>()
        {
            return Enum.GetNames(typeof(T));
        }

        public static IEnumerable<T> GetEnumTypeValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}


