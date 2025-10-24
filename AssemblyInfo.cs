using System.Runtime.CompilerServices;
using ZLinq;

[assembly: InternalsVisibleTo("CustomUtils.Editor")]

[assembly: ZLinqDropIn("CustomUtils.Runtime",
    DropInGenerateTypes.Array | DropInGenerateTypes.List | DropInGenerateTypes.Enumerable)]