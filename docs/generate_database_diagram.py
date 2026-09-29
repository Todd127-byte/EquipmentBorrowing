from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


OUTPUT = Path(__file__).with_name("database-diagram.png")
WIDTH, HEIGHT = 1600, 1260
BACKGROUND = "#111827"
CARD = "#1f2937"
HEADER = "#263b5c"
TEXT = "#f3f4f6"
MUTED = "#b7c3d4"
ACCENT = "#72b7f2"
GOLD = "#f6c85f"
LINE = "#9fb4cc"


def font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont | ImageFont.ImageFont:
    filename = "arialbd.ttf" if bold else "arial.ttf"
    path = Path("C:/Windows/Fonts") / filename
    if path.exists():
        return ImageFont.truetype(str(path), size)
    return ImageFont.load_default(size=size)


image = Image.new("RGB", (WIDTH, HEIGHT), BACKGROUND)
draw = ImageDraw.Draw(image)
title_font = font(42, bold=True)
subtitle_font = font(22)
table_font = font(26, bold=True)
field_font = font(21)
badge_font = font(18, bold=True)
footer_font = font(20)


def centered(text: str, y: int, selected_font: ImageFont.ImageFont, fill: str) -> None:
    bounds = draw.textbbox((0, 0), text, font=selected_font)
    x = (WIDTH - (bounds[2] - bounds[0])) // 2
    draw.text((x, y), text, font=selected_font, fill=fill)


centered("Campus Equipment Borrowing System", 62, title_font, TEXT)
centered("SQLite relational model · Entity Framework Core · InitialCreate", 118, subtitle_font, MUTED)
centered("One student or equipment item can have many borrowing records over time", 178, subtitle_font, ACCENT)


def table_card(
    x: int,
    y: int,
    width: int,
    title: str,
    rows: list[tuple[str, str]],
    min_height: int = 320,
) -> tuple[int, int, int, int]:
    row_height = 54
    header_height = 72
    height = max(min_height, header_height + row_height * len(rows) + 22)
    draw.rounded_rectangle((x, y, x + width, y + height), radius=20, fill=CARD, outline="#536579", width=2)
    draw.rounded_rectangle((x, y, x + width, y + header_height), radius=20, fill=HEADER)
    draw.rectangle((x, y + header_height - 18, x + width, y + header_height), fill=HEADER)
    draw.text((x + 24, y + 18), title, font=table_font, fill=TEXT)

    current_y = y + header_height + 12
    for name, detail in rows:
        selected_field_font = font(16) if len(name) > 20 else field_font
        draw.text((x + 24, current_y), name, font=selected_field_font, fill=TEXT)
        detail_width = draw.textbbox((0, 0), detail, font=badge_font)[2]
        badge_x = x + width - detail_width - 22
        draw.rounded_rectangle((badge_x - 9, current_y - 2, x + width - 15, current_y + 31), radius=10, fill="#30445d")
        draw.text((badge_x, current_y + 2), detail, font=badge_font, fill=ACCENT)
        current_y += row_height
    return x, y, width, height


def relation(start: tuple[int, int], end: tuple[int, int], label_x: int, label_y: int) -> None:
    draw.line((start, end), fill=LINE, width=4)
    draw.ellipse((start[0] - 7, start[1] - 7, start[0] + 7, start[1] + 7), fill=GOLD)
    direction = 1 if end[0] >= start[0] else -1
    base_x = end[0] - direction * 19
    draw.polygon(
        [end, (base_x, end[1] - 9), (base_x, end[1] + 9)],
        fill=GOLD,
    )
    draw.rounded_rectangle((label_x - 9, label_y - 5, label_x + 76, label_y + 30), radius=10, fill=BACKGROUND, outline="#536579", width=1)
    draw.text((label_x, label_y), "1 : many", font=badge_font, fill=GOLD)


relation((500, 620), (650, 690), 535, 632)
relation((1100, 620), (950, 690), 1000, 632)

student = table_card(
    85,
    300,
    450,
    "Students",
    [
        ("Id", "PK · INTEGER"),
        ("Name", "TEXT · NOT NULL"),
        ("IsAllowedToBorrow", "INTEGER · NOT NULL"),
        ("MaximumActiveBorrowings", "INTEGER · CHECK ≥ 0"),
    ],
)
equipment = table_card(
    1065,
    300,
    450,
    "Equipment",
    [
        ("Id", "PK · INTEGER"),
        ("Name", "TEXT · NOT NULL"),
        ("IsAvailable", "INTEGER · NOT NULL"),
    ],
)
borrowing = table_card(
    575,
    690,
    450,
    "Borrowings",
    [
        ("Id", "PK · GUID / TEXT"),
        ("StudentId", "FK · INTEGER"),
        ("EquipmentId", "FK · INTEGER"),
        ("DateBorrowed", "TEXT · NOT NULL"),
        ("ExpectedReturnDate", "TEXT · NOT NULL"),
        ("Status", "INTEGER · 0 / 1"),
    ],
    min_height=420,
)

footer_y = 1160
centered("Relationships use required foreign keys with ON DELETE RESTRICT", footer_y, footer_font, MUTED)
centered("Only one active borrowing per equipment item · Status check: Active (0) or Returned (1)", footer_y + 34, footer_font, MUTED)

image.save(OUTPUT, format="PNG", optimize=True)
print(f"Wrote {OUTPUT}")
