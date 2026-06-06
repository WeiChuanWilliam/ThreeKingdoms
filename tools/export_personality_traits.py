#!/usr/bin/env python3
"""Export Docs/PERSONALITY_TRAITS_TABLES.md → Assets/StreamingAssets/personality_traits.json"""

import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MD = ROOT / "Docs" / "PERSONALITY_TRAITS_TABLES.md"
OUT = ROOT / "Assets" / "StreamingAssets" / "personality_traits.json"

CAT_MAP = {
    "一、統御": "統御",
    "二、戰鬥": "戰鬥",
    "三、智略": "智略",
    "四、政治": "政治",
    "五、特殊": "特殊",
}

TIER_ORDER = {"紫": 0, "金": 1, "藍": 2, "紅": 3, "-": 4}
CAT_ORDER = {"統御": 0, "戰鬥": 1, "智略": 2, "政治": 3, "特殊": 4}

MUTEX_REF = {
    "A": [
        "向心", "威風", "樂奏", "才媛", "幻術", "妖術", "鬥將", "脫兔", "強行", "軍心",
        "一心", "長驅", "疾走", "孤狼", "豪傑", "解毒", "使役", "再起", "洞察", "破罠",
        "石兵", "滅火", "屯田", "反計", "施計", "慎重", "調配", "搬運",
        "富豪", "米道", "徵稅", "農政", "振興", "名聲", "文化", "教化",
        "支援", "協作", "強運",
    ],
    "C": ["龍膽", "常勝", "任才", "神威", "逆境", "孤狼", "猛虎"],
    "C2": ["膽識", "麒麟", "五胡", "南蠻", "胡人", "水戰", "山戰", "森戰", "蠻族", "地利", "使役", "宿將", "果敢"],
    "C3": ["掃蕩", "無畏"],
    "C4": ["堅守", "殿軍", "鐵壁"],
    "C5": ["遠矢", "操器"],
    "C6": ["崩壁", "操器"],
    "D": ["神將", "名將", "水將"],
    "D2": ["驍將"],
    "D3": ["剛將"],
    "D4": ["勇將"],
    "D5": ["督將"],
    "D6": ["工神"],
    "D7": ["才媛", "傾國"],
    "E": ["梟雄", "智將"],
    "E2": ["機略"],
    "E3": ["妙算"],
    "E4": ["詭計"],
    "E5": ["詐謀"],
    "G_morale_friend": ["樂奏", "傾國"],
    "G_morale_enemy": ["幻術", "鬼門"],
    "G_stamina_friend": ["應援"],
    "G_stamina_enemy": ["妖術", "鬼門"],
}


def main() -> None:
    text = MD.read_text(encoding="utf-8")
    sections = re.split(r"\n## ", text)
    traits = []
    seen = set()
    trait_names = set()

    for sec in sections:
        m = re.match(r"([一二三四五]、[^\n]+)", sec)
        if not m:
            continue
        title = m.group(1)
        cat = next((v for k, v in CAT_MAP.items() if title.startswith(k)), None)
        if not cat:
            continue
        for row in re.finditer(r"^\| ([^|]+?) \| ([紫金藍紅\-]) \| (.+?) \|$", sec, re.M):
            name = row.group(1).strip()
            tier = row.group(2).strip()
            effect = row.group(3).strip()
            if name in ("名稱", "----"):
                continue
            if name in seen:
                raise SystemExit(f"duplicate trait: {name}")
            seen.add(name)
            trait_names.add(name)
            traits.append({"name": name, "category": cat, "tier": tier, "effect": effect})

    for t in traits:
        parts = [p.strip() for p in re.split(r"[；;]", t["effect"]) if p.strip()]
        t["composes"] = [p for p in parts if p in trait_names]

    traits.sort(key=lambda x: (CAT_ORDER[x["category"]], TIER_ORDER.get(x["tier"], 9), x["name"]))
    for i, t in enumerate(traits, 1):
        t["id"] = i
        t["key"] = re.sub(r"[^a-zA-Z0-9\u4e00-\u9fff]+", "_", t["name"]).strip("_") or f"trait_{i}"

    purple = {cat: [t["name"] for t in traits if t["category"] == cat and t["tier"] == "紫"] for cat in CAT_ORDER}

    out = {
        "version": 1,
        "sourceDoc": "Docs/PERSONALITY_TRAITS_TABLES.md",
        "categoryOrder": list(CAT_ORDER.keys()),
        "tierOrder": ["紫", "金", "藍", "紅"],
        "composeSeparator": "；",
        "purplePerCategoryMax": 1,
        "purpleByCategory": purple,
        "pairedMutex": [["逆境", "高慢"]],
        "mutexGroups": MUTEX_REF,
        "traits": traits,
    }
    OUT.parent.mkdir(parents=True, exist_ok=True)
    OUT.write_text(json.dumps(out, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"Exported {len(traits)} traits → {OUT}")


if __name__ == "__main__":
    main()
