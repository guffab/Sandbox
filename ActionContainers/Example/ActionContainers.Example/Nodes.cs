using Newtonsoft.Json;

namespace ActionContainers.Example;

record TemplateAction([JsonProperty("I")] string Id, [JsonProperty("P")] List<TemplateParameter> Parameters, [JsonProperty("T")] List<TypeAction> Types);

record TemplateParameter([JsonProperty("I")] string Id, [JsonProperty("U")] string Unit);

record TypeAction([JsonProperty("I")] string Id, [JsonProperty("V")] List<string> ParameterValues);
