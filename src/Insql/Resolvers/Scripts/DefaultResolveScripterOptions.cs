﻿namespace Insql.Resolvers.Scripts
{
    public class DefaultResolveScripterOptions
    {
        public bool IsConvertEnum { get; set; }

        public bool IsConvertOperator { get; set; }

        public bool IsConvertDateTimeMin { get; set; }

        public string[] ExcludeOperators { get; set; }
    }
}
