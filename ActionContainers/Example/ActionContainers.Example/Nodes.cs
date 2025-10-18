using Newtonsoft.Json;

namespace ActionContainers.Example;

record TemplateAction([JsonProperty("I")] string Id, [JsonProperty("P")] IReadOnlyList<TemplateParameter> Parameters, [JsonProperty("T")] IReadOnlyList<TypeAction> Types);

record TemplateParameter([JsonProperty("I")] string Id, [JsonProperty("U")] string Unit = "");

record TypeAction([JsonProperty("I")] string Id, [JsonProperty("V")] IReadOnlyList<string> ParameterValues);
