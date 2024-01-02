using Godot;
using System;
using System.Collections.Generic;

namespace Kids;
using RuleStats = Dictionary<int, RuleStat>; // Map level number to RuleStat

public partial class PlayerVars : Node {
	internal RuleStats Stats = [];
	public int Level = 9;
}
