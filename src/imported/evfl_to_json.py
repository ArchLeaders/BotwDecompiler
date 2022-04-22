# Modifed version of https://github.com/Nitr4m12/bfevfl_tools/blob/main/evfl_to_json.py | Credit to Nitr4m12

import json

from evfl import EventFlow
from evfl.repr_util import generate_flowchart_graph
from pathlib import Path


def convert(file: Path) -> str:
    flow = EventFlow()
    with open(file, "rb") as f:
        flow.read(f.read())

    return json.dumps(
        generate_flowchart_graph(flow), indent=4, default=lambda x: str(x)
    )
