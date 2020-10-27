
using System;
using System.Collections.Generic;

namespace Sini
{
    /// <summary>
    /// Configuration for parsing ini files.
    /// </summary>
    public struct IniConfig
    {
        /// <summary>
        /// Characters used as comment lines.
        /// </summary>
        public char[] CommentCharacters;

        /// <summary>
        /// If true, allow comments in the middle of the line, after the '=' sign.
        /// If false, will only allow comments as first character of every line.
        /// </summary>
        public bool AllowCommentsInLine;

        /// <summary>
        /// If true, will allow adding sections with square brackets.
        /// </summary>
        public bool AllowSections;

        /// <summary>
        /// Character used to split between key and value.
        /// </summary>
        public char SeparationCharacter;

        /// <summary>
        /// Regex used to validate keys and section names.
        /// Set to null to skip validations.
        /// </summary>
        public string KeyValidationRegex;

        /// <summary>
        /// Words to parse as 'true' when trying to parse booleans.
        /// </summary>
        public HashSet<string> BoolPositiveValues;

        /// <summary>
        /// Words to parse as 'false' when trying to parse booleans.
        /// </summary>
        public HashSet<string> BoolNegativeValues;

        /// <summary>
        /// If true, will always lowercase bool values before parsing them.
        /// </summary>
        public bool LowercaseBoolValues;

        /// <summary>
        /// Custom parsing methods for fetching custom types.
        /// </summary>
        public Dictionary<Type, Func<string, object>> CustomParsers;

        /// <summary>
        /// Return default configs.
        /// </summary>
        public static IniConfig Defaults()
        {
            IniConfig ret = new IniConfig();
            ret.CommentCharacters = new char[] { '#', ';' };
            ret.AllowCommentsInLine = true;
            ret.AllowSections = true;
            ret.SeparationCharacter = '=';
            ret.KeyValidationRegex = @"^[a-zA-Z_\.0-9]+$";
            ret.BoolPositiveValues = new HashSet<string>(new string[] { "1", "true", "yes", "on" });
            ret.BoolNegativeValues = new HashSet<string>(new string[] { "0", "false", "no", "off" });
            ret.LowercaseBoolValues = true;
            ret.CustomParsers = new Dictionary<Type, Func<string, object>>();
            return ret;
        }
    }
    
}
