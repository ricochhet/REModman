// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace REMod.Models
{
    public class ModInfoBridge
    {
        public string? Name { get; set; }
        public string? Guid { get; set; }
        public string? Version { get; set; }
        public string? Description { get; set; }
        public string? GameType { get; set; }
        public TextBlock? LogBox { get; set; }

        public void Execute() { }
    }

}