import json
import subprocess
import oead
import os

from botw_havok import Havok
from evfl import EventFlow
from evfl.repr_util import generate_flowchart_graph
from pathlib import Path, WindowsPath
from utils import error
from zlib import crc32


def aamp(data: bytes, out: Path):
    """Decompile an aamp file"""

    if data[0:4] != b"AAMP":
        error(f"[WARNING] Could not decompile AAMP file!")
        return

    cdir(out)

    data = oead.aamp.ParameterIO.from_binary(data)
    data = oead.aamp.ParameterIO.to_text(data)

    Path(f"{out}.yml").write_bytes(str(data).encode())

    print("[AAMP] Decompiled aamp file from memory")


def bars(file: Path, out: Path):
    """Decompile a bars file"""

    from imported.bars_extractor import extract

    cdir(out, True)
    extract(file, out)


def evfl(data: bytes, out: Path):
    """Decompile a bfevfl file"""

    cdir(out)

    out = Path(f"{out}.json")

    flow: EventFlow = EventFlow()
    flow.read(data)

    out.write_bytes(
        json.dumps(
            generate_flowchart_graph(flow),
            indent=4,
            default=lambda x: str(x),
        ).encode()
    )

    print("[EVFL] Decompiled event flow file from memory")


def fres(file: Path, out: Path):
    """Decompile a bfres file"""

    cdir(out, True)
    subprocess.check_call([".\\lib\\Decompiler.Bfres.exe", f"{file}", f"{out}"])


def byml(data: bytes, out: Path):
    """Decompile a byml file"""

    if data[0:2] != b"BY" and data[0:2] != b"YB":
        error(f"[WARNING] Could not decompile BYML file!")
        return

    cdir(out)

    data = oead.byml.from_binary(data)
    data = oead.byml.to_text(data)

    Path(f"{out}.yml").write_bytes(str(data).encode())

    print("[BYML] Decompiled binary yaml file from memory")


def havok(data: bytes, out: Path):
    """Decompile a havok file"""

    cdir(out)
    hk = Havok.from_bytes(data)
    hk.deserialize()
    hk.to_json(Path(f"{out}.json"), True)

    print("[HAVOK] Decompiled havok file from memory")


def msbt(file: Path, out: Path):
    """Decompile a msbt file"""

    cdir(out)
    print(f"[MSBT] [SHELL] Decompile {file.stem}")
    subprocess.check_call([".\\lib\\Msyt.exe", "export", f"-o", out, f"{file}"])


def sarc(data: bytes, out: Path):
    """Decompile a sarc file"""

    print("[SARC] Parsing sarc archive from memory...")

    # create Sarc
    data = oead.Sarc(data)

    for sfile in data.get_files():

        sdata = bytes(sfile.data)
        out_file = Path(out, sfile.name)
        temp_file = Path(out, f"{sfile.name}.temp")

        cdir(out_file)
        ead(sdata, out_file)

    print("[SARC] Parsed sarc in memory")


def ead(file: bytes or Path, out: Path, conf: dict):

    data = b"\x00"

    if type(file) == WindowsPath:
        data = Path(file).read_bytes()
    elif type(file) == bytes:
        data = file

    temp: Path = Path(f".\\temp\\[{crc32(data)}].bin")

    # decompress yaz0
    if data[0:4] == b"Yaz0":
        data = oead.yaz0.decompress(data)

    if data[0:4] == b"AAMP" and conf["aamp"] == True:
        aamp(data, out)

    elif data[0:4] == b"BARS" and conf["bars"] == True:
        if type(file) != bytes:
            bars(file, out)
        else:
            cdir(temp)
            temp.write_bytes(data)
            bars(temp, out)
            temp.unlink()

    elif data[0:6] == b"BFEVFL" and conf["evfl"] == True:
        evfl(data, out)

    elif data[0:4] == b"FRES" and conf["fres"] == True:
        if type(file) != bytes:
            fres(file, out)
        else:
            cdir(temp)
            temp.write_bytes(data)
            fres(temp, out)
            temp.unlink()

    elif data[0:2] == b"BY" or data[0:2] == b"YB" and conf["byml"] == True:
        byml(data, out)

    elif data[0:8] == b"\x57\xE0\xE0\x57\x10\xC0\xC0\x10" and conf["havk"] == True:
        havok(data, out)

    elif data[0:8] == b"\x4D\x73\x67\x53\x74\x64\x42\x6E" and conf["msbt"] == True:
        if type(file) != bytes:
            msbt(file, out)
        else:
            cdir(temp)
            temp.write_bytes(data)
            msbt(temp, out)
            temp.unlink()

    elif data[0:4] == b"SARC" and conf["sarc"] == True:
        sarc(data, out)

    elif conf["copy"] == True:
        cdir(out)
        print(f"[WRITE] {os.path.basename(out)}")
        out.write_bytes(data)

    else:
        print(f"[SKIPPED] {os.path.basename(out)}")
        return


def cdir(out: Path, is_archive: bool = False):

    if is_archive:
        os.makedirs(out, exist_ok=True)
    else:
        os.makedirs(os.path.dirname(out), exist_ok=True)
