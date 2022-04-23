import subprocess
import oead

from pathlib import Path
from utils import error


async def aamp(file: Path, out: Path):
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


async def bars(file: Path, out: Path):
    """Decompile a bars file"""

    from imported.bars_extractor import extract

    try:
        extract(file, out)
    except RuntimeError as ex:
        error(f"[BARS] {ex}")


async def evfl(file: Path, out: Path):
    """Decompile a bfevfl file"""

    from imported.evfl_to_json import convert

    try:
        out.write_text(convert(file))
    except RuntimeError as ex:
        error(f"[EVFL] {ex}")


async def bfres(file: Path, out: Path):
    """Decompile a bfres file"""

    try:
        subprocess.check_call([".\\lib\\DecompileBfres.exe", f"{file}", f"{out}"])
    except RuntimeError as ex:
        error(f"[BFRES] {ex}")


async def byml(file: Path, out: Path):
    """Decompile a byml file"""

    try:
        data = file.read_bytes()

        if data[0 - 4] == b"Yaz0":
            data = oead.yaz0.decompress(data)

        if data[0 - 2] != b"BY" and data[0 - 2] != b"YB":
            error(
                f"[WARNING] Could not decompile '{file}' because it was not a valid BYML file!"
            )
            return

        data = oead.byml.from_binary(data)
        data = oead.aamp.to_text(data)

        Path(out, ".yml").write_text(data)

    except RuntimeError as ex:
        error(f"[BYML] {ex}")


async def havok(file: Path, out: Path):
    """Decompile a havok file"""


async def msbt(file: Path, out: Path):
    """Decompile a msbt file"""


async def sarc(file: Path, out: Path):
    """Decompile a sarc file"""
