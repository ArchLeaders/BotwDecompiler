# -*- coding: utf-8 -*-
#!/usr/bin/python3

"""bars_extractor.py: Extracts BFWAV files from Wii U (and possible 3DS, but this is untested) BARS files"""

__author__ = "Peter Wunder (@SamusAranX)"
__license__ = "WTFPL"
__version__ = "1.1"

import os
import sys
import glob
import struct

# Magic numbers, commonly known as "headers"
BARS_HEADER = b"BARS"
AMTA_HEADER = b"AMTA"
DATA_HEADER = b"DATA"
MARK_HEADER = b"MARK"
EXT_HEADER  = b"EXT_"
STRG_HEADER = b"STRG"
FWAV_HEADERS = [b"FWAV", b"FSTP"]

# The Python structs to go with the above headers
BARS_HEADER_STRUCT = struct.Struct(">4sIH2xI")
AMTA_HEADER_STRUCT = struct.Struct(">4sH2x5I")
DATA_HEADER_STRUCT = struct.Struct(">4sI")
MARK_HEADER_STRUCT = struct.Struct(">4sI")
EXT_HEADER_STRUCT  = struct.Struct(">4sI")
STRG_HEADER_STRUCT = struct.Struct(">4sI")
FWAV_HEADER_STRUCT = struct.Struct(">4s8xI8x2I32x")

def plural_s(n):
	return "s" if n != 1 else ""

def extract_from_bars(fname):
	with open(fname, "rb") as f:
		bars_header, bars_file_length, bars_endianness, bars_count = BARS_HEADER_STRUCT.unpack(f.read(BARS_HEADER_STRUCT.size))
		bars_track_struct = struct.Struct(f">{bars_count*4}x{bars_count*2}I")
		bars_track_offsets = bars_track_struct.unpack(f.read(bars_track_struct.size))

		if bars_header != BARS_HEADER:
			raise RuntimeError(f"{f.name}: Not a valid BARS file.")

		file_size = os.fstat(f.fileno()).st_size
		# if bars_file_length != file_size:
			# raise RuntimeError(f"{f.name}: File size mismatch (expected {bars_file_length} bytes, got {file_size} bytes)")

		track_names = []
		
		for t in range(bars_count):
			bars_amta_offset = bars_track_offsets[t * 2] # Get AMTA offset from list
			f.seek(bars_amta_offset)

			amta_bytes = f.read(AMTA_HEADER_STRUCT.size)
			amta_header, amta_endianness, amta_length, data_offset, mark_offset, ext_offset, strg_offset = AMTA_HEADER_STRUCT.unpack(amta_bytes)

			if amta_header != AMTA_HEADER:
				raise RuntimeError(f"{f.name}: Track {t+1} has an invalid AMTA header")

			data_bytes = f.read(DATA_HEADER_STRUCT.size)
			data_header, data_length = DATA_HEADER_STRUCT.unpack(data_bytes)
			if data_header != DATA_HEADER:
				raise RuntimeError(f"{f.name}: Track {t+1} has an invalid DATA header")

			data = f.read(data_length)

			mark_bytes = f.read(MARK_HEADER_STRUCT.size)
			mark_header, mark_length = MARK_HEADER_STRUCT.unpack(mark_bytes)
			if mark_header != MARK_HEADER:
				raise RuntimeError(f"{f.name}: Track {t+1} has an invalid MARK header")

			mark = f.read(mark_length)

			ext_bytes = f.read(EXT_HEADER_STRUCT.size)
			ext_header, ext_length = EXT_HEADER_STRUCT.unpack(ext_bytes)
			if ext_header != EXT_HEADER:
				raise RuntimeError(f"{f.name}: Track {t+1} has an invalid EXT_ header")

			ext = f.read(ext_length)

			strg_bytes = f.read(STRG_HEADER_STRUCT.size)
			strg_header, strg_length = STRG_HEADER_STRUCT.unpack(strg_bytes)
			if strg_header != STRG_HEADER:
				raise RuntimeError(f"{f.name}: Track {t+1} has an invalid STRG header")

			# Read track name and convert the resulting byte sequence to an UTF8 string
			strg = f.read(strg_length).decode("utf8")
			track_names.append(strg)

		print(f"{f.name}: {bars_count} track{plural_s(bars_count)} found!")

		if f.tell() == bars_file_length: # We have now reached the end of the file despite the file telling us that there would be stuff here
			raise RuntimeError(f"{f.name}: Reached EOF, this file probably doesn't actually contain any FWAVs despite containing the offsets for them")

		for t in range(bars_count):
			bars_track_offset = bars_track_offsets[t * 2 + 1] # Get track offset from list
			if bars_track_offset >= bars_file_length: # The offset the file is telling us to jump to can't exist because the file's too small
				print(f"{f.name}: Track {t+1} probably doesn't exist, skipping it")
				continue

			f.seek(bars_track_offset) # seek to the next FWAV header

			fwav_header_bytes = f.read(FWAV_HEADER_STRUCT.size)
			fwav_header, fwav_length, fwav_info_offset, fwav_data_offset = FWAV_HEADER_STRUCT.unpack(fwav_header_bytes)
			if fwav_header not in FWAV_HEADERS:
				print(f"{f.name}: Track {t+1} has an invalid FWAV header")
				continue

			f.seek(-FWAV_HEADER_STRUCT.size, os.SEEK_CUR) # seek back to the start of the FWAV data...
			fwav_data = f.read(fwav_length) # ...so that we can read it out in one big chunk

			track_ext = [".bfwav", ".bfstp"][FWAV_HEADERS.index(fwav_header)] # Give the output file a different extension depending on content
			track_name = track_names[t].replace("\0", "_")[:24] # Remove null bytes and trim potentially LONG file names
			bfwav_name = os.path.splitext(f.name)[0] + "_" + track_name + track_ext # Construct output file name

			with open(bfwav_name, "wb") as wf:
				wf.write(fwav_data) # write the data to a BFWAV file
				print(f"{f.name}: Saved track {t+1} to {bfwav_name}")

def main():
	if len(sys.argv) == 1: # no arguments
		print("No input files specified.")
		sys.exit(1)

	if len(sys.argv) == 2: # one argument
		files = glob.glob(sys.argv[1])
	else: # more than one argument
		files = sys.argv[1:]

	for _f in files:
		try:
			extract_from_bars(_f)
		except RuntimeError as e:
			print(e)

if __name__ == '__main__':
	main()