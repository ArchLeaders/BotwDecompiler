import decomp
import sys
import os

from bcml import util
from pathlib import Path

out = ".\\decompiled"

if len(sys.argv) >= 2:
    out = sys.argv[2]

if not Path(out).is_dir():
    os.makedirs(out)

is_nx: bool = util.get_settings("wiiu")

dirs = {
    "game": util.get_game_dir(),
    "update": util.get_update_dir(),
    "dlc": f"{util.get_aoc_dir()}..\\",
}


def main():

    print("Scanning source files...")

    for key, dir in dirs.items():
        for file in Path(dir).glob("**/*.*"):

            # void update
            key = key.replace("update", "game")

            # get extension
            ext = str(file).split(".")
            ext = ext[len(ext) - 1]

            # get output path
            out_file = f"{out}\\{key}"
            out_file = str(file).replace(str(dir), out_file)

            # handle task
            # print(f"open<{os.path.basename(file)}>")
            with open(file, "rb") as fs:
                try:
                    decomp.ead(fs.read(), Path(out_file))
                except RuntimeError as ex:
                    print(ex)


if __name__ == "__main__":
    main()
