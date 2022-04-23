import asyncio
import os
import sys
import oead
import exts
import decomp

from bcml import util
from pathlib import Path

out = ".\\decompiled_bta"

if len(sys.argv) >= 2:
    out = sys.argv[2]

if not Path(out).is_dir():
    os.makedirs(out)

is_nx: bool = util.get_settings("wiiu")

dirs = {
    "game": util.get_game_dir(),
    "update": util.get_update_dir(),
    "dlc": util.get_aoc_dir(),
}


async def main():

    print("Scanning source files...")

    # tasks = []

    for key, dir in dirs.items():
        for file in Path(dir).glob("**/*.*"):

            # void update
            key = key.replace("update", "game")

            # get extension
            ext = str(file).split(".")
            ext = ext[len(ext) - 1]

            # get output path
            out_file = f"{out}\\{key}"

            out_file = str(file).replace(str(dir), str(out_file))

            # print(f"await::<{file}>")

            if ext in exts.BARS_EXT:
                decomp.bars(file, out_file)

            elif ext in exts.BFEVFL_EXT:
                decomp.evfl(file, out_file)

            elif ext in exts.BFRES_EXT:
                decomp.bfres(file, out_file)

            elif ext in exts.BYML_EXTS:
                decomp.byml(file, out_file)

            elif ext in exts.HK_EXT:
                decomp.havok(file, out_file)

            elif ext in exts.MSBT_EXT:
                decomp.msbt(file, out_file)

            elif ext in exts.SARC_EXTS:
                decomp.sarc(file, out_file)

            else:
                decomp.copy(file, out_file)

    # asyncio.gather(*tasks)


if __name__ == "__main__":
    asyncio.run(main())
