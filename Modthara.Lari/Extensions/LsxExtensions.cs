﻿using Modthara.Lari.Lsx;

namespace Modthara.Lari.Extensions;

public static class LsxExtensions
{
    public static List<Module> ToShortDescModules(this IEnumerable<LsxNode> nodes) =>
        nodes.Where(x => x.Id == "ModuleShortDesc").Select(x => new Module(x)).ToList();

    public static LariUuid GetUuid(this LsxNode node) => new(node.GetAttribute("UUID").Value);

    public static LariVersion GetVersion(this LsxNode node) =>
        new(Convert.ToUInt64((node.GetAttributeOrDefault("Version64") ??
                              node.GetAttribute("Version")).Value));

    public static int GetInt32(this LsxAttribute attribute) => Convert.ToInt32(attribute.Value);
}
