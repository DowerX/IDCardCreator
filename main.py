import win32gui
import pyscreenshot
import subprocess

def getWindowPos(name):
    return win32gui.GetWindowRect(win32gui.FindWindow(None, name))

def saveScreen( path, _bbox):
    im = pyscreenshot.grab(bbox=_bbox)
    im.save(path)

if __name__ == "__main__":
    pos = getWindowPos("xd.txt - Notepad")
    saveScreen( "cam.png", pos)
    subprocess.call(["D:/Programming/c#/sdlidcard/sdlidcard/bin/Release/netcoreapp3.0/win-x64/sdlidcard.exe", "D:/Programming/c#/sdlidcard/sdlidcard/bin/Release/netcoreapp3.0/win-x64/IranNationalCard.png", "./cam.png", "./idcard.png"])