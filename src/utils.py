from datetime import datetime
from pathlib import Path

def error(msg: str):
    print(msg)
    log_file = Path('.\\errno.log')

    log = ''
    if log_file.is_file():
        log: Path = log_file.read_text()

    log_file.write_text(f'{log}[{datetime.now().strftime("%r")}] {msg}\n')