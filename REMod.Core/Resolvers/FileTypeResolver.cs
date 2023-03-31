using REMod.Core.Resolvers.Enums;
using REMod.Core.Resolvers.Structs;
using System;

namespace REMod.Core.Resolvers
{
    public class FileTypeResolver
    {
        public static string File(FileType type) => type switch
        {
            FileType.Log => FileTypeStruct.Log,
            FileType.Cache => FileTypeStruct.Cache,
            FileType.Settings => FileTypeStruct.Settings,
            _ => throw new NotImplementedException(),
        };
    }
}
