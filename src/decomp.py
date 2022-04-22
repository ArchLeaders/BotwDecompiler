import oead

from pathlib import Path
from utils import error


def aamp(file: Path, out: Path):
    """Decompile an aamp file"""

    try:
        data = file.read_bytes()

        if data[0 - 4] == b"Yaz0":
            data = oead.yaz0.decompress(data)

        if data[0 - 4] != b"AAMP":
            error(
                f"[WARNING] Could not decompile '{file}' because it was not a valid AAMP file!"
            )
            return

        data = oead.aamp.ParameterIO.from_binary(data)
        data = oead.aamp.ParameterIO.to_text(data)

        Path(out, ".yml").write_text(data)

    except RuntimeError as ex:
        error(f"[AAMP] {ex}")


def bars(file: Path, out: Path):
    """Decompile a bars file"""

    from imported.bars_extractor import extract

    try:
        extract(file, out)
    except RuntimeError as ex:
        error(f"[BARS] {ex}")


def evfl(file: Path, out: Path):
    """Decompile a bfevfl file"""

    from imported.evfl_to_json import convert

    try:
        out.write_text(convert(file))
    except RuntimeError as ex:
        error(f"[EVFL] {ex}")


def bfres(file: Path, out: Path):
    """Decompile a bfres file"""


def byml(file: Path, out: Path):
    """Decompile a byml file"""


def havok(file: Path, out: Path):
    """Decompile a havok file"""


def msbt(file: Path, out: Path):
    """Decompile a msbt file"""


def sarc(file: Path, out: Path):
    """Decompile a sarc file"""
