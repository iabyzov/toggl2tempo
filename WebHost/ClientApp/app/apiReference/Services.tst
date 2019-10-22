${
    using Typewriter.Extensions.WebApi;
    using System.IO;
    using File = Typewriter.CodeModel.File;

    const string ApiNamespace = "WebHost.ClientApi";

    string ServiceName(Class c) => c.Name.Replace("Controller", "Service");

    string FilePath(File f)
    {
        var @class = f.Classes.Single();
        var serviceName = ServiceName(@class);
        var fileName = ModuleName(@class) + "_" + Path.ChangeExtension(serviceName, "ts");

        var path = fileName;
        return path;
    }

    Template(Settings settings)
    {
        settings.OutputFilenameFactory = FilePath;
    }

    bool BelongsToClientApi(string @namespace) => @namespace.StartsWith(ApiNamespace) && !@namespace.Contains("CodeGenerationSupport");
    bool BelongsToClientApi(Class @class) => BelongsToClientApi(@class.Namespace) && !@class.Attributes.Any(a => a.Name == "ClientRefIgnore");
    bool BelongsToClientApi(Method @method) => !@method.Attributes.Any(a => a.Name == "ClientRefIgnore");
    bool BelongsToClientApi(Type t) => BelongsToClientApi(t.Namespace);

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

    bool IsVoid(Method method) => method.Type.Name == "void";

    bool IsString(Method method) => method.Type.Name == "string";

    bool NeedRequestData(Method method) => HttpMethodExtensions.HttpMethod(method) == "post" || method.RequestData() != "null";

    string ModuleName(string @namespace)
    {
        var relativeNamespace = @namespace.Replace(ApiNamespace, "").Remove(0, 1);
        return relativeNamespace.Split(new [] {"."}, StringSplitOptions.None).First();
    }

    string ModuleName(Class @class) => ModuleName(@class.Namespace);
    string ModuleName(Type t) => ModuleName(t.Namespace);

    IEnumerable<Type> ImportedTypes(Class c) => c.Methods
        .Where(m => BelongsToClientApi(m))
        .SelectMany(m => m.Parameters.Select(p => p.Type).Union(new [] { m.Type }))
        .Select(t => UnwrapCollections(t))
        .GroupBy(t => t.FullName)
        .Select(g => g.First())
        .Where(t => BelongsToClientApi(t));
    bool AnyMethodNeedRequestData(Class c) => c.Methods.Any(m => NeedRequestData(m));

    Type UnwrapCollections(Type t)
    {
        if(!t.IsGeneric) return t;
        if(!t.IsEnumerable) return t;

        var itemType = t.TypeArguments.Single();
        return itemType;
    }

    string ProduceTargetTypeName(Type t)
    {
        if(t == "Date")
        {
            return "string";
        }

        return t.Name;
    }

    string RewrittenTypeName(Type t)
    {
        var unwrappedType = UnwrapCollections(t);
        var hasUnwrapped = unwrappedType != t;

        var targetTypeName = ProduceTargetTypeName(unwrappedType);
        return hasUnwrapped ? targetTypeName + "[]" : targetTypeName;
    }

    string RewrittenTypeName(Parameter p) => RewrittenTypeName(p.Type);
    string RewrittenTypeName(Method m) => RewrittenTypeName(m.Type);
}$Classes(c => BelongsToClientApi(c) && IsController(c))[// ! Generated Code !

import {Http, Response$AnyMethodNeedRequestData[, Headers, RequestOptions]} from "@angular/http";
import {Injectable} from "@angular/core";
import {Observable} from "rxjs/Rx";
import 'rxjs/add/operator/map';

$ImportedTypes[import {$Name} from "./$ModuleName_$Name";
]
@Injectable()
export class $ServiceName {
    constructor(private http: Http) {
    }$Methods(m => BelongsToClientApi(m))[

    public $name($Parameters[$name: $RewrittenTypeName][, ]):Observable<$IsVoid[any][$RewrittenTypeName]> {$NeedRequestData[
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });]
        return this.http.$HttpMethod(`$Url`$NeedRequestData[, JSON.stringify($RequestData), options])
            .map(($IsVoid[_]res:Response) => $IsVoid[null][<$RewrittenTypeName>$IsString[res.text()][(res.text() ? res.json() : null)]]);
    }]
}]