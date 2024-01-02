using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Kids;

using RuleStats = Dictionary<int, RuleStat>; // Map level number to RuleStat

public partial class GameLevel : Control {
	public Button BackBtn => GetNode<Button>("backBtn");
	public Control WorkingArea => GetNode<Control>("MarginContainer/workingArea");
	public Label EquationLabel => WorkingArea.GetNode<Label>("Equation/Label");
	private Button[] buttons;

	private PlayerVars playerVars;
	private int level; // [0, MultiplyRule.Rules.length)
	public int Level => level;
	private List<MulEquation> equations;
	private int questionNumber = -1;

	[Signal]
	public delegate void AnswerEventHandler(int answer);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		buttons = WorkingArea.GetNode("buttonsGrid").GetChildren().Where(b => b is Button).Cast<Button>().ToArray();
		playerVars = GetNode<PlayerVars>("/root/PlayerVars");
		level = playerVars.Level;
		equations = Examples(playerVars.Stats);
		foreach (Button btn in buttons) {
			btn.Pressed += () => EmitSignal(SignalName.Answer, btn.Text.Trim().ToInt());
		}
		Answer += (answer) => {
			if (answer == equations[questionNumber].Result) {
				NextQuestion();
			}
		};

		NextQuestion();
	}

	private void NextQuestion() {
		questionNumber++;
		if (questionNumber > equations.Count) {
			// TODO
			return;
		}
		var e = equations[questionNumber];
		EquationLabel.Text = e.Question;
		var answers = TakeDistinces([e.Result], () => rnd.Next(101), buttons.Length).ToArray();
		rnd.Shuffle(answers);
		foreach (var (btn, answer) in buttons.Zip(answers)) {
			btn.Text = answer.ToString();
		}
	}


	private List<MulEquation> Examples(RuleStats stats) {
		if (Level == 0) {
			var r0 = (CompoundRule)MultiplyRule.Rules[0];
			TakeDistinces(() => r0.RandomEquation(rnd, true));
		}
		var (totalLosePercent, level2Percents) = stats
			.Where((p) => p.Key < Level)
			.Aggregate((0, (List<(int, int)>)[]), (acc, p) => {
				var (level, stat) = p;
				var (sum, list) = acc;
				sum += stat.LosePercent;
				list.Add((level, sum));
				return (sum, list);
			});

		var e = GetEquation(MultiplyRule.Rules[Level]);
		var ret = TakeDistinces([e, e.Swap], () => {
			var percent = rnd.Next(totalLosePercent);
			var level = level2Percents.FirstOrDefault(p => p.Item2 > percent).Item1;
			MultiplyRule r = MultiplyRule.Rules[level];
			var e = GetEquation(r);
			return rnd.NextBool() ? e : e.Swap;
		});
		ret.Swap(1, Random.Shared.Next(ret.Count));
		return ret;
	}

	private static readonly Random rnd = Random.Shared;
	private static MulEquation GetEquation(MultiplyRule r) => r switch {
		CompoundRule r1 => r1.RandomEquation(rnd),
		_ => (r as SimpleRule)!.ToEquation
	};

	private static List<T> TakeDistinces<T>(Func<T> rndGen, int count = 5) => TakeDistinces([], rndGen, count);
	private static List<T> TakeDistinces<T>(List<T> result, Func<T> rndGen, int count = 5) {
		for (var i = result.Count; i < count; i++) {
			while (true) {
				var t = rndGen();
				if (!result.Contains(t)) {
					result.Add(t);
					break;
				}
			}
		}
		return result;
	}
}
