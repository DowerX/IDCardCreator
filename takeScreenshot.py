import win32gui
import pyscreenshot
import subprocess

def saveScreen( path, _bbox):
    im = pyscreenshot.grab(bbox=_bbox)
    im.save(path)

if __name__ == "__main__":
    #hwnd = win32gui.FindWindow(None, "EOS Camera Movie Record")
    hwnd = win32gui.FindWindow(None, "xd.txt - Notepad")
    win32gui.SetForegroundWindow(hwnd)
    pos = win32gui.GetWindowRect(hwnd)
    saveScreen("temp.png", pos)
    win32gui.ShowWindow(hwnd, 0)
    subprocess.call(["./sdlidcard/bin/Release/netcoreapp3.0/win-x64/sdlidcard.exe", "./szemelyi.png", "./temp.png", "./idcard.png"])