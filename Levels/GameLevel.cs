using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kids.LevelMaps;
using Kids.Models;

namespace Kids.Levels;

enum AnswerMode {
    Choise,
    NumPad
}

public partial class GameLevel : TextureRect {
    [GetNode("%")] private GridContainer _buttonsGrid = null!;
    [GetNode("%")] private NumPad _numPad = null!;
    [GetNode("%")] private EquationBox _equationBox = null!;
    [GetNode("%")] private ProgressBar _progress = null!;
    private Button[] _buttons = null!; // @onready
    private List<MulEquation> _equations = null!; // @onready
    private PlayerData _playerData = PlayerData.Load();
    private int _questionNumber;

    private AnswerMode _mode = AnswerMode.Choise;

    private AnswerMode Mode {
        get => _mode;
        set {
            if (_mode == value) return;
            _mode = value;
            _buttonsGrid.Visible = _mode == AnswerMode.Choise;
            _numPad.Visible = _mode == AnswerMode.NumPad;
        }
    }

    private int _level;

    [Signal] public delegate void HealthDownEventHandler();

    [Signal] public delegate void FinishLevelEventHandler();

    [Signal] public delegate void AnswerDoneEventHandler();

    public static GameLevel Load(int level) {
        var ret = (GameLevel)ResourceLoader.Load<PackedScene>("res://Levels/GameLevel.tscn").Instantiate();
        ret._level = level;
        return ret;
    }

    public override void _Ready() {
        GetNode<TextureButton>("%BackBtn").WithSound().Pressed += Main.Back;

        _buttons = _buttonsGrid.GetChildren().Where(b => b is Button).Cast<Button>().ToArray();
        foreach (var btn in _buttons) {
            btn.Pressed += () => OnAnswer(btn.Text.Trim().ToInt());
        }

        _numPad.Submit += OnAnswer;
        _numPad.ValueChanged += value => _equationBox.Result.Text = value;

        GetNode<HealthBox>("%Health").HealthEmpty += () => {
            Main.Audio.Play("game-over.ogg");
            GetTree().CreateTimer(0.5).Timeout += Main.Back;
        };
        AnswerDone += OnAnswerDone;
        FinishLevel += OnFinishLevel;
        LoadCurrentLevel(AnswerMode.Choise);
    }

    private void LoadCurrentLevel(AnswerMode mode) {
        Mode = mode;
        _equations = Examples(_playerData, _level);
        _questionNumber = -1;
        if (mode == AnswerMode.Choise) _progress.Value = 0;
        NextQuestion();
    }

    private void NextQuestion() {
        if (++_questionNumber >= _equations.Count) {
            EmitSignal(SignalName.AnswerDone);
            return;
        }

        var e = _equations[_questionNumber];
        _equationBox.TypingEffect(e.Question);
        switch (Mode) {
            case AnswerMode.Choise:
                var answers = TakeUniques([e.Result], () => Rnd.Next(101), _buttons.Length).ToArray();
                Rnd.Shuffle(answers);
                foreach (var (btn, answer) in _buttons.Zip(answers)) {
                    btn.Text = answer.ToString();
                }

                break;
            case AnswerMode.NumPad:
                _numPad.Reset();
                break;
        }
    }

    private void OnAnswer(int answer) {
        var correct = answer == _equations[_questionNumber].Result;
        _playerData.FinishQuestion(correct, _level);
        if (correct) {
            _progress.Value += _progress.MaxValue / (2 * _equations.Count);
            NextQuestion();
        } else EmitSignal(SignalName.HealthDown);
    }

    private void OnAnswerDone() {
        switch (Mode) {
            case AnswerMode.Choise:
                LoadCurrentLevel(AnswerMode.NumPad);
                break;
            case AnswerMode.NumPad:
                EmitSignal(SignalName.FinishLevel);
                break;
        }
    }

    private void OnFinishLevel() {
        if (_level >= MultiplyRule.Rules.Length - 1) {
            GD.Print($"TODO Finished!");
        } else {
            Main.Audio.Play("win.mp3");
            if (_level == _playerData.Level) Main.Scene<LevelMap>(Main.LevelMapName).LevelScroll(1);
            GetTree().CreateTimer(1).Timeout += Main.Back;
            _playerData.FinishLevel(_level);
        }
    }

    private const int QuestionsPerLevel = 4;

    private static List<MulEquation> Examples(PlayerData p, int level) {
        if (level == 0) {
            var r0 = (CompoundRule)MultiplyRule.Rules[0];
            TakeUniques([], () => r0.RandomEquation(Rnd, true));
        }

        // Take random examples for rules in LOWER level. Random weight is stats.LosePercent
        var (totalLosePercent, level2Percents) = p.Stats
            .Where(e => e.Key < level)
            .Aggregate((0, (List<(int, int)>) []), (acc, pair) => {
                var (level, stat) = pair;
                var (sum, list) = acc;
                sum += stat.LosePercent;
                list.Add((level, sum));
                return (sum, list);
            });

        var e = GetEquation(MultiplyRule.Rules[level]);

        List<MulEquation> ret = [e];
        if (e.Left != e.Right) ret.Add(e.Swap);
        TakeUniques(ret, () => {
            var percent = Rnd.Next(totalLosePercent);
            var level = level2Percents.FirstOrDefault(p => p.Item2 > percent).Item1;
            MultiplyRule r = MultiplyRule.Rules[level];
            var e = GetEquation(r);
            return Rnd.NextBool() ? e : e.Swap;
        });
        if (e.Left != e.Right) ret.Swap(1, Random.Shared.Next(ret.Count));
        return ret;
    }

    private static readonly Random Rnd = Random.Shared;

    private static MulEquation GetEquation(MultiplyRule r) => r switch {
        CompoundRule r1 => r1.RandomEquation(Rnd),
        _ => (r as SimpleRule)!.ToEquation
    };

    private static List<T> TakeUniques<T>(List<T> result, Func<T> rndGen, int count = QuestionsPerLevel) {
        for (var i = result.Count; i < count; i++) {
            while (true) {
                var t = rndGen();
                if (result.Contains(t)) continue;
                result.Add(t);
                break;
            }
        }

        return result;
    }
}