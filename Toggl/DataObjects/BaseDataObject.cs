using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Toggl.DataObjects
{
    public class BaseDataObject
    {
        public List<KeyValuePair<string, string>> ToKeyValuePair()
        {
            var lst = new List<KeyValuePair<string, string>>();

            //Type type = GetType();
            //type.GetProperties();
            GetType().GetRuntimeProperties().ToList()
                .ForEach(p =>
                         {
                             var val = p.GetValue(this, null);
                             var jsonProperty = p.GetCustomAttributes(typeof(JsonPropertyAttribute), false).Single() as JsonPropertyAttribute;

                             if (jsonProperty == null || val == null) return;

                             var ints = val as IEnumerable<int>;

                             if (ints != null)
                             {
                                 var param = string.Join(",", ints);
                                 var pair = new KeyValuePair<string, string>(jsonProperty.PropertyName, param);
                                 lst.Add(pair);
                             }
                             else
                             {
                                 var pair = new KeyValuePair<string, string>(jsonProperty.PropertyName, val.ToString());
                                 lst.Add(pair);
                             }
                         });

            return lst;
        }

        public string ToJson(string objName = "")
        {
            var cverts = new List<JsonConverter>();
            cverts.Add(new IsoDateTimeConverter());
            var data = JsonConvert.SerializeObject(this,
                                                Formatting.None,
                                                new JsonSerializerSettings
                                                {
                                                    NullValueHandling = NullValueHandling.Ignore,
                                                    Converters = cverts
                                                }

                                );
            var propNm = (string.IsNullOrEmpty(objName)) ? GetType().Name.LowerCaseUnderscore() : objName;

            return "{\"" + propNm + "\":" + data + "}";
        }

    }
}
