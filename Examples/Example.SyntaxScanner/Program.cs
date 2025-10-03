using Example.SyntaxScanner;

var example1 = "Disassemble[Parameters[Definition.Name((PF_Number)|(Number))].AsElementId.Name(^Ass)].Name";
var example2 = "Disassemble[Parameters[Definition.Name((PF_Number)|(Number)).AsElementId.Name(^Ass)].Name";
var example3 = "Disassemble[Parameters[Definition.Name((PF_Number)|(Number)].AsElementId.Name(^Ass)].Name";
var example4 = "Disassemble[Parameters[PF_Number].AsString(^1)].Name";

var aa = ObjectPathParser.Parse(example1.AsSpan());
var bb = ObjectPathParser.Parse(example2.AsSpan());
var cc = ObjectPathParser.Parse(example3.AsSpan());
var dd = ObjectPathParser.Parse(example4.AsSpan());

var av = aa?.IsValid() ?? false; //expected: true
var bv = bb?.IsValid() ?? false; //expected: false
var cv = cc?.IsValid() ?? false; //expected: false
var dv = dd?.IsValid() ?? false; //expected: true
