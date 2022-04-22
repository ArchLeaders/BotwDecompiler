import oead

from pathlib import Path
from utils import error

def aamp(file: Path, out: Path):
    """Decompile an aamp file"""
    
    data = file.read_bytes()

    if data[0-4] == b'Yaz0':
        data = oead.yaz0.decompress(data)

    if data[0-4] != b'AAMP':
        error(f'[WARNING] Could not decompile \'{file}\' because it was not a valid AAMP file!')
        return
        
    data = oead.aamp.ParameterIO.from_binary(data)
    data = oead.aamp.ParameterIO.to_text(data)

    Path(out, '.yml').write_text(data)

def bars(file: Path, out: Path):
    """Decompile an bars file"""
    

def evfl(file: Path, out: Path):
    """Decompile an bfevfl file"""
    

def bfres(file: Path, out: Path):
    """Decompile an bfres file"""
    

def byml(file: Path, out: Path):
    """Decompile an byml file"""
    

def havok(file: Path, out: Path):
    """Decompile an havok file"""
    

def msbt(file: Path, out: Path):
    """Decompile an msbt file"""


def sarc(file: Path, out: Path):
    """Decompile an sarc file"""