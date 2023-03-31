using REMod.Core.Resolvers.Enums;
using REMod.Core.Resolvers.Structs;
using System;

namespace REMod.Core.Resolvers
{
    public class FolderTypeResolver
    {
        public static string Folder(FolderType type) => type switch
        {
            FolderType.Data => FolderTypeStruct.Data,
            FolderType.Mods => FolderTypeStruct.Mods,
            FolderType.Downloads => FolderTypeStruct.Downloads,
            _ => throw new NotImplementedException(),
        };
    }
}
