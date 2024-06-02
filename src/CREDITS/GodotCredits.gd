extends Control

@export var title_color := Color.AQUA
@export var text_color := Color.WHITE
@export var title_size := 30 # should adjust section_time and line_time according to
@export var text_size := 13
@export var title_font: FontFile = null
@export var text_font: FontFile = null

const section_time := 0.7
const line_time := 0.35
const base_speed := 140
const speed_up_multiplier := 10.0

var scroll_speed: float = base_speed
var speed_up := false

@onready var line := $CreditsContainer/Line
var started := false
var finished := false

var section: Array
var section_next := true
var section_timer := 0.0
var line_timer := 0.0
var lines: Array[Label] = []

var credits = [
    [
        "A game by Wilbur's Family"
    ],[
        "Programming",
        "thanhbv",
    ],[
        "Art and Sound Effects",
        "Thomas Bui Viet Hung",
        "VERTICAL 2D GAME LEVEL MAP by MarwaMJ - marwamj.itch.io"
    ],[
        "Music",
        "Moonlight Sonata @ Ludwig Van Beethoven performed by Guss Griswold"
    ],[
        "Testers",
        "Wilbur Bui Viet",
        "Zoe Bui Gia Han",
    ],[
        "Tools used",
        "Developed with Godot Engine",
        "https://godotengine.org/license",
        "",
        "Art created with GIMP and Inkscape",
        "https://gimp.org",
        "https://inkscape.org",
        "",
        "Sound edit with Audacity",
        "https://audacityteam.org",
        "",
        "Use MIT License open source code from:",
        "https://github.com/MiDe-S/Credits-Godot"
    ],[
        "Special thanks",
        "Our Grandmother for delicious meals",
        "Our Mother Tham Mom for her gentleness and beauty",
    ]
]
var title_settings := LabelSettings.new()
var text_settings := LabelSettings.new()

func _ready():
    title_settings.font_color = title_color
    title_settings.font_size = title_size
    title_settings.font = title_font
    text_settings.font_color = text_color
    text_settings.font = text_font
    text_settings.font_size = text_size

func _process(delta):
    scroll_speed = base_speed * delta

    if section_next:
        section_timer += delta * speed_up_multiplier if speed_up else delta
        if section_timer >= section_time:
            section_timer -= section_time

            if !credits.is_empty():
                started = true
                section = credits.pop_front()
                add_line(title_settings)

    else:
        line_timer += delta * speed_up_multiplier if speed_up else delta
        if line_timer >= line_time:
            line_timer -= line_time
            add_line(text_settings)

    if speed_up:
        scroll_speed *= speed_up_multiplier

    if !lines.is_empty():
        for l in lines:
            l.set_global_position(l.get_global_position() - Vector2(0, scroll_speed))
            if l.get_global_position().y < l.get_line_height():
                lines.erase(l)
                l.queue_free()
    elif started:
        finish()

func finish():
    if not finished:
        finished = true
        var main_cs = load("res://src/Main.cs")
        main_cs.Back()

func add_line(lbl_settings: LabelSettings):
    var new_line: Label = line.duplicate()
    new_line.text = section.pop_front()
    new_line.label_settings = lbl_settings
    lines.append(new_line)
    $CreditsContainer.add_child(new_line)
    section_next = section.is_empty()

func _unhandled_input(event):
    if event.is_action_pressed("ui_cancel"):
        finish()
    if event.is_action_pressed("ui_down") and !event.is_echo():
        speed_up = true
    if event.is_action_released("ui_down") and !event.is_echo():
        speed_up = false
