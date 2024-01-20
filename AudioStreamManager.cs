using System.Collections.Generic;
using Godot;

namespace Kids;

public partial class AudioStreamManager : Node {
    private readonly StringName _soundBus = "Sound";
    private const int PlayerCount = 4;
    private readonly Queue<AudioStreamPlayer> _playerQueue = [];
    private readonly Queue<string> _audioPathQueue = [];

    public override void _Ready() {
        for (var i = 0; i < PlayerCount; i++) {
            var p = new AudioStreamPlayer { Bus = _soundBus };
            AddChild(p);
            _playerQueue.Enqueue(p);
            p.Finished += () => _playerQueue.Enqueue(p);
        }
    }

    public void Play(string path) {
        _audioPathQueue.Enqueue($"res://assets/{path}");
    }

    public void PlayClick() => Play("click.mp3");

    public override void _Process(double delta) {
        if (_playerQueue.IsEmpty() || _audioPathQueue.IsEmpty()) return;
        var p = _playerQueue.Dequeue();
        var audioPath = _audioPathQueue.Dequeue();
        p.Stream = ResourceLoader.Load<AudioStream>(audioPath);
        p.Play();
    }
}