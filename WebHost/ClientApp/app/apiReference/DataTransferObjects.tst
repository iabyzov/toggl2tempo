${
    using System.IO;
    using System.Text;
    using File = Typewriter.CodeModel.File;

    string FilePath(File f)
    {
        if (f.Classes.Any()) {
            var @class = f.Classes.Single();
            var fileName = ModuleName(@class) + "_" + Path.ChangeExtension(@class.Name, "ts");

            var path = fileName;
            return path;
        } else {
            var @enum = f.Enums.Single();
            var fileName = ModuleName(@enum) + "_" + Path.ChangeExtension(@enum.Name, "ts");

            var path = fileName;
            return path;
        }
    }

    Template(Settings settings)
    {
        settings.OutputFilenameFactory = FilePath;
    }

    const string ApiNamespace = "WebHost.ClientApi";

    bool BelongsToClientApi(string @namespace) => @namespace.StartsWith(ApiNamespace) && !@namespace.Contains("CodeGenerationSupport");
    bool BelongsToClientApi(Class @class) => BelongsToClientApi(@class.Namespace) && !@class.Attributes.Any(a => a.Name == "ClientRefIgnore");
    bool BelongsToClientApi(Enum @enum) => BelongsToClientApi(@enum.Namespace) && !@enum.Attributes.Any(a => a.Name == "ClientRefIgnore");
    bool BelongsToClientApi(Type t) => BelongsToClientApi(t.Namespace) && !t.Attributes.Any(a => a.Name == "ClientRefIgnore");

    bool IsRecursivelyDefined(Property p) => p.Attributes.Any(a => a.Name == "RecursivelyDefined");

    bool IsController(Class @class)
    {
        var baseClass = @class.BaseClass;
        if(baseClass == null)
        {
            return false;
        }

        var rootClass = baseClass.BaseClass;

        return baseClass.Name == "Controller"
            || baseClass.Name == "CqrsControllerBase"
            || (rootClass != null && rootClass.Name == "CqrsControllerBase");
    }

    string ModuleName(string @namespace)
    {
        var relativeNamespace = @namespace.Replace(ApiNamespace, "").Remove(0, 1);
        return relativeNamespace.Split(new [] {"."}, StringSplitOptions.None).First();
    }

    string ModuleName(Class @class) => ModuleName(@class.Namespace);
    string ModuleName(Enum @enum) => ModuleName(@enum.Namespace);
    string ModuleName(Type t) => ModuleName(t.Namespace);

    Type UnwrapCollections(Type t)
    {
        if(!t.IsGeneric) return t;
        if(!t.IsEnumerable) return t;

        var itemType = t.TypeArguments.Single();
        return itemType;
    }

    string ProduceTargetTypeName(Type t, bool isRecursivelyDefined)
    {
        if(BelongsToClientApi(t))
        {
            return t.Name;
        }
        else
        {
            if(t == "Date")
            {
                return "string";
            }

            return t.Name;
        }
    }

    void AppendLineWithTabs (StringBuilder sb, string str, int i)
    {
        sb.Append(' ', i*4);
        sb.AppendLine(str);
    }

    string RewrittenTypeName(Type t, bool isRecursivelyDefined)
    {
      var unwrappedType = UnwrapCollections(t);
      var hasUnwrapped = unwrappedType != t;
      var targetTypeName = ProduceTargetTypeName(unwrappedType, isRecursivelyDefined);
      return hasUnwrapped ? targetTypeName + "[]" : targetTypeName;
    }

    string RewrittenTypeName(Property p)
    {
      return RewrittenTypeName(p.Type, IsRecursivelyDefined(p));
    }

    string RewrittenTypeNameForDecorator(Property p) {
        var unwrappedType = UnwrapCollections(p.Type);
        if(unwrappedType.IsEnum)
        {
            return "Number";
        }
        var rewrittenTypeName = ProduceTargetTypeName(unwrappedType, IsRecursivelyDefined(p));

        if(p.Type.IsPrimitive) {
            rewrittenTypeName = char.ToUpper(rewrittenTypeName[0]) + rewrittenTypeName.Substring(1);
        }
        return rewrittenTypeName;
    }

    bool IsEnumerable(Property p) => p.Type.IsEnumerable;
    bool HasProperties(Class c) => c.Properties.Any();
    bool HasBase(Class c) => c.BaseClass != null;
    string BaseClassModuleName(Class c) => ModuleName(c.BaseClass);
    IEnumerable<Type> PropertyTypesToImport(Class c) => c.Properties
        .Select(p => UnwrapCollections(p.Type))
        .Where(t => t.FullName != c.FullName)
        .GroupBy(t => t.FullName)
        .Select(g => g.First());

    const int maxOrderNum = int.MaxValue;
    const string orderIndexAttributeName = "OrderIndex";

    static string GenerateOrderedEnumValues(Enum e)
    {
        if(!e.Values.Any()) return string.Empty;
        var sb = new StringBuilder();
        var tabsCount = 1;

        // HACK [as] We need to use Tuple<> because typewriter doesnt support anonymous classes
        var t = e.Values
            .Where(x => !x.Attributes.Any(a => a.Name == "ClientRefIgnoreEnumValue"))
            .Select(x => new Tuple<EnumValue, Attribute>(x, x.Attributes.SingleOrDefault(a => a.Name == orderIndexAttributeName)))
            .Select(x => new Tuple<EnumValue, int>(x.Item1, x.Item2 != null ? int.Parse(x.Item2.Value) : maxOrderNum))
            .OrderBy(x => x.Item2);


        for (var i = 0; i < t.Count(); i++)
        {
            var value = t.ElementAt(i);
            var enumValueTemplate = "{0} = {1}";

            var isCommaRequired = i != t.Count() - 1;
            if(isCommaRequired)
            {
                enumValueTemplate += ",";
            }

            AppendLineWithTabs(sb, string.Format("/// {0}",  value.Item1.DocComment), tabsCount);
            AppendLineWithTabs(sb, string.Format(enumValueTemplate, value.Item1.Name, value.Item1.Value), tabsCount);
        }

        return sb.ToString();
    }

    string GenerateSwitchString(Enum e)
    {
        var sb = new StringBuilder();
        var tabsCount = 2;

        AppendLineWithTabs(sb, "switch(value)", tabsCount);
        AppendLineWithTabs(sb, "{", tabsCount++);
        foreach(var value in e.Values)
        {
            var ignoreAttribute = value.Attributes.SingleOrDefault(x=>x.Name == "ClientRefIgnoreEnumValue");
            if(ignoreAttribute != null)
            {
                continue;
            }
            
            var presentationAttribute = value.Attributes.SingleOrDefault(x=>x.Name == "StringValuePresentation");
            var stringValue = presentationAttribute != null ?
                presentationAttribute.Value :
                value.Name;

            AppendLineWithTabs(sb, $"case {e.Name}.{value.Name}:", tabsCount++);
            AppendLineWithTabs(sb, $"return \"{stringValue}\";", tabsCount--);
        }
        AppendLineWithTabs(sb, "}", --tabsCount);
        return sb.ToString();
    }

}// ! Generated Code ! $Classes(c => !IsController(c) && BelongsToClientApi(c))[
$PropertyTypesToImport[$BelongsToClientApi[import {$Name} from "./$ModuleName_$Name";
]]$HasBase[import {$BaseClass} from "./$BaseClassModuleName_$BaseClass";
]
export class $Name $HasBase[extends $BaseClass ][]{$Properties[
    public $name: $RewrittenTypeName;]
}
]$Enums(e=> BelongsToClientApi(e))[

// ReSharper disable InconsistentNaming
export enum $Name {
$GenerateOrderedEnumValues
}

export module $Name {
    export function getStringValuePresentation(value: $Name): string {
$GenerateSwitchString
    }
}


// ReSharper restore InconsistentNaming
]