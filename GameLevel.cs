using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Kids;

// Map level number to RuleStat
// Use Godot's Dictionary to support [Export]
// @see https://docs.godotengine.org/en/4.2/tutorials/scripting/c_sharp/diagnostics/GD0102.html
// @see https://docs.godotengine.org/en/stable/tutorials/scripting/resources.html
using RuleStats = Godot.Collections.Dictionary<int, RuleStat>;

public partial class GameLevel : Control {
	private Button BackBtn => GetNode<Button>("backBtn");
	private Control WorkingArea => GetNode<Control>("MarginContainer/workingArea");
	private HealthBox Health => WorkingArea.GetNode<HealthBox>("health");
	private Label EquationLabel => WorkingArea.GetNode<Label>("Equation/Label");
	private Button[] buttons; // lateinit

	private PlayerData playerData; // lateinit
	private List<MulEquation> equations; // lateinit
	private int questionNumber;

	[Signal] public delegate void HealthDownEventHandler();
	[Signal] public delegate void FinishLevelEventHandler(int level);

	public override void _Ready() {
		buttons = WorkingArea.GetNode("buttonsGrid").GetChildren().Where(b => b is Button).Cast<Button>().ToArray();
		playerData = PlayerData.Load();
		foreach (var btn in buttons) {
			btn.Pressed += () => OnAnswer(btn.Text.Trim().ToInt());
		}
		Health.HealthEmpty += () => {
			GD.PrintErr("TODO HealthEmpty");
		};
		FinishLevel += OnFinishLevel;
		LoadCurrentLevel();
	}

	private int Level => playerData.Level;

	private void OnAnswer(int answer) {
		var correct = answer == equations[questionNumber].Result;
		playerData.FinishQuestion(correct);
		if (correct) NextQuestion();
		else {
			EmitSignal(SignalName.HealthDown);
		}
	}
	private void OnFinishLevel(int level) {
		if (Level >= MultiplyRule.Rules.Length - 1) {
			GD.Print($"Finished!");
		} else {
			playerData.FinishLevel();
			LoadCurrentLevel();
		}
	}

	private void LoadCurrentLevel() {
		equations = Examples(playerData);
		questionNumber = -1;
		NextQuestion();
	}

	private void NextQuestion() {
		// if (questionNumber >= equations.Count - 1) {
		if (questionNumber >= 3) {
			GD.Print($"Passed level {Level}");
			EmitSignal(SignalName.FinishLevel, Level);
			// TODO
			return;
		}
		questionNumber++;
		var e = equations[questionNumber];
		EquationLabel.Text = e.Question;
		var answers = TakeDistinces([e.Result], () => rnd.Next(101), buttons.Length).ToArray();
		rnd.Shuffle(answers);
		foreach (var (btn, answer) in buttons.Zip(answers)) {
			btn.Text = answer.ToString();
		}
	}


	private const int QuestionsPerLevel = 4;
	private static List<MulEquation> Examples(PlayerData p) {
		if (p.Level == 0) {
			var r0 = (CompoundRule)MultiplyRule.Rules[0];
			TakeDistinces(() => r0.RandomEquation(rnd, true));
		}
		// Take random examples for rules in LOWER level. Random weight is stats.LosePercent
		var (totalLosePercent, level2Percents) = p.Stats
			.Where(e => e.Key < p.Level)
			.Aggregate((0, (List<(int, int)>)[]), (acc, pair) => {
				var (level, stat) = pair;
				var (sum, list) = acc;
				sum += stat.LosePercent;
				list.Add((level, sum));
				return (sum, list);
			});

		var e = GetEquation(MultiplyRule.Rules[p.Level]);

		List<MulEquation> ret = [e];
		if (e.Left != e.Right) ret.Add(e.Swap);
		TakeDistinces(ret, () => {
			var percent = rnd.Next(totalLosePercent);
			var level = level2Percents.FirstOrDefault(p => p.Item2 > percent).Item1;
			MultiplyRule r = MultiplyRule.Rules[level];
			var e = GetEquation(r);
			return rnd.NextBool() ? e : e.Swap;
		});
		if (e.Left != e.Right) ret.Swap(1, Random.Shared.Next(ret.Count));
		return ret;
	}

	private static readonly Random rnd = Random.Shared;
	private static MulEquation GetEquation(MultiplyRule r) => r switch {
		CompoundRule r1 => r1.RandomEquation(rnd),
		_ => (r as SimpleRule)!.ToEquation
	};

	private static List<T> TakeDistinces<T>(Func<T> rndGen, int count = QuestionsPerLevel) => TakeDistinces([], rndGen, count);
	private static List<T> TakeDistinces<T>(List<T> result, Func<T> rndGen, int count = QuestionsPerLevel) {
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
